using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo
{
    public abstract class PlaybackSessionBase<TMediaSource> : IDisposable where TMediaSource : class
    {
        private readonly int _id = Interlocked.Increment(ref _idCount);
        private readonly TaskCompletionSource<TMediaSource> _mediaSourceTaskCompletionSource = new TaskCompletionSource<TMediaSource>();
        private readonly CancellationTokenSource _playingCancellationTokenSource = new CancellationTokenSource();
        private static int _idCount;
        private int _isDisposed;

        public ContentType ContentType { get; set; }

        protected IMediaStreamFacadeBase<TMediaSource> MediaStreamFacade { get; }

        protected PlaybackSessionBase(IMediaStreamFacadeBase<TMediaSource> mediaStreamFacade)
        {
            if (null == mediaStreamFacade)
                throw new ArgumentNullException(nameof(mediaStreamFacade));
            MediaStreamFacade = mediaStreamFacade;
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref _isDisposed, 1))
                return;
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            _mediaSourceTaskCompletionSource.TrySetCanceled();
            _playingCancellationTokenSource.CancelDisposeSafe();
        }

        public virtual Task PlayAsync(Uri source, CancellationToken cancellationToken)
        {
            Debug.WriteLine("PlaybackSessionBase.PlayAsync() " + this);
            Task task = PlayerAsync(source, cancellationToken);
            TaskCollector.Default.Add(task, "StreamingMediaPlugin PlayerAsync");
            return MediaStreamFacade.PlayingTask;
        }

        private async Task PlayerAsync(Uri source, CancellationToken cancellationToken)
        {
            try
            {
                using (var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_playingCancellationTokenSource.Token, cancellationToken))
                {
                    MediaStreamFacade.ContentType = ContentType;
                    var mss = await MediaStreamFacade.CreateMediaStreamSourceAsync(source, linkedTokenSource.Token);
                    if (!_mediaSourceTaskCompletionSource.TrySetResult(mss))
                        throw new OperationCanceledException();
                    return;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlaybackSessionBase.PlayerAsync() failed: " + ex.ExtendedMessage());
            }
            try
            {
                _mediaSourceTaskCompletionSource.TrySetCanceled();
                await MediaStreamFacade.CloseAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlaybackSessionBase.PlayerAsync() cleanup failed: " + ex.ExtendedMessage());
            }
        }

        public virtual async Task<TMediaSource> GetMediaSourceAsync(CancellationToken cancellationToken)
        {
            return await _mediaSourceTaskCompletionSource.Task.WithCancellation(cancellationToken);
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            await MediaStreamFacade.StopAsync(cancellationToken);
            await MediaStreamFacade.PlayingTask;
        }

        public virtual async Task CloseAsync()
        {
            if (!_playingCancellationTokenSource.IsCancellationRequested)
                _playingCancellationTokenSource.Cancel();

            await MediaStreamFacade.PlayingTask;
        }

        public virtual async Task OnMediaFailedAsync()
        {
            Debug.WriteLine("PlaybackSessionBase.OnMediaFailedAsync() " + this);
            try
            {
                await CloseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlaybackSessionBase.OnMediaFailedAsync() CloseAsync() failed: " + ex.Message);
            }
        }

        public override string ToString()
        {
            return $"Playback ID {_id} IsCompleted {MediaStreamFacade.PlayingTask.IsCompleted}";
        }
    }
}
