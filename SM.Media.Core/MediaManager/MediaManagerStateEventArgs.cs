using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.MediaManager
{
    public class MediaManagerStateEventArgs : EventArgs
    {
        public readonly string Message;
        public readonly MediaManagerState State;

        public MediaManagerStateEventArgs(MediaManagerState state, string message = null)
        {
            this.State = state;
            this.Message = message;
        }
    }
}
