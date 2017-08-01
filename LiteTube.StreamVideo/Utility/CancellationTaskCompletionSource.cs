using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public sealed class CancellationTaskCompletionSource<TItem> : TaskCompletionSource<TItem>
  {
    private readonly Action<CancellationTaskCompletionSource<TItem>> _cancellationAction;
    private CancellationTokenRegistration _cancellationTokenRegistration;
    private int _isDisposed;

    public CancellationTaskCompletionSource(Action<CancellationTaskCompletionSource<TItem>> cancellationAction, CancellationToken cancellationToken)
    {
      if (null == cancellationAction)
        throw new ArgumentNullException("cancellationAction");
      this._cancellationAction = cancellationAction;
      this._cancellationTokenRegistration = cancellationToken.Register((Action<object>) (obj => ((CancellationTaskCompletionSource<TItem>) obj).Cancel()), (object) this);
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      DisposeExtensions.DisposeSafe((IDisposable) this._cancellationTokenRegistration);
      this.TrySetCanceled();
    }

    private void Cancel()
    {
      this._cancellationAction(this);
      this.Dispose();
    }
  }
}
