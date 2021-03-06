﻿using System;

namespace LiteTube.StreamVideo.Utility
{
  public static class RetryManagerExtensions
  {
    public static IRetry CreateWebRetry(this IRetryManager retryManager, int maxRetries, int delayMilliseconds)
    {
      return retryManager.CreateRetry(maxRetries, delayMilliseconds, new Func<Exception, bool>(RetryPolicy.IsWebExceptionRetryable));
    }
  }
}
