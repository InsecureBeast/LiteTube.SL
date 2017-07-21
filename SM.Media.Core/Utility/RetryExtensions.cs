using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public static class RetryExtensions
  {
    public static Task CallAsync(this IRetry retry, Func<Task> operation, CancellationToken cancellationToken)
    {
      return (Task) retry.CallAsync<int>((Func<Task<int>>) (async () =>
      {
        await operation().ConfigureAwait(false);
        return 0;
      }), cancellationToken);
    }
  }
}
