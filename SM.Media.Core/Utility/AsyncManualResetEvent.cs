using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Utility
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
