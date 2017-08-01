using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
    public static class RetryExtensions
    {
        public static Task CallAsync(this IRetry retry, Func<Task> operation, CancellationToken cancellationToken)
        {
            return retry.CallAsync(async () =>
            {
                await operation().ConfigureAwait(false);
                return 0;
            }, cancellationToken);
        }
    }
}
