using System;
using System.Collections.Generic;
using System.Configuration;

namespace UmbracoBookshelf.Helpers
{
    public class Constants
    {
        public static string ROOT_DIRECTORY = ConfigurationManager.AppSettings["UmbracoBookshelf:customFolder"] ?? "~/UmbracoBookshelf";

        public static bool DISABLE_EDITING = Convert.ToBoolean(ConfigurationManager.AppSettings["UmbracoBookshelf:disableEditing"] ?? "false");

        public const string MARKDOWN_FILE_EXTENSION = ".md";

        public static List<string> ALLOWED_FILE_EXTENSIONS =
            new List<string>() { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".csv"};

        public static List<string> ALLOWED_IMAGE_EXTENSIONS =
            new List<string>() { ".jpg", ".png", ".gif" };

        public const string FOLDER_FILE = "README.md";

        public const string FEED_URL = "https://raw.githubusercontent.com/kgiszewski/UmbracoBookshelf/master/src/Feed/main.js";
    }
}
