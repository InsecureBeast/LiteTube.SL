// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.CancellationTaskCompletionSource`1
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
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
