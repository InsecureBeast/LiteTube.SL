using System;

namespace SM.Media.Core.Utility
{
  public class RetryManager : IRetryManager
  {
    private readonly IPlatformServices _platformServices;

    public RetryManager(IPlatformServices platformServices)
    {
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._platformServices = platformServices;
    }

    public IRetry CreateRetry(int maxRetries, int delayMilliseconds, Func<Exception, bool> retryableException)
    {
      return (IRetry) new Retry(maxRetries, delayMilliseconds, retryableException, this._platformServices);
    }
  }
}
