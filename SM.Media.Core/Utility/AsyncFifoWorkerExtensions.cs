using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  public static class AsyncFifoWorkerExtensions
  {
    public static void Post(this AsyncFifoWorker worker, Action action, string description, CancellationToken cancellationToken)
    {
      worker.Post((Func<Task>) (() =>
      {
        action();
        return TplTaskExtensions.CompletedTask;
      }), description, cancellationToken);
    }

    public static void Post(this AsyncFifoWorker worker, Task work, string description, CancellationToken cancellationToken)
    {
      worker.Post((Func<Task>) (() => work), description, cancellationToken);
    }
  }
}
