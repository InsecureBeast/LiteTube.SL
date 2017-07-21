﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.AsyncFifoWorkerExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
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