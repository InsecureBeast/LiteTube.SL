using System;

namespace LiteTube.StreamVideo.Utility
{
  public interface IRetryManager
  {
    IRetry CreateRetry(int maxRetries, int delayMilliseconds, Func<Exception, bool> retryableException);
  }
}
