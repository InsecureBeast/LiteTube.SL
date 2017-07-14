using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.Common.Exceptions
{
    class ChannelNotFoundException : Exception
    {
        public ChannelNotFoundException(string message) : base(message)
        {
        }

        public ChannelNotFoundException(Exception e) : base(e.Message)
        {
        }

        public ChannelNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
