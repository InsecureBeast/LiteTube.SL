using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Buffering;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.MediaManager
{
    public sealed class SmMediaManager : IMediaManager, IDisposable
    {
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly object _lock = new object();
        private CancellationTokenSource _closeCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource _playbackCancellationTokenSource = new CancellationTokenSource();
        private TaskCompletionSource<object> _playbackTaskCompletionSource = new TaskCompletionSource<object>();
        private const int MaxBuffers = 8;
        private readonly Func<IBufferingManager> _bufferingManagerFactory;
        private readonly IMediaParserFactory _mediaParserFactory;
        private readonly IMediaStreamConfigurator _mediaStreamConfigurator;
        private readonly Action<IProgramStreams> _programStreamsHandler;
        private readonly SignalTask _reportStateTask;
        private readonly ISegmentReaderManagerFactory _segmentReaderManagerFactory;
        private TaskCompletionSource<object> _closeTaskCompletionSource;
        private int _isDisposed;
        private MediaManagerState _mediaState;
        private string _mediaStateMessage;
        private int _openCount;
        private Task _playTask;
        private ISegmentReaderManager _readerManager;
        private IMediaReader[] _readers;

        private bool IsClosed
        {
            get
            {
                MediaManagerState state = State;
                return state == MediaManagerState.Idle || MediaManagerState.Closed == state;
            }
        }

        private bool IsRunning
        {
            get
            {
                MediaManagerState state = State;
                return MediaManagerState.OpenMedia == state || MediaManagerState.Opening == state || MediaManagerState.Playing == state || MediaManagerState.Seeking == state;
            }
        }

        public MediaManagerState State
        {
            get
            {
                lock(_lock)
                {
                    return _mediaState;
                }
            }
            private set
            {
                SetMediaState(value, null);
            }
        }

        public TimeSpan? SeekTarget
        {
            get
            {
                return _mediaStreamConfigurator.SeekTarget;
            }
            set
            {
                _mediaStreamConfigurator.SeekTarget = value;
            }
        }

        public ContentType ContentType { get; set; }

        public Task PlayingTask
        {
            get
            {
                return _playbackTaskCompletionSource.Task;
            }
        }

        public event EventHandler<MediaManagerStateEventArgs> OnStateChange;

        public SmMediaManager(ISegmentReaderManagerFactory segmentReaderManagerFactory, IMediaStreamConfigurator mediaStreamConfigurator, Func<IBufferingManager> bufferingManagerFactory, IMediaManagerParameters mediaManagerParameters, IMediaParserFactory mediaParserFactory)
        {
            if (null == segmentReaderManagerFactory)
                throw new ArgumentNullException("segmentReaderManagerFactory");
            if (null == mediaStreamConfigurator)
                throw new ArgumentNullException("mediaStreamConfigurator");
            if (null == bufferingManagerFactory)
                throw new ArgumentNullException("bufferingManagerFactory");
            _segmentReaderManagerFactory = segmentReaderManagerFactory;
            _mediaStreamConfigurator = mediaStreamConfigurator;
            _bufferingManagerFactory = bufferingManagerFactory;
            _mediaParserFactory = mediaParserFactory;
            _programStreamsHandler = mediaManagerParameters.ProgramStreamsHandler;
            _playbackCancellationTokenSource.Cancel();
            _playbackTaskCompletionSource.TrySetResult((object)null);
            _reportStateTask = new SignalTask(ReportState);
        }

        public void Dispose()
        {
            Debug.WriteLine("SmMediaManager.Dispose()");
            if (0 != Interlocked.Exchange(ref _isDisposed, 1))
                return;
            if (null != OnStateChange)
            {
                Debug.WriteLine("SmMediaManager.Dispose(): OnStateChange is not null");
                if (Debugger.IsAttached)
                    Debugger.Break();
                OnStateChange = null;
            }
            _mediaStreamConfigurator.MediaManager = null;
            CloseAsync().Wait();
            using (_reportStateTask)
            {
                using (_asyncLock)
                {
                }
            }
            using (_playbackCancellationTokenSource)
            {
            }
            using (_closeCancellationTokenSource)
            {
            }
        }

        public async Task<IMediaStreamConfigurator> OpenMediaAsync(ICollection<Uri> source, CancellationToken cancellationToken)
        {
            Debug.WriteLine("SmMediaManager.OpenMediaAsync()");
            if (null == source)
                throw new ArgumentNullException(nameof(source));
            if (source.Count == 0 || source.Any(s => null == s))
                throw new ArgumentException("No valid URLs", nameof(source));
            source = source.ToArray();
            IMediaStreamConfigurator streamConfigurator;
            using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (!IsClosed)
                    await CloseAsync().ConfigureAwait(false);
                _playbackTaskCompletionSource = new TaskCompletionSource<object>();
                State = MediaManagerState.OpenMedia;
                await OpenAsync(source).ConfigureAwait(false);
                streamConfigurator = _mediaStreamConfigurator;
            }
            return streamConfigurator;
        }

        public async Task StopMediaAsync(CancellationToken cancellationToken)
        {
            Debug.WriteLine("SmMediaManager.StopMediaAsync()");
            if (IsRunning)
            {
                using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
                    await CloseAsync().ConfigureAwait(false);
            }
        }

        public async Task CloseMediaAsync()
        {
            Debug.WriteLine("SmMediaManager.CloseMediaAsync()");
            if (!IsClosed)
            {
                try
                {
                    using (await _asyncLock.LockAsync(CancellationToken.None).ConfigureAwait(false))
                        await CloseAsync().ConfigureAwait(false);
                }
                catch (OperationCanceledException ex)
                {
                    Debug.WriteLine("SmMediaManager.CloseMediaAsync() operation canceled exception: " + ex.Message);
                }
                catch (ObjectDisposedException ex)
                {
                    Debug.WriteLine("SmMediaManager.CloseMediaAsync() object disposed exception: " + ex.Message);
                }
            }
        }

        public async Task<TimeSpan> SeekMediaAsync(TimeSpan position)
        {
            Debug.WriteLine("SmMediaManager.SeekMediaAsync({0})", position);
            TimeSpan timeSpan;
            using (await _asyncLock.LockAsync(_closeCancellationTokenSource.Token).ConfigureAwait(false))
                timeSpan = await SeekAsync(position).ConfigureAwait(false);
            return timeSpan;
        }

        private async Task CloseAsync()
        {
            Debug.WriteLine("SmMediaManager.CloseAsync()");
            TaskCompletionSource<object> closeTaskCompletionSource = new TaskCompletionSource<object>();
            TaskCompletionSource<object> currentCloseTaskCompletionSource = Interlocked.CompareExchange(ref _closeTaskCompletionSource, closeTaskCompletionSource, null);
            if (null != currentCloseTaskCompletionSource)
            {
                object obj = await currentCloseTaskCompletionSource.Task.ConfigureAwait(false);
                Debug.WriteLine("SmMediaManager.CloseAsync() completed by other caller");
            }
            else
            {
                State = MediaManagerState.Closing;
                TaskCompletionSource<object> playbackTaskCompletionSource = _playbackTaskCompletionSource;
                _closeCancellationTokenSource.Cancel();
                ConfiguredTaskAwaitable configuredTaskAwaitable = CloseCleanupAsync().ConfigureAwait(false);
                await configuredTaskAwaitable;
                State = MediaManagerState.Closed;
                configuredTaskAwaitable = _reportStateTask.WaitAsync().ConfigureAwait(false);
                await configuredTaskAwaitable;
                Debug.WriteLine("SmMediaManager.CloseAsync() completed");
                Interlocked.CompareExchange(ref _closeTaskCompletionSource, null, closeTaskCompletionSource);
                Task task = TaskEx.Run(() =>
                {
                    closeTaskCompletionSource.TrySetResult(null);
                    playbackTaskCompletionSource.TrySetResult(null);
                });
                TaskCollector.Default.Add(task, "SmMediaManager close");
            }
        }

        private async Task CloseCleanupAsync()
        {
            Debug.WriteLine("SmMediaManager.CloseCleanupAsync()");
            List<Task> tasks = new List<Task>();
            ISegmentReaderManager readerManager = _readerManager;
            if (null != readerManager)
            {
                _readerManager = null;
                tasks.Add(readerManager.StopAsync());
            }
            IMediaStreamConfigurator msc = _mediaStreamConfigurator;
            if (null != msc)
                tasks.Add(msc.CloseAsync());
            if (_readers != null && _readers.Length > 0)
                tasks.Add(CloseReadersAsync());
            if (null != _playTask)
                tasks.Add(_playTask);
            if (tasks.Count > 0)
            {
                while (tasks.Any(t => !t.IsCompleted))
                {
                    try
                    {
                        Task t = TaskEx.Delay(2500);
                        Debug.WriteLine("SmMediaManager.CloseCleanupAsync() waiting for tasks");
                        Task task = await TaskEx.WhenAny(t, TaskEx.WhenAll(tasks)).ConfigureAwait(false);
                        Debug.WriteLine("SmMediaManager.CloseCleanupAsync() finished tasks");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("SmMediaManager.CloseCleanupAsync() play task failed: " + ex.ExtendedMessage());
                    }
                }
            }
            if (null != msc)
                msc.MediaManager = null;
            DisposeReaders();
            readerManager?.DisposeSafe();
        }

        private Task ReportState()
        {
            Debug.WriteLine("SmMediaManager.ReportState() state {0} message {1}", _mediaState, _mediaStateMessage);
            MediaManagerState state;
            string message;
            lock(_lock)
            {
                state = _mediaState;
                message = _mediaStateMessage;
                _mediaStateMessage = null;
            }

            OnStateChange?.Invoke(this, new MediaManagerStateEventArgs(state, message));
            if (null != message)
            {
                IMediaStreamConfigurator streamConfigurator = _mediaStreamConfigurator;
                streamConfigurator?.ReportError(message);
            }
            return TplTaskExtensions.CompletedTask;
        }

        private void ResetCancellationToken()
        {
            Debug.WriteLine("SmMediaManager.ResetCancellationToken()");
            if (_closeCancellationTokenSource.IsCancellationRequested)
            {
                _closeCancellationTokenSource.CancelDisposeSafe();
                _closeCancellationTokenSource = new CancellationTokenSource();
            }
            if (!_playbackCancellationTokenSource.IsCancellationRequested)
                return;
            _playbackCancellationTokenSource.CancelDisposeSafe();
            _playbackCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_closeCancellationTokenSource.Token);
        }

        private void SetMediaState(MediaManagerState state, string message)
        {
            lock (_lock)
            {
                if (state == _mediaState)
                    return;
                Debug.WriteLine("SmMediaManager.SetMediaState() {0} -> {1}", _mediaState, state);
                _mediaState = state;
                if (null != message)
                    _mediaStateMessage = message;
            }
            _reportStateTask.Fire();
        }

        private void StartReaders()
        {
            CancellationToken token = _playbackCancellationTokenSource.Token;
            Task<Task> task = TaskEx.WhenAll(_readers.Select(r => (Task)r.ReadAsync(token))).ContinueWith(async t =>
            {
                AggregateException ex = t.Exception;
                if (null != ex)
                {
                    Debug.WriteLine("SmMediaManager.StartReaders() ReadAsync failed: " + ex.ExtendedMessage());
                    SetMediaState(MediaManagerState.Error, ex.ExtendedMessage());
                    lock(_lock)
                    {
                        if (null != _closeTaskCompletionSource)
                            return;
                    }
                    try
                    {
                        await CloseMediaAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex1)
                    {
                        Debug.WriteLine("SmMediaManager.StartReaders() ReadAsync close media failed " + ex1);
                    }
                }
            }, token, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
            TaskCollector.Default.Add(task, "SmMediaManager.StartReaders() cleanup tasks");
        }

        private async Task OpenAsync(ICollection<Uri> source)
        {
            Debug.WriteLine("SmMediaManager.OpenAsync() state " + State);
            State = MediaManagerState.Opening;
            ++_openCount;
            ResetCancellationToken();
            Task<IMediaReader>[] readerTasks = null;
            Exception exception;
            try
            {
                _mediaStreamConfigurator.Initialize();
                _readerManager = await _segmentReaderManagerFactory.CreateAsync(new SegmentManagerParameters()
                {
                    Source = source
                }, ContentType, _playbackCancellationTokenSource.Token).ConfigureAwait(false);
                if (null == _readerManager)
                {
                    Debug.WriteLine("SmMediaManager.OpenAsync() unable to create reader manager");
                    SetMediaState(MediaManagerState.Error, "Unable to create reader");
                    return ;
                }
                else
                {
                    readerTasks = _readerManager.SegmentManagerReaders.Select(CreateReaderPipeline).ToArray();
                    _readers = await TaskEx.WhenAll<IMediaReader>(readerTasks).ConfigureAwait(false);
                    foreach (IMediaReader mediaReader in _readers)
                        mediaReader.IsEnabled = true;

                    TimeSpan timeSpan = await _readerManager.StartAsync(_playbackCancellationTokenSource.Token).ConfigureAwait(false);
                    _mediaStreamConfigurator.MediaManager = (this);
                    StartReaders();
                    return;
                }
            }
            catch (OperationCanceledException ex)
            {
                SetMediaState(MediaManagerState.Error, "Media play canceled");
                exception = ex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SmMediaManager.OpenAsync() failed: " + ex.Message);
                SetMediaState(MediaManagerState.Error, "Unable to play media");
                exception = new AggregateException(ex.Message, ex);
            }
            await CleanupFailedOpenAsync(readerTasks);
            throw exception;
        }

        private async Task CleanupFailedOpenAsync(Task<IMediaReader>[] readerTasks)
        {
            Debug.WriteLine("SmMediaManager.CleanupFailedOpenAsync() state " + State);
            _playbackCancellationTokenSource.Cancel();
            if (_readers == null && null != readerTasks)
            {
                _readers = readerTasks.Where(r =>
                {
                    if (null == r)
                        return false;

                    AggregateException exception = r.Exception;
                    if (null == exception)
                        return r.IsCompleted;
                    Debug.WriteLine("SmMediaManager.CleanupFailedOpenAsync(): reader create failed: " + exception.Message);
                    return false;
                }).Select(r => r.Result).ToArray();
                await CloseReadersAsync().ConfigureAwait(false);
                DisposeReaders();
            }
            if (null != _readerManager)
            {
                _readerManager.DisposeSafe();
                _readerManager = null;
            }
        }

        private async Task<IMediaReader> CreateReaderPipeline(ISegmentManagerReaders segmentManagerReaders)
        {
            MediaReader reader = new MediaReader(_bufferingManagerFactory(), _mediaParserFactory, segmentManagerReaders, new WorkBufferBlockingPool(8));
            await reader.InitializeAsync(segmentManagerReaders, CheckConfigurationCompleted, _mediaStreamConfigurator.CheckForSamples, _playbackCancellationTokenSource.Token, _programStreamsHandler).ConfigureAwait(false);
            return reader;
        }

        private void CheckConfigurationCompleted()
        {
            MediaManagerState state = State;
            if (MediaManagerState.Opening != state && MediaManagerState.OpenMedia != state || (_readers == null || _readers.Any(r => !r.IsConfigured)))
                return;
            _playTask = _mediaStreamConfigurator.PlayAsync(_readers.SelectMany(r => r.MediaStreams), _readerManager.Duration, _closeCancellationTokenSource.Token);
            State = MediaManagerState.Playing;
            int openCount = _openCount;
            _playTask.ContinueWith(async t =>
            {
                AggregateException taskException = t.Exception;
                if (null != taskException)
                    Debug.WriteLine("SmMediaManager.CheckConfigurationComplete() play task failed: " + taskException.Message);
                try
                {
                    using (await _asyncLock.LockAsync(CancellationToken.None).ConfigureAwait(false))
                    {
                        if (openCount == _openCount)
                            await CloseAsync().ConfigureAwait(false);
                        else
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SmMediaManager.CheckConfigurationComplete() play continuation failed: " + ex.Message);
                }
            });
        }

        private async Task CloseReadersAsync()
        {
            Debug.WriteLine("SmMediaManager.CloseReadersAsync() closing readers");
            if (_readers == null || _readers.Length < 1)
            {
                Debug.WriteLine("SmMediaManager.CloseReadersAsync() no readers");
            }
            else
            {
                try
                {
                    IEnumerable<Task> tasks = _readers.Select(async reader =>
                    {
                        if (null != reader)
                        {
                            try
                            {
                                await reader.CloseAsync().ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("SmMediaManager.CloseReadersAsync(): reader.CloseAsync failed: " + ex.Message);
                            }
                        }
                    }).Where(t => null != t);
                    await TaskEx.WhenAll(tasks).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SmMediaManager.CloseReadersAsync: task failed: " + ex.Message);
                }
                Debug.WriteLine("SmMediaManager.CloseReadersAsync() readers closed");
            }
        }

        private void DisposeReaders()
        {
            Debug.WriteLine("SmMediaManager.DisposeReaders()");
            IMediaReader[] mediaReaderArray = _readers;
            _readers = null;
            if (mediaReaderArray == null || mediaReaderArray.Length < 1)
                return;
            foreach (var disposable in mediaReaderArray)
                disposable.DisposeBackground("SmMediaManager dispose reader");
            Debug.WriteLine("SmMediaManager.DisposeReaders() completed");
        }

        private bool IsSeekInRange(TimeSpan position)
        {
            return _readers.All(reader => reader.IsBuffered(position));
        }

        private async Task<TimeSpan> SeekAsync(TimeSpan position)
        {
            Debug.WriteLine("SmMediaManager.SeekAsync()");
            TimeSpan timeSpan;
            if (_playbackCancellationTokenSource.IsCancellationRequested)
            {
                timeSpan = TimeSpan.MinValue;
            }
            else
            {
                try
                {
                    if (IsSeekInRange(position))
                    {
                        return position;
                    }
                    else
                    {
                        IMediaReader[] readers = _readers;
                        if (readers == null || readers.Length < 1)
                        {
                            return TimeSpan.MinValue;
                        }
                        else
                        {
                            await TaskEx.WhenAll(readers.Select(reader => reader.StopAsync())).ConfigureAwait(false);
                            if (_playbackCancellationTokenSource.IsCancellationRequested)
                            {
                                return TimeSpan.MinValue;
                            }
                            else
                            {
                                foreach (IMediaReader mediaReader in readers)
                                    mediaReader.IsEnabled = true;
                                
                                State = MediaManagerState.Seeking;
                                TimeSpan actualPosition = await _readerManager.SeekAsync(position, _playbackCancellationTokenSource.Token).ConfigureAwait(false);
                                StartReaders();
                                timeSpan = actualPosition;
                                return timeSpan;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SmMediaManager.SeekAsync() failed: " + ex.Message);
                }
                timeSpan = TimeSpan.MinValue;
            }
            
            return timeSpan;
        }
    }
}
