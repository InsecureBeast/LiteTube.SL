﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.SingleUseTaskTimer
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
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
