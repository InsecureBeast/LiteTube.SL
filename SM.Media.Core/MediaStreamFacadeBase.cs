using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Builder;
using SM.Media.Core.Content;
using SM.Media.Core.MediaManager;
using SM.Media.Core.MediaParser;
using SM.Media.Core.Utility;
using AsyncLock = SM.Media.Core.Utility.AsyncLock;

namespace SM.Media.Core
{
    public abstract class MediaStreamFacadeBase<TMediaStreamSource> : IMediaStreamFacadeBase<TMediaStreamSource>, IMediaStreamFacadeBase, IDisposable where TMediaStreamSource : class
    {
        private readonly Utility.AsyncLock _asyncLock = new AsyncLock();
        private readonly CancellationTokenSource _disposeCancellationTokenSource = new CancellationTokenSource();
        private readonly object _lock = new object();
        private readonly IBuilder<IMediaManager> _mediaManagerBuilder;
        private CancellationTokenSource _closeCancellationTokenSource;
        private int _isDisposed;
        private IMediaManager _mediaManager;

        private IMediaManager MediaManager
        {
            get
            {
                bool lockTaken = false;
                object obj = null;
                try
                {
                    Monitor.Enter(obj = this._lock, ref lockTaken);
                    return this._mediaManager;
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(obj);
                }
            }
            set
            {
                this.SetMediaManager(value);
            }
        }

        public bool IsDisposed
        {
            get
            {
                return 0 != this._isDisposed;
            }
        }

        public Task PlayingTask
        {
            get
            {
                bool lockTaken = false;
                object obj = null;
                IMediaManager mediaManager;
                try
                {
                    Monitor.Enter(obj = this._lock, ref lockTaken);
                    mediaManager = this._mediaManager;
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(obj);
                }
                if (null == mediaManager)
                    return TplTaskExtensions.CompletedTask;
                return mediaManager.PlayingTask;
            }
        }

        public ContentType ContentType { get; set; }

        public TimeSpan? SeekTarget
        {
            get
            {
                IMediaManager mediaManager = this.MediaManager;
                return mediaManager == null ? new TimeSpan?() : mediaManager.SeekTarget;
            }
            set
            {
                this.ThrowIfDisposed();
                IMediaManager mediaManager = this.MediaManager;
                if (null == mediaManager)
                    return;
                mediaManager.SeekTarget = value;
            }
        }

        public MediaManagerState State
        {
            get
            {
                IMediaManager mediaManager = this.MediaManager;
                if (null == mediaManager)
                    return MediaManagerState.Closed;
                return mediaManager.State;
            }
        }

        public IBuilder<IMediaManager> Builder
        {
            get
            {
                return this._mediaManagerBuilder;
            }
        }

        public event EventHandler<MediaManagerStateEventArgs> StateChange;

        protected MediaStreamFacadeBase(IBuilder<IMediaManager> mediaManagerBuilder)
        {
            if (mediaManagerBuilder == null)
                throw new ArgumentNullException("mediaManagerBuilder");
            this._mediaManagerBuilder = mediaManagerBuilder;
            this._closeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._disposeCancellationTokenSource.Token);
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
                return;
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("MediaStreamFacadeBase.StopAsync()");
            this.ThrowIfDisposed();
            using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._closeCancellationTokenSource.Token))
                await this.StopMediaAsync(this.MediaManager, linkedTokenSource.Token).ConfigureAwait(false);
        }

        public async Task CloseAsync()
        {
            Debug.WriteLine("MediaStreamFacadeBase.CloseAsync()");
            await this.CloseMediaManagerAsync(this.MediaManager).ConfigureAwait(false);
        }

        public virtual async Task<TMediaStreamSource> CreateMediaStreamSourceAsync(Uri source, CancellationToken cancellationToken)
        {
            Debug.WriteLine("MediaStreamFacadeBase.CreateMediaStreamSourceAsync() " + (object)source);
            if ((Uri)null != source && !source.IsAbsoluteUri)
                throw new ArgumentException("source must be absolute: " + (object)source);
            CancellationTokenSource closeCts = CancellationTokenSource.CreateLinkedTokenSource(this._disposeCancellationTokenSource.Token);
            if (!this._closeCancellationTokenSource.IsCancellationRequested)
                this._closeCancellationTokenSource.Cancel();
            Exception exception;
            TMediaStreamSource mediaStreamSource;
            try
            {
                using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, closeCts.Token))
                {
                    linkedTokenSource.CancelAfter(MediaStreamFacadeSettings.Parameters.CreateTimeout);
                    IMediaManager mediaManager = this.MediaManager;
                    if (null != mediaManager)
                        await this.StopMediaAsync(mediaManager, linkedTokenSource.Token).ConfigureAwait(false);
                    this._closeCancellationTokenSource.Dispose();
                    this._closeCancellationTokenSource = closeCts;
                    if ((Uri)null == source)
                    {
                        mediaStreamSource = default(TMediaStreamSource);
                        goto label_21;
                    }
                    else
                    {
                        IMediaManager mediaManager1 = this.MediaManager;
                        if (mediaManager1 == null)
                            mediaManager1 = await this.CreateMediaManagerAsync(linkedTokenSource.Token).ConfigureAwait(false);
                        mediaManager = mediaManager1;
                        IMediaStreamConfigurator configurator = await mediaManager.OpenMediaAsync(new Uri[1]
                        {
                            source
                        }, linkedTokenSource.Token).ConfigureAwait(false);
                        TMediaStreamSource mss = await configurator.CreateMediaStreamSourceAsync<TMediaStreamSource>(linkedTokenSource.Token).ConfigureAwait(false);
                        mediaStreamSource = mss;
                        goto label_21;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                exception = (Exception)ex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MediaStreamFacadeBase.CreateAsync() failed: " + Utility.ExceptionExtensions.ExtendedMessage(ex));
                exception = (Exception)new AggregateException(ex.Message, ex);
            }
            await this.CloseAsync().ConfigureAwait(false);
            throw exception;
            label_21:
            return mediaStreamSource;
        }

        private void SetMediaManager(IMediaManager value)
        {
            if (object.ReferenceEquals((object)this._mediaManager, (object)value))
                return;
            bool lockTaken = false;
            object obj = null;
            IMediaManager mediaManager;
            try
            {
                Monitor.Enter(obj = this._lock, ref lockTaken);
                mediaManager = this._mediaManager;
                this._mediaManager = value;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(obj);
            }
            if (null == mediaManager)
                return;
            Debug.WriteLine("**** MediaStreamFacadeBase.SetMediaManager() _mediaManager was not null");
            this.CleanupMediaManager(mediaManager);
        }

        private void ThrowIfDisposed()
        {
            if (0 != this._isDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        protected virtual void Dispose(bool disposing)
        {
            Debug.WriteLine("MediaStreamFacadeBase.Dispose({0})", (object)(bool)(disposing ? true : false));
            if (!disposing)
                return;
            if (!this._closeCancellationTokenSource.IsCancellationRequested)
                this._closeCancellationTokenSource.Cancel();
            if (!this._disposeCancellationTokenSource.IsCancellationRequested)
                this._disposeCancellationTokenSource.Cancel();
            this._asyncLock.LockAsync(CancellationToken.None).Wait();
            this.StateChange = (EventHandler<MediaManagerStateEventArgs>)null;
            bool lockTaken = false;
            object obj = null;
            IMediaManager mediaManager;
            try
            {
                Monitor.Enter(obj = this._lock, ref lockTaken);
                mediaManager = this._mediaManager;
                this._mediaManager = (IMediaManager)null;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(obj);
            }
            if (null != mediaManager)
                this.CleanupMediaManager(mediaManager);
            DisposeExtensions.DisposeSafe((IDisposable)this._mediaManagerBuilder);
            this._asyncLock.Dispose();
            this._closeCancellationTokenSource.Dispose();
            this._disposeCancellationTokenSource.Dispose();
        }

        private void CleanupMediaManager(IMediaManager mediaManager)
        {
            Debug.WriteLine("MediaStreamFacadeBase.CleanupMediaManager()");
            if (null == mediaManager)
                return;
            mediaManager.OnStateChange -= new EventHandler<MediaManagerStateEventArgs>(this.MediaManagerOnStateChange);
            DisposeExtensions.DisposeSafe((IDisposable)mediaManager);
            this._mediaManagerBuilder.Destroy(mediaManager);
            Debug.WriteLine("MediaStreamFacadeBase.CleanupMediaManager() completed");
        }

        protected async Task<IMediaManager> CreateMediaManagerAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IMediaManager mediaManager1;
            using (await this._asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (null != this.MediaManager)
                    throw new InvalidOperationException("A MediaManager already exists");
                cancellationToken.ThrowIfCancellationRequested();
                IMediaManager mediaManager2 = this.CreateMediaManager();
                Debug.Assert(null == this._mediaManager);
                this.MediaManager = mediaManager2;
                mediaManager1 = mediaManager2;
            }
            return mediaManager1;
        }

        private IMediaManager CreateMediaManager()
        {
            Debug.WriteLine("MediaStreamFacadeBase.CreateMediaManager()");
            Debug.Assert(null == this.MediaManager);
            IMediaManager mediaManager = this._mediaManagerBuilder.Create();
            mediaManager.ContentType = this.ContentType;
            mediaManager.OnStateChange += new EventHandler<MediaManagerStateEventArgs>(this.MediaManagerOnStateChange);
            return mediaManager;
        }

        private async Task StopMediaAsync(IMediaManager mediaManager, CancellationToken cancellationToken)
        {
            Debug.WriteLine("MediaStreamFacadeBase.StopMediaAsync()");
            using (await this._asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                IMediaManager mm = this.MediaManager;
                if (mm != null && object.ReferenceEquals((object)mm, (object)mediaManager))
                    await mm.StopMediaAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task CloseMediaManagerAsync(IMediaManager mediaManager)
        {
            Debug.WriteLine("MediaStreamFacadeBase.CloseMediaAsync()");
            using (await this._asyncLock.LockAsync(CancellationToken.None).ConfigureAwait(false))
                await this.UnlockedCloseMediaManagerAsync(mediaManager).ConfigureAwait(false);
        }

        private async Task UnlockedCloseMediaManagerAsync(IMediaManager mediaManager)
        {
            Debug.WriteLine("MediaStreamFacadeBase.UnlockedCloseMediaAsync()");
            bool lockTaken = false;
            IMediaManager mm;
            object obj = null;
            try
            {
                Monitor.Enter(obj = this._lock, ref lockTaken);
                mm = this._mediaManager;
                if (mm != null && object.ReferenceEquals((object)mm, (object)mediaManager))
                    this._mediaManager = (IMediaManager)null;
                else
                    goto label_12;
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(obj);
            }
            if (!this._closeCancellationTokenSource.IsCancellationRequested)
                this._closeCancellationTokenSource.Cancel();
            try
            {
                Debug.WriteLine("MediaStreamFacadeBase.UnlockedCloseMediaManagerAsync() calling mediaManager.CloseAsync()");
                await mm.CloseMediaAsync().ConfigureAwait(false);
                Debug.WriteLine("MediaStreamFacadeBase.UnlockedCloseMediaManagerAsync() returned from mediaManager.CloseAsync()");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MediaStreamFacadeBase.UnlockedCloseMediaManagerAsync() Media manager close failed: " + ex.Message);
            }
            this.CleanupMediaManager(mm);
            Debug.WriteLine("MediaStreamFacadeBase.UnlockedCloseMediaManagerAsync() completed");
            label_12:;
        }

        private async void MediaManagerOnStateChange(object sender, MediaManagerStateEventArgs e)
        {
            Debug.WriteLine("MediaStreamFacadeBase.MediaManagerOnStateChange() to {0}: {1}", (object)e.State, (object)e.Message);
            if (e.State == MediaManagerState.Closed)
            {
                try
                {
                    await this.StopMediaAsync((IMediaManager)sender, this._closeCancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MediaStreamFacadeBase.MediaManagerOnStateChange() stop failed: " + ex.Message);
                }
            }
            EventHandler<MediaManagerStateEventArgs> stateChange = this.StateChange;
            if (null != stateChange)
            {
                try
                {
                    stateChange((object)this, e);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("MediaStreamFacadeBase.MediaManagerOnStateChange() Exception in StateChange event handler: " + ex.Message);
                }
            }
        }
    }
}
