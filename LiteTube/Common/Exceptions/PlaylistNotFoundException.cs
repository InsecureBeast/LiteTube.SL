using System;

namespace LiteTube.Common.Exceptions
{
    class PlaylistNotFoundException : Exception
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
