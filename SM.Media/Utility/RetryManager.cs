// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.RetryManager
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Utility
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
