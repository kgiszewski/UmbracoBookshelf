using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Umbraco.Core.IO;

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
    }
}