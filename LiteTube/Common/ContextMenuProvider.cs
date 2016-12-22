using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.Common
{
    public interface IContextMenuProvider
    {
        bool CanDelete { get; }
        bool CanAddToPlayList { get; }
    }

    class ContextMenuProvider : IContextMenuProvider
    {
        public bool CanAddToPlayList
        {
            get; set;
        }

        public bool CanDelete
        {
            get; set;
        }
    }
}
