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
using System.Net;
using ClientDependency.Core;
using Umbraco.Core.Logging;
using UmbracoBookshelf.Models;

namespace UmbracoBookshelf.Controllers
{
    [PluginController("UmbracoBookshelfApi")]
    public class UmbracoBookshelfController : UmbracoAuthorizedJsonController
    {
        public object GetFileContents(string filePath)
        {
            var systemFilePath = IOHelper.MapPath(WebUtility.UrlDecode(Helpers.Constants.ROOT_DIRECTORY + filePath));

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

        public object GetFolderContents(string dirPath)
        {
            var systemFilePath = IOHelper.MapPath(WebUtility.UrlDecode(Helpers.Constants.ROOT_DIRECTORY + dirPath));

            var readme = Directory.GetFiles(systemFilePath).FirstOrDefault(x => Path.GetFileName(x) == Helpers.Constants.FOLDER_FILE);

            if (readme == null)
            {
                return new
                {
                    Content = ""
                };
            }

            var content = File.ReadAllText(readme);

            return new
            {
                Content = content
            };
        }

        [HttpPost]
        public object SaveFile(FileSaveModel model)
        {
            var systemFilePath = IOHelper.MapPath("~" + Helpers.Constants.ROOT_DIRECTORY + model.FilePath);

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
    }
}
