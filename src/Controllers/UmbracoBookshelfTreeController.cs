using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Configuration;
using System.Web.Configuration;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core.Logging;
using Umbraco.Core.IO;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace UmbracoBookshelf.Controllers
{
    [PluginController("UmbracoBookshelf")]
    [Umbraco.Web.Trees.Tree("UmbracoBookshelf", "UmbracoBookshelfTree", "Umbraco Bookshelf", iconClosed: "icon-folder")]
    public class UmbracoBookshelfTreeController : FileSystemTreeController
    {
        protected override string FilePath
        {
            get { return Helpers.Constants.ROOT_DIRECTORY; }
        }

        protected override string FileSearchPattern
        {
            get { return "*" + Helpers.Constants.ALLOWED_FILE_EXTENSION; }
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            menu.Items.Add<ActionDelete>("Delete");

            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            nodes.AddRange(getNodes(id, queryStrings));

            return nodes;
        }

        private string getWebPath(string mappedPath)
        {
            var urlRoot = new Uri(IOHelper.MapPath("~") + "/");
            var path = urlRoot.MakeRelativeUri(new Uri(mappedPath)).ToString();        

            //normal filesystem placement
            path = path.Replace(".." + Helpers.Constants.ROOT_DIRECTORY , "");

            //fixup virtual relative paths
            path = path.Replace("/..", "").Replace("..","");

            //both virtual and normal
            path = path.Replace("/", "%2F");

            return path;
        }

        private TreeNodeCollection getNodes(string id, FormDataCollection queryStrings)
        {
            var orgPath = "";
            var path = "";

            if (!string.IsNullOrEmpty(id) && id != "-1")
            {
                orgPath = id;
                path = IOHelper.MapPath(FilePath + "/" + orgPath);
                orgPath += "/";
            }
            else
            {
                path = IOHelper.MapPath(FilePath);
            }

            var dirInfo = new DirectoryInfo(path);
            var dirInfos = dirInfo.GetDirectories();

            var nodes = new TreeNodeCollection();

            foreach (var dir in dirInfos)
            {
                var hasChildren = dir.GetFiles().Any(x => x.Name.EndsWith(Helpers.Constants.ALLOWED_FILE_EXTENSION)) || dir.GetDirectories().Length > 0;

                if (hasChildren && (dir.Attributes & FileAttributes.Hidden) == 0)
                {
                    var node = CreateTreeNode(orgPath + dir.Name, orgPath, queryStrings, dir.Name, "icon-folder", hasChildren);

                    node.RoutePath = "/UmbracoBookshelf/UmbracoBookshelfTree/folder/" + getWebPath(dir.FullName);

                    if (node != null)
                        nodes.Add(node);
                }
            }

            //this is a hack to enable file system tree to support multiple file extension look-up
            //so the pattern both support *.* *.xml and xml,js,vb for lookups
            var allowedExtensions = new string[0];
            var filterByMultipleExtensions = FileSearchPattern.Contains(",");
            FileInfo[] fileInfo;

            if (filterByMultipleExtensions)
            {
                fileInfo = dirInfo.GetFiles();
                allowedExtensions = FileSearchPattern.ToLower().Split(',');
            }
            else
                fileInfo = dirInfo.GetFiles(FileSearchPattern);

            foreach (var file in fileInfo)
            {
                if ((file.Attributes & FileAttributes.Hidden) == 0)
                {
                    if (filterByMultipleExtensions && Array.IndexOf<string>(allowedExtensions, file.Extension.ToLower().Trim('.')) < 0)
                        continue;

                    var node = CreateTreeNode(orgPath + file.Name, orgPath, queryStrings, file.Name, "icon-document", false);

                    node.RoutePath = "/UmbracoBookshelf/UmbracoBookshelfTree/file/" + getWebPath(file.FullName);

                    if (node != null)
                        nodes.Add(node);
                }
            }

            return nodes;
        }
    }
}
