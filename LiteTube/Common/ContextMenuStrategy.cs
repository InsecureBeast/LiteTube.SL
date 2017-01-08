using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.Common
{
    public interface IContextMenuStrategy
    {
        bool CanDelete { get; }
        bool CanAddToPlayList { get; }
    }

    class ContextMenuStartegy : IContextMenuStrategy
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

    class NoContextMenuStrategy : IContextMenuStrategy
    {
        public NoContextMenuStrategy()
        {
            CanAddToPlayList = false;
            CanDelete = false;
        }

        public bool CanAddToPlayList
        {
            get; private set;
        }

        public bool CanDelete
        {
            get; private set;
        }
    }
}
