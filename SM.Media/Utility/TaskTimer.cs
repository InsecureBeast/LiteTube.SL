// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.TaskTimer
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Diagnostics;
using System.Threading;

namespace SM.Media.Utility
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
