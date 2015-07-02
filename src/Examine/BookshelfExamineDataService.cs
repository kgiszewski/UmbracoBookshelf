using System.Collections.Generic;
using Examine;
using Examine.LuceneEngine;
using Umbraco.Core.Logging;

namespace UmbracoBookshelf.Examine
{
    public class BookshelfExamineDataService : ISimpleDataService
    {
        public IEnumerable<SimpleDataSet> GetAllData(string indexType)
        {
            LogHelper.Info<BookshelfExamineDataService>("Building the Bookshelf index...");
            
            var data = new List<SimpleDataSet>();

            data.Add(new SimpleDataSet()
            {
                NodeDefinition = new IndexedNode()
                {
                    Type = "Bookshelf",
                    NodeId = 1
                },
                RowData = new Dictionary<string, string>()
                {
                    {"book", "Learn Umbraco 7"},
                    {"title", "Developer Tools"},
                    {"text", "lorem ipsum"},
                    {"url", "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/file/%252FBooks%252FLearnUmbraco7%252FChapter%25200%252F01%2520-%2520Developer%2520Tools.md"}
                }
            });

            data.Add(new SimpleDataSet()
            {
                NodeDefinition = new IndexedNode()
                {
                    Type = "Bookshelf",
                    NodeId = 2
                },
                RowData = new Dictionary<string, string>()
                {
                    {"book", "Learn Umbraco 7"},
                    {"title", "Orientation"},
                    {"text", "foo bar"},
                    {"url", "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/folder/%252FBooks%252FLearnUmbraco7%252FChapter%252002%2520-%2520Backoffice%2520Orientation"}
                }
            });

            return data;
        }
    }
}