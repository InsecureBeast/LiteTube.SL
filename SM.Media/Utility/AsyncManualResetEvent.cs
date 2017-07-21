// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.AsyncManualResetEvent
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
{
  public sealed class AsyncManualResetEvent
  {
    private volatile TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

    public AsyncManualResetEvent(bool initialState = false)
    {
      if (!initialState)
        return;
      this.Set();
    }

    public Task WaitAsync()
    {
      return (Task) this._tcs.Task;
    }

    public void Set()
    {
      this._tcs.TrySetResult(true);
    }

    public void Reset()
    {
      TaskCompletionSource<bool> comparand = this._tcs;
      if (!comparand.Task.IsCompleted)
        return;
      TaskCompletionSource<bool> completionSource1 = new TaskCompletionSource<bool>();
      do
      {
        TaskCompletionSource<bool> completionSource2 = Interlocked.CompareExchange<TaskCompletionSource<bool>>(ref this._tcs, completionSource1, comparand);
        if (comparand == completionSource2 && !comparand.Task.IsCompleted)
        {
          Debug.WriteLine("*** AsyncManualResetEvent.Reset(): task completion source was not completed");
          comparand.TrySetResult(true);
        }
        comparand = completionSource2;
      }
      while (comparand.Task.IsCompleted);
    }
  }
}
