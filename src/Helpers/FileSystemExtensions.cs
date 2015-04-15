using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Web;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace UmbracoBookshelf.Helpers
{
    public static class FileSystemExtensions
    {
        public static string ToSystemPath(this string filePath, int skip = 2)
        {
            //prevent relative system path

            var filePathSections = WebUtility.UrlDecode(filePath).MakeSafe().Split('/');

            return IOHelper.MapPath(Helpers.Constants.ROOT_DIRECTORY + "/" + string.Join("/", filePathSections.Skip(skip)));
        }

        public static string MakeSafe(this string input)
        {
            return input.Replace("../", "").Replace("..\\", "").Replace(":", "").Replace(@"\\", @"\");
        }

        public static string ToWebPath(this string mappedPath, bool preserveSlashes = false)
        {
            var urlRoot = new Uri(IOHelper.MapPath("~") + "/");
            var path = urlRoot.MakeRelativeUri(new Uri(mappedPath)).ToString();

            //normal filesystem placement
            path = path.Replace(".." + Helpers.Constants.ROOT_DIRECTORY, "");

            //fixup virtual relative paths
            path = path.Replace("/..", "").Replace("..", "");

            if (preserveSlashes)
            {
                return HttpUtility.UrlDecode(path);
            }

            //both virtual and normal
            path = path.Replace("/", "%2F");

            return path;
        }

        public static IEnumerable<string> GetFilesRecursively(this string startingDirectory)
        {
            var list = new List<string>();

            try
            {
                foreach (string dir in Directory.GetDirectories(startingDirectory))
                {
                    foreach (string file in Directory.GetFiles(dir))
                    {
                        if (Constants.ALLOWED_IMAGE_EXTENSIONS.Any(file.EndsWith))
                        {
                            list.Add(file);
                        }
                    }

                    GetFilesRecursively(dir);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<Exception>(ex.Message, ex);
            }

            return list;
        }
    }
}