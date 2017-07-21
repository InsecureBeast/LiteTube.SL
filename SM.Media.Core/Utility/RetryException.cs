using System;
using System.Collections.Generic;

namespace SM.Media.Core.Utility
{
  public class RetryException : AggregateException
  {
    public RetryException(string message, IEnumerable<Exception> exceptions)
      : base(message, exceptions)
    {
    }
  }
}
