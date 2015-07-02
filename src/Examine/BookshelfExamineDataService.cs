using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Examine;
using Examine.LuceneEngine;
using Umbraco.Core.Logging;
using UmbracoBookshelf.Helpers;

namespace UmbracoBookshelf.Examine
{
    public class BookshelfExamineDataService : ISimpleDataService
    {
        public IEnumerable<SimpleDataSet> GetAllData(string indexType)
        {
            LogHelper.Info<BookshelfExamineDataService>("Building the Bookshelf index...");
            
            var data = new List<SimpleDataSet>();
            var count = 1;

            foreach (var bookPath in _getBooksAsDirectories())
            {
                LogHelper.Info<ISimpleDataService>("Processing Book... " + bookPath);

                foreach (var file in bookPath.GetFilesRecursively(false))
                {
                    LogHelper.Info<ISimpleDataService>("Processing... " + file);

                    var dataset = new SimpleDataSet()
                    {
                        NodeDefinition = new IndexedNode()
                        {
                            Type = "Bookshelf",
                            NodeId = count
                        }
                    };

                    dataset.RowData = new Dictionary<string, string>()
                    {
                        {"book", Path.GetFileName(bookPath)},
                        {"title", Path.GetFileNameWithoutExtension(file)},
                        {"text", File.ReadAllText(file)},
                        {"url", "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/file/" + file.ToWebPath().Replace("%2F", "%252F").Replace("%20F", "%2520F")} //total hack job here b/c of some sort of double encoding somewhere
                    };

                    data.Add(dataset);
                    count++;

                    LogHelper.Info<string>("Added=>" + Path.GetFileName(file));
                }
            }

            return data;
        }

        private IEnumerable<string> _getBooksAsDirectories()
        {
            var systemPath = Constants.ROOT_DIRECTORY.ToSystemPath();

            return Directory.GetDirectories(systemPath).Where(x => !(x.EndsWith("TEMP")));
        }
    }
}