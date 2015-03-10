using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoBookshelf.Helpers
{
    public class Constants
    {
        public static string ROOT_DIRECTORY = ConfigurationManager.AppSettings["UmbracoBookshelf:customFolder"] ??
                                              "~/UmbracoBookshelf";

        public const string MARKDOWN_FILE_EXTENSION = ".md";

        public static List<string> MEDIA_FILE_EXTENSIONS =
            new List<string>() {".jpg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".csv"};

        public const string FOLDER_FILE = "README.md";

        public const string FEED_URL = "https://raw.githubusercontent.com/kgiszewski/UmbracoBookshelf/master/src/Feed/main.js";
    }
}
