using System;
using System.Collections.Generic;
using Examine;

namespace UmbracoBookshelf.Examine
{
    public static class BookshelfSearcher
    {
        public static ISearchResults Search(string keywords)
        {
            var searcher = ExamineManager.Instance.SearchProviderCollection["BookshelfSearcher"];
            var searchCriteria = searcher.CreateSearchCriteria(global::Examine.SearchCriteria.BooleanOperation.Or);
            var rawQueries = new List<string>();

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                var words = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    var contentRawQuery = string.Format("(+__IndexType:bookshelf && (title: {0}~{1} path: {0}~{1} book: {0}~{1} text: {0}~{1}))", word, 0.5);
                    rawQueries.Add(contentRawQuery);
                }
            }

            var query = searchCriteria.RawQuery("(" + String.Join(")(", rawQueries) + ")");

            return searcher.Search(query);
        }
    }
}