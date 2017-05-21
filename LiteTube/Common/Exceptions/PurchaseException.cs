using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.Common.Exceptions
{
    class PurchaseException : Exception
    {
        public PurchaseException(string message) : base(message)
        {
        }

        public PurchaseException(Exception e) : base(e.Message)
        {
        }

        public PurchaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
