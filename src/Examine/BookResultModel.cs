using System.Collections.Generic;

namespace UmbracoBookshelf.Examine
{
    public class BookResultModel
    {
        public string Name { get; set; }
        public IEnumerable<BookEntry> Results { get; set; }

        public class BookEntry
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
    }
}