using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.Common
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
