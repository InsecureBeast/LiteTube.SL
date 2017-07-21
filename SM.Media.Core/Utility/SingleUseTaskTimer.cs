using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
{
  internal sealed class SingleUseTaskTimer : CancellationTokenSource, IDisposable
  {
    public SingleUseTaskTimer(Action callback, TimeSpan expiration)
    {
      Task.Delay(expiration, this.Token).ContinueWith((Action<Task, object>) ((t, s) => ((Action) s)()), (object) callback, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    public new void Dispose()
    {
      this.Cancel();
    }
  }
}
