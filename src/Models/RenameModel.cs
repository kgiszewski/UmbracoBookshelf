using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UmbracoBookshelf.Models
{
    public class RenameModel
    {
        public string SourcePath { get; set; }
        public string NewName { get; set; }
        public bool IsFolder { get; set; }
    }
}