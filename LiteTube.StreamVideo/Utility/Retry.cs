using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public class Retry : IRetry
  {
    private static readonly IEnumerable<Exception> NoExceptions = (IEnumerable<Exception>) new Exception[0];
    private readonly int _delayMilliseconds;
    private readonly int _maxRetries;
    private readonly IPlatformServices _platformServices;
    private readonly Func<Exception, bool> _retryableException;
    private int _delay;
    private List<Exception> _exceptions;
    private int _retry;

    public Retry(int maxRetries, int delayMilliseconds, Func<Exception, bool> retryableException, IPlatformServices platformServices)
    {
      if (maxRetries < 1)
        throw new ArgumentOutOfRangeException("maxRetries", "The number of retries must be positive.");
      if (delayMilliseconds < 0)
        throw new ArgumentOutOfRangeException("delayMilliseconds", "The delay cannot be negative");
      if (null == retryableException)
        throw new ArgumentNullException("retryableException");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._maxRetries = maxRetries;
      this._delayMilliseconds = delayMilliseconds;
      this._retryableException = retryableException;
      this._platformServices = platformServices;
      this._retry = 0;
      this._delay = 0;
    }

    public async Task<TResult> CallAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
    {
      this._retry = 0;
      this._delay = this._delayMilliseconds;
      if (null != this._exceptions)
        this._exceptions.Clear();
      TResult result;
      while (true)
      {
        try
        {
          result = await operation().ConfigureAwait(false);
          break;
        }
        catch (OperationCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          if (!this._retryableException(ex))
          {
            throw;
          }
          else
          {
            if (null == this._exceptions)
              this._exceptions = new List<Exception>();
            this._exceptions.Add(ex);
            if (++this._retry >= this._maxRetries)
              throw new RetryException("Giving up after " + (object) this._retry + " retries", (IEnumerable<Exception>) this._exceptions);
            Debug.WriteLine("Retry {0} after: {1}", (object) this._retry, (object) ex.Message);
          }
        }
        await this.DelayAsync(cancellationToken).ConfigureAwait(false);
      }
      return result;
    }

    public async Task<bool> CanRetryAfterDelayAsync(CancellationToken cancellationToken)
    {
      bool flag;
      if (this._retry >= this._maxRetries)
      {
        flag = false;
      }
      else
      {
        ++this._retry;
        await this.DelayAsync(cancellationToken).ConfigureAwait(false);
        flag = true;
      }
      return flag;
    }

    private async Task DelayAsync(CancellationToken cancellationToken)
    {
      int actualDelay = (int) ((double) this._delay * (0.5 + this._platformServices.GetRandomNumber()));
      this._delay += this._delay;
      await TaskEx.Delay(actualDelay, cancellationToken).ConfigureAwait(false);
    }
  }
}
