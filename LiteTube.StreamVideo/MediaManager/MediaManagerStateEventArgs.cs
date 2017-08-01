using System;

namespace LiteTube.StreamVideo.MediaManager
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
