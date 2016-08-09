using System;

namespace LiteTube.Core.Common.Exceptions
{
    public class LiteTubeException : Exception
    {
        public LiteTubeException(string message) : base(message)
        {
        }

        public LiteTubeException(Exception e) : base(e.Message)
        {
        }

        public LiteTubeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
