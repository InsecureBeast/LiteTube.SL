using System;

namespace LiteTube.Core.Common.Exceptions
{
    public class PlaylistNotFoundException : Exception
    {
        public PlaylistNotFoundException(string message) : base(message)
        {
        }

        public PlaylistNotFoundException(Exception e) : base(e.Message)
        {
        }

        public PlaylistNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
