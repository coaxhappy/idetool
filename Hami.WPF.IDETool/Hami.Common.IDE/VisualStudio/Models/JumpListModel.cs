using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hami.Common.IDE.VisualStudio.Models
{
    public class JumpListModel
    {
        public string SourceFileName { get; set; }

        public bool IsPinned { get; set; }

        public int MRUPosition { get; set; }
    }
}
