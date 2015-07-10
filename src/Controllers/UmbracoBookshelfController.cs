using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.IO;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Examine;
using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using UmbracoBookshelf.Examine;
using UmbracoBookshelf.Models;
using UmbracoBookshelf.Helpers;
using Constants = UmbracoBookshelf.Helpers.Constants;

namespace UmbracoBookshelf.Controllers
{
    [PluginController("UmbracoBookshelfApi")]
    public class UmbracoBookshelfController : UmbracoAuthorizedJsonController
    {
        public object GetConfig()
        {
            return new
            {
                fileExtensions = Helpers.Constants.ALLOWED_FILE_EXTENSIONS,
                imageExtensions = Helpers.Constants.ALLOWED_IMAGE_EXTENSIONS,
                disableEditing = Helpers.Constants.DISABLE_EDITING
            };
        }

        public object GetFileContents(string filePath)
        {
            try
            {
                var systemFilePath = filePath.ToSystemPath();

                var extension = Path.GetExtension(systemFilePath);

                if (!extension.EndsWith(Helpers.Constants.MARKDOWN_FILE_EXTENSION))
                {
                    throw new WebException("Invalid file type");
                }

                var content = File.ReadAllText(systemFilePath);


                //remove blacklisted tags
                var blacklistedTags = new string[] {"script", "style"};

                var doc = new HtmlDocument();
                doc.LoadHtml(content);

                doc.DocumentNode.Descendants()
                                .Where(n => n.Name.ContainsAny(blacklistedTags))
                                .ToList()
                                .ForEach(n => n.Remove());

                return new
                {
                    Content = doc.DocumentNode.OuterHtml
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

        public object GetSiblingFiles(string filePath)
        {
            try
            {
                var fullFilePath = filePath.ToSystemPath();

                var extension = Path.GetExtension(fullFilePath);

                if (!extension.EndsWith(Helpers.Constants.MARKDOWN_FILE_EXTENSION))
                {
                    throw new WebException("Invalid file type");
                }

                var searchedFilename = Path.GetFileName(fullFilePath);

                var systemFilePath = Path.GetDirectoryName(fullFilePath);

                var files = Directory.GetFiles(systemFilePath);

                var indexOfSearchedFile = files.IndexOf(fullFilePath);

                //figure next file
                var nextFile = "";

                if (files.Length - 1 > indexOfSearchedFile)
                {
                    nextFile = Path.GetFileName(files[indexOfSearchedFile + 1]);

                    if (nextFile.ToLower() == "readme.md")
                    {
                        nextFile = "";
                    }
                }
                else
                {
                    if (searchedFilename.ToLower() == "readme.md")
                    {
                        nextFile = Path.GetFileName(files[0]);
                    }
                }

                //figure back file
                var backFile = "";

                if (indexOfSearchedFile == 0)
                {
                    backFile = "README.md";
                }
                else
                {
                    backFile = Path.GetFileName(files[indexOfSearchedFile - 1]);

                    if (searchedFilename.ToLower() == "readme.md")
                    {
                        backFile = "";
                    }
                }

                return new
                {
                    SearchedFile = searchedFilename,
                    Back = backFile,
                    Next = nextFile
                };
            }
            catch (Exception ex)
            {
                LogHelper.Error<Exception>(ex.Message, ex);

                return new
                {
                    Back = "",
                    Next = ""
                };
            }
        }

        public object GetFolderContents(string dirPath)
        {
            try
            {
               var systemFilePath = dirPath.ToSystemPath();

               var readme = Directory.GetFiles(systemFilePath)
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

        [System.Web.Http.HttpPost]
        public object SaveFile(FileSaveModel model)
        {
            _ensureDisableEditing();

            var systemFilePath = model.FilePath.ToSystemPath();

            if(!systemFilePath.EndsWith(Helpers.Constants.MARKDOWN_FILE_EXTENSION))
            {
                throw new WebException("Invalid file type");
            }

            File.WriteAllText(systemFilePath, model.Content);

            return new
            {
                Status = "Saved."
            };
        }

        [System.Web.Http.HttpPost]
        public object Delete(DeletePathModel model)
        {
            _ensureDisableEditing();

            var systemPath = ("/" + model.Path).ToSystemPath(1);

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

        [System.Web.Http.HttpPost]
        public object CreateFile(CreateFileModel model)
        {
            _ensureDisableEditing();

            model.FilePath = _ensureRootPath(model.FilePath);

            var systemPath = model.FilePath.ToSystemPath(1);

            File.WriteAllText(systemPath + ".md", @"#Overview#");

            return new
            {
                Status = "Created."
            };
        }

        [System.Web.Http.HttpPost]
        public object Rename(RenameModel model)
        {
            var systemPath = model.SourcePath.ToSystemPath(0);
            model.NewName = model.NewName.MakeSafe().Replace("\\", "");

            var newSystemPath = Path.GetDirectoryName(systemPath) + "\\" + model.NewName;

            if (!model.IsFolder)
            {
                newSystemPath += ".md";
            }

            Directory.Move(systemPath, newSystemPath);

            return new
            {
                Status = "Renamed."
            };
        }

        [System.Web.Http.HttpPost]
        public object CreateFolder(CreateFolderModel model)
        {
            _ensureDisableEditing();

            model.Path = _ensureRootPath(model.Path);

            var systemPath = model.Path.ToSystemPath(1);

            //create directory
            Directory.CreateDirectory(systemPath);

            //add file
            File.WriteAllText(systemPath + "/README.md", @"#Overview#");

            return new
            {
                Status = "Created."
            };
        }

        [System.Web.Http.HttpGet]
        public object DownloadUrl(string url)
        {
            try
            {
                var downloadsDirectory = IOHelper.MapPath(Constants.ROOT_DIRECTORY + "/TEMP/" + DateTime.Now.Ticks + "/");
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
                var bookName = _unCamelCase(bookNameCamelCased).Replace("-master", "");
                var renamedDirectory = firstDirectoryPath.Replace(bookNameCamelCased, "") + bookName;

                var finalDestination = string.Format("{0}/{1} ({2})", IOHelper.MapPath(Helpers.Constants.ROOT_DIRECTORY), bookName, DateTime.Now.ToString("yyyy-MM-dd"));

                Directory.Move(unzippedDirectoryPath + bookNameCamelCased, renamedDirectory);

                if (Directory.Exists(finalDestination))
                {
                    Directory.Delete(finalDestination, true);
                }

                Directory.Move(renamedDirectory, finalDestination);

                Directory.Delete(downloadsDirectory, true);

                ExamineManager.Instance.IndexProviderCollection["BookshelfIndexer"].RebuildIndex();

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

        [System.Web.Http.HttpGet]
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

        [System.Web.Http.HttpGet]
        public object GetImages(string currentPath)
        {
            //get current directory
            var systemPath = Path.GetDirectoryName(_ensureRootPath(currentPath).ToSystemPath());
            
            //search for any media files
            var imageFiles = systemPath.GetFilesRecursively(Constants.ALLOWED_IMAGE_EXTENSIONS);

            return imageFiles.Select(x => new ImageModel()
            {
                RelativePath = x.ToWebPath(true).Replace(systemPath.ToWebPath(true), "").Substring(1),
                Alt = Path.GetFileName(x),
                FilePath = x.ToWebPath(true)
            });
        }

        [System.Web.Http.HttpGet]
        public object SearchFiles(string keywords)
        {
            var results = BookshelfSearcher.Search(keywords);

            var data = new List<BookResultModel>();

            if (results.Any())
            {
                var books = results.GroupBy(x => x.Fields["book"]);

                var resultsPerBook = 10;

                foreach (var book in books)
                {
                    var bookData = new BookResultModel()
                    {
                        Name = book.Key
                    };

                    var resultsList = new List<BookResultModel.BookEntry>();

                    foreach(var result in book.OrderByDescending(x => x.Score).Take(resultsPerBook))
                    {
                        var hintUrl = HttpUtility.UrlDecode(HttpUtility.UrlDecode(result.Fields["url"]));
                        var title = HttpUtility.UrlDecode(HttpUtility.UrlDecode(result.Fields["title"]));

                        hintUrl = hintUrl.Substring(0, hintUrl.Length - title.Length - 3);

                        var hintWindowLength = 50;

                        if (hintUrl.Length - hintWindowLength > 0)
                        {
                            hintUrl = "…" + hintUrl.Substring(hintUrl.Length - hintWindowLength);
                        }

                        resultsList.Add(new BookResultModel.BookEntry()
                        {
                            Title = title,
                            Url = result.Fields["url"],
                            HintUrl = hintUrl,
                            Score = result.Score.ToString()
                        });

                        bookData.TotalScore += result.Score;
                    }

                    bookData.Results = resultsList;
                    data.Add(bookData);
                }
            }

            return data.OrderByDescending(x => x.TotalScore);
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

                    var whiteListedExtensions = new List<string>() { Helpers.Constants.MARKDOWN_FILE_EXTENSION };
                    whiteListedExtensions.AddRange(Helpers.Constants.ALLOWED_FILE_EXTENSIONS);
                    whiteListedExtensions.AddRange(Helpers.Constants.ALLOWED_IMAGE_EXTENSIONS);

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

        private string _unCamelCase(String str)
        {
           return Regex.Replace( Regex.Replace( str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2" ), @"(\p{Ll})(\P{Ll})", "$1 $2" );
        }

        private string _ensureRootPath(string path)
        {
            if (path.StartsWith("/-1"))
            {
                return "/" + path.Substring(3);
            }

            return path;
        }

        private void _ensureDisableEditing()
        {
            if (Helpers.Constants.DISABLE_EDITING)
            {
                throw new Exception("Editing is disabled."); 
            }
        }
    }
} 
