using System;

namespace SM.Media.Core.Utility
{
  public interface IRetryManager
  {
    IRetry CreateRetry(int maxRetries, int delayMilliseconds, Func<Exception, bool> retryableException);
  }
}
