using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core.Logging;
using Umbraco.Core.IO;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Microsoft.Web.Administration;
using Microsoft.Web.Management;

namespace UmbracoBookshelf.Controllers
{
    [PluginController("UmbracoBookshelf")]
    [Umbraco.Web.Trees.Tree("UmbracoBookshelf", "UmbracoBookshelfTree", "Umbraco Bookshelf", iconClosed: "icon-folder")]
    public class UmbracoBookshelfTreeController : FileSystemTreeController
    {
        protected override string FilePath
        {
            get { return "~" + Helpers.Constants.ROOT_DIRECTORY; }
        }

        protected override string FileSearchPattern
        {
            get { return "*" + Helpers.Constants.ALLOWED_FILE_EXTENSION; }
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            //menu.DefaultMenuAlias = ActionNew.Instance.Alias;
            //menu.Items.Add<ActionNew>("Create");
            menu.Items.Add<ActionDelete>("Delete");
            //menu.Items.Add<ActionMove>("Move");
            return menu;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            try
            {
                nodes.AddRange(getNodes(id, queryStrings, FilePath));

                foreach (var vdir in GetVirtualDirectories())
                {
                    nodes.AddRange(getNodes(id, queryStrings, vdir.Path));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error<Exception>(ex.Message, ex);
            }

            return nodes;
        }

        private string getWebPath(string mappedPath)
        {
            LogHelper.Info<TreeController>("mp=>" + mappedPath);

            var urlRoot = new Uri(IOHelper.MapPath("~") + "/");
            var path = urlRoot.MakeRelativeUri(new Uri(mappedPath)).ToString();
            
            LogHelper.Info<TreeController>("R URI=>" +path);

            //normal filesystem placement
            path = path.Replace(".." + Helpers.Constants.ROOT_DIRECTORY , "");

            //fixup virtual relative paths
            path = path.Replace("/..", "").Replace("..","");

            //both virtual and normal
            path = path.Replace("/", "%2F");

            LogHelper.Info<TreeController>("computed path=>" + path);

            return path;
        }

        public IEnumerable<VirtualDirectory> GetVirtualDirectories()
        {
            var manager = new ServerManager();
            var defaultSite = manager.Sites["mendozaapp.local"];

            return
                defaultSite.Applications.First()
                    .VirtualDirectories.Where(x => x.Path.Contains("UmbracoBookshelf"));
        }

        private TreeNodeCollection getNodes(string id, FormDataCollection queryStrings, string file_path)
        {
            LogHelper.Info<TreeNodeCollection>(file_path);

            var orgPath = "";
            var path = "";
            var _filePath = file_path;

            if (!string.IsNullOrEmpty(id) && id != "-1")
            {
                orgPath = id;
                path = IOHelper.MapPath(_filePath + "/" + orgPath);
                orgPath += "/";
            }
            else
            {
                path = IOHelper.MapPath(_filePath);
            }

            LogHelper.Info<TreeNodeCollection>("Mapped=>" + path);

            var dirInfo = new DirectoryInfo(path);
            var dirInfos = dirInfo.GetDirectories();

            LogHelper.Info<TreeNodeCollection>("# Dirs=>" + dirInfos.Count());

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
