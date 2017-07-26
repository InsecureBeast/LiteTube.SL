using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Utility;

namespace SM.Media.Core
{
    public abstract class PlaybackSessionBase<TMediaSource> : IDisposable where TMediaSource : class
    {
        private readonly int _id = Interlocked.Increment(ref PlaybackSessionBase<TMediaSource>._idCount);
        private readonly TaskCompletionSource<TMediaSource> _mediaSourceTaskCompletionSource = new TaskCompletionSource<TMediaSource>();
        private readonly CancellationTokenSource _playingCancellationTokenSource = new CancellationTokenSource();
        private static int _idCount;
        private readonly IMediaStreamFacadeBase<TMediaSource> _mediaStreamFacade;
        private int _isDisposed;

        public ContentType ContentType { get; set; }

        protected IMediaStreamFacadeBase<TMediaSource> MediaStreamFacade
        {
            get
            {
                return this._mediaStreamFacade;
            }
        }

        protected PlaybackSessionBase(IMediaStreamFacadeBase<TMediaSource> mediaStreamFacade)
        {
            if (null == mediaStreamFacade)
                throw new ArgumentNullException("mediaStreamFacade");
            this._mediaStreamFacade = mediaStreamFacade;
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
                return;
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            this._mediaSourceTaskCompletionSource.TrySetCanceled();
            this._playingCancellationTokenSource.CancelDisposeSafe();
        }

        public virtual Task PlayAsync(Uri source, CancellationToken cancellationToken)
        {
            Debug.WriteLine("PlaybackSessionBase.PlayAsync() " + (object)this);
            Task task = this.PlayerAsync(source, cancellationToken);
            TaskCollector.Default.Add(task, "StreamingMediaPlugin PlayerAsync");
            return this.MediaStreamFacade.PlayingTask;
        }

        private async Task PlayerAsync(Uri source, CancellationToken cancellationToken)
        {
            try
            {
                using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._playingCancellationTokenSource.Token, cancellationToken))
                {
                    this.MediaStreamFacade.ContentType = this.ContentType;
                    TMediaSource mss = await this.MediaStreamFacade.CreateMediaStreamSourceAsync(source, linkedTokenSource.Token).ConfigureAwait(false);
                    if (!this._mediaSourceTaskCompletionSource.TrySetResult(mss))
                        throw new OperationCanceledException();
                    goto label_15;
                }
            }
            catch (OperationCanceledException ex)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlaybackSessionBase.PlayerAsync() failed: " + ExceptionExtensions.ExtendedMessage(ex));
            }
            try
            {
                this._mediaSourceTaskCompletionSource.TrySetCanceled();
                await this.MediaStreamFacade.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlaybackSessionBase.PlayerAsync() cleanup failed: " + ExceptionExtensions.ExtendedMessage(ex));
            }
            label_15:;
        }

        public virtual async Task<TMediaSource> GetMediaSourceAsync(CancellationToken cancellationToken)
        {
            return await TplTaskExtensions.WithCancellation<TMediaSource>(this._mediaSourceTaskCompletionSource.Task, cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            ConfiguredTaskAwaitable configuredTaskAwaitable = this.MediaStreamFacade.StopAsync(cancellationToken).ConfigureAwait(false);
            await configuredTaskAwaitable;
            configuredTaskAwaitable = this.MediaStreamFacade.PlayingTask.ConfigureAwait(false);
            await configuredTaskAwaitable;
        }

        public virtual async Task CloseAsync()
        {
            if (!_playingCancellationTokenSource.IsCancellationRequested)
                _playingCancellationTokenSource.Cancel();

            await MediaStreamFacade.PlayingTask;
        }

        public virtual async Task OnMediaFailedAsync()
        {
            Debug.WriteLine("PlaybackSessionBase.OnMediaFailedAsync() " + (object)this);
            try
            {
                await this.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlaybackSessionBase.OnMediaFailedAsync() CloseAsync() failed: " + ex.Message);
            }
        }

        public override string ToString()
        {
            return string.Format("Playback ID {0} IsCompleted {1}", new object[2]
            {
                (object) this._id,
                (object) (bool) (this.MediaStreamFacade.PlayingTask.IsCompleted ? true : false)
            });
        }
    }
}
