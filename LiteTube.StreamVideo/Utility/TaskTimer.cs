using System;
using System.Diagnostics;
using System.Threading;

namespace LiteTube.StreamVideo.Utility
{
  public sealed class TaskTimer : IDisposable
  {
    private SingleUseTaskTimer _timer;

    public void Dispose()
    {
      this.Cancel();
    }

    public void SetTimer(Action callback, TimeSpan expiration)
    {
      this.SetTimer(new SingleUseTaskTimer(callback, expiration));
    }

    public void Cancel()
    {
      this.SetTimer((SingleUseTaskTimer) null);
    }

    private void SetTimer(SingleUseTaskTimer timer)
    {
      timer = Interlocked.Exchange<SingleUseTaskTimer>(ref this._timer, timer);
      if (null == timer)
        return;
      TaskTimer.CleanupTimer(timer);
    }

    private static void CleanupTimer(SingleUseTaskTimer timer)
    {
      try
      {
        timer.Cancel();
        timer.Dispose();
      }
      catch (Exception ex)
      {
        Debug.WriteLine("Timer.Dispose(): " + ex.Message);
      }
    }
  }
}
