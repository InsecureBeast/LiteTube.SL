using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public interface IRetry
  {
    Task<TResult> CallAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken);

    Task<bool> CanRetryAfterDelayAsync(CancellationToken cancellationToken);
  }
}
