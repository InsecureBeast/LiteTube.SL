using System;
using System.Collections.Generic;

namespace LiteTube.StreamVideo.Utility
{
  public class RetryException : AggregateException
  {
    public RetryException(string message, IEnumerable<Exception> exceptions)
      : base(message, exceptions)
    {
    }
  }
}
