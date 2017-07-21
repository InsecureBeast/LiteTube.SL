// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.TplTaskExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
{
  public static class TplTaskExtensions
  {
    public static readonly Task NeverCompletedTask = (Task) new TaskCompletionSource<object>().Task;
    public static readonly Task<bool> TrueTask = TaskEx.FromResult<bool>(true);
    public static readonly Task<bool> FalseTask = TaskEx.FromResult<bool>(false);
    public static readonly Task CompletedTask = (Task) TplTaskExtensions.TrueTask;

    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
      TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
      using (cancellationToken.Register((Action<object>) (s => ((TaskCompletionSource<bool>) s).TrySetResult(true)), (object) tcs, false))
      {
        if ((Task<T>) task != await TaskEx.WhenAny((Task) task, (Task) tcs.Task).ConfigureAwait(false))
          throw new OperationCanceledException(cancellationToken);
      }
      return await task.ConfigureAwait(false);
    }

    public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
      TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
      using (cancellationToken.Register((Action<object>) (s => ((TaskCompletionSource<bool>) s).TrySetResult(true)), (object) tcs, false))
      {
        if (task != await TaskEx.WhenAny(task, (Task) tcs.Task).ConfigureAwait(false))
          throw new OperationCanceledException(cancellationToken);
      }
      await task.ConfigureAwait(false);
    }
  }
}
