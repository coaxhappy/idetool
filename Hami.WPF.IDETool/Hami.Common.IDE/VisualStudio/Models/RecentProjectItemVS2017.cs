using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hami.Common.IDE.VisualStudio.Models
{
    public class RecentProjectItemVS2017
    {
        public string Key { get; set; }

        public RecentProjectItemDataVS2017 Value { get; set; }
    }

    public class RecentProjectItemDataVS2017
    {
        public RecentProjectItemDataLocalProperty LocalProperties { get; set; }

        public object Remote { get; set; }

        public bool IsFavorite { get; set; }

        public string LastAccessed { get; set; }

        public bool IsLocal { get; set; }

        public bool HasRemote { get; set; }

        public bool IsSourceControlled { get; set; }
    }

    public class RecentProjectItemDataLocalProperty
    {
        public string FullPath { get; set; }

        public int Type { get; set; }

        public object SourceControl { get; set; }
    }
         
}
