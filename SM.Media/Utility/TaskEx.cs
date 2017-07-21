// Decompiled with JetBrains decompiler
// Type: SM.Media.TaskEx
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media
{
  public static class TaskEx
  {
    public static Task Run(Action action)
    {
      return Task.Run(action);
    }

    public static Task Run(Action action, CancellationToken cancellationToken)
    {
      return Task.Run(action, cancellationToken);
    }

    public static Task Run(Func<Task> function)
    {
      return Task.Run(function);
    }

    public static Task Run(Func<Task> function, CancellationToken cancellationToken)
    {
      return Task.Run(function, cancellationToken);
    }

    public static Task Delay(int millisecondDelay)
    {
      return Task.Delay(millisecondDelay);
    }

    public static Task Delay(int millisecondDelay, CancellationToken cancellationToken)
    {
      return Task.Delay(millisecondDelay, cancellationToken);
    }

    public static Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
      return Task.Delay(delay, cancellationToken);
    }

    public static Task WhenAll(IEnumerable<Task> tasks)
    {
      return Task.WhenAll(tasks);
    }

    public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
    {
      return Task.WhenAll<TResult>(tasks);
    }

    public static Task WhenAll(params Task[] tasks)
    {
      return Task.WhenAll(tasks);
    }

    public static Task<Task> WhenAny(params Task[] tasks)
    {
      return Task.WhenAny(tasks);
    }

    public static Task<TResult> FromResult<TResult>(TResult result)
    {
      return Task.FromResult<TResult>(result);
    }
  }
}
