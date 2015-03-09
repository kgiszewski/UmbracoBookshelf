using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using umbraco.presentation.plugins.tinymce3;
using Umbraco.Core.IO;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using ClientDependency.Core;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using umbraco.cms.businesslogic.packager;
using umbraco.presentation.umbraco;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using UmbracoBookshelf.Models;

namespace UmbracoBookshelf.Controllers
{
    [PluginController("UmbracoBookshelfApi")]
    public class UmbracoBookshelfController : UmbracoAuthorizedJsonController
    {
        public object GetFileContents(string filePath)
        {
            try
            {
                var systemFilePath = getSystemPath(filePath);

                var extension = Path.GetExtension(systemFilePath);

                if (!extension.EndsWith(Helpers.Constants.ALLOWED_FILE_EXTENSION))
                {
                    throw new WebException("Invalid file type");
                }

                var content = File.ReadAllText(systemFilePath);

                return new
                {
                    Content = content
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error<Exception>(ex.Message, ex);

                return new
                {
                    Content = ""
                };
            }
        }

        public object GetFolderContents(string dirPath)
        {
            var systemFilePath = "";

            var readme = "";

            try
            {
                systemFilePath = getSystemPath(dirPath);

               readme = Directory.GetFiles(systemFilePath)
                    .FirstOrDefault(x => Path.GetFileName(x) == Helpers.Constants.FOLDER_FILE);

               var content = File.ReadAllText(readme);

               return new
               {
                   Content = content
               };
            }
            catch (Exception ex)
            {
                LogHelper.Error<Exception>(ex.Message, ex);

                return new
                {
                    Content = ""
                };
            }
        }

        [HttpPost]
        public object SaveFile(FileSaveModel model)
        {
            var systemFilePath = getSystemPath(model.FilePath);

            if(!systemFilePath.EndsWith(Helpers.Constants.ALLOWED_FILE_EXTENSION))
            {
                throw new WebException("Invalid file type");
            }

            File.WriteAllText(systemFilePath, model.Content);

            return new
            {
                Status = "Saved"
            };
        }

        [HttpPost]
        public object Delete(DeletePathModel model)
        {
            LogHelper.Info<UmbracoBookshelfTreeController>("trying to kill:" + model.Path);

            var systemPath = getSystemPath("/" + model.Path, 1);

            var isDirectory = File.GetAttributes(systemPath).HasFlag(FileAttributes.Directory);

            if (isDirectory)
            {
                Directory.Delete(systemPath, true);
            }
            else
            {
                File.Delete(systemPath);
            }

            return new
            {
                Status = "Deleted."
            };
        }

        [HttpGet]
        public object DownloadUrl(string url)
        {
            try
            {
                var downloadsDirectory = IOHelper.MapPath(@"c:/temp/UmbracoBookshelf/" + DateTime.Now.Ticks + "/");
                var fileName = Path.GetFileName(url);

                Directory.CreateDirectory(Path.GetDirectoryName(downloadsDirectory));

                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(url, downloadsDirectory + fileName);
                }

                var unzippedDirectoryPath = downloadsDirectory + "unzipped/";

                ExtractZipFile(downloadsDirectory + fileName, unzippedDirectoryPath);

                var firstDirectoryPath = Directory.GetDirectories(unzippedDirectoryPath).First();

                var bookNameCamelCased = Path.GetFileName(firstDirectoryPath);
                var bookName = unCamelCase(bookNameCamelCased).Replace("-master", "");
                var renamedDirectory = firstDirectoryPath.Replace(bookNameCamelCased, "") + bookName;

                var finalDestination = string.Format("{0}/{1} ({2})", IOHelper.MapPath(Helpers.Constants.ROOT_DIRECTORY), bookName, DateTime.Now.ToString("yyyy-MM-dd"));

                Directory.Move(unzippedDirectoryPath + bookNameCamelCased, renamedDirectory);

                if (Directory.Exists(finalDestination))
                {
                    Directory.Delete(finalDestination, true);
                }

                Directory.Move(renamedDirectory, finalDestination);

                Directory.Delete(downloadsDirectory, true);

                return new
                {
                    Status = "Downloaded"
                };
            }

            catch (Exception ex)
            {
                LogHelper.Error<Exception>(ex.Message, ex);

                return new
                {
                    Status = "Error", Message = ex.Message
                };
            }
        }

        [HttpGet]
        public object GetBookFeed()
        {
            var request = WebRequest.Create(Helpers.Constants.FEED_URL);

            var response = request.GetResponse();

            var dataStream = response.GetResponseStream();

            var reader = new StreamReader(dataStream);

            var responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return JObject.Parse(responseFromServer);
        }

        private void ExtractZipFile(string archiveFilenameIn, string outFolder, string password = "")
        {
            ZipFile zf = null;
            try
            {
                var fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);

                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }

                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }

                    var entryFileName = zipEntry.Name;

                    var whiteListedExtensions = new List<string>() {".md", ".jpg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".csv"};

                    if (!whiteListedExtensions.Any(x => entryFileName.EndsWith(x)))
                    {
                        continue;
                    }

                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    var buffer = new byte[4096];     // 4K is optimum
                    var zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

        private String unCamelCase(String str)
        {
           return Regex.Replace( Regex.Replace( str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2" ), @"(\p{Ll})(\P{Ll})", "$1 $2" );
        }

        private string getSystemPath(string filePath, int skip = 2)
        {
            var filePathSections = WebUtility.UrlDecode(filePath).Split('/');

            return IOHelper.MapPath(Helpers.Constants.ROOT_DIRECTORY + "/" + string.Join("/", filePathSections.Skip(skip)));
        }
    }
} 
