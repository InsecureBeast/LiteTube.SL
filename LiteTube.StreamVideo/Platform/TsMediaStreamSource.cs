using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using LiteTube.StreamVideo.Pes;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Platform
{
    public sealed class TsMediaStreamSource : MediaStreamSource, IDisposable
    {
        private static readonly Dictionary<MediaSampleAttributeKeys, string> NoMediaSampleAttributes = new Dictionary<MediaSampleAttributeKeys, string>();
        private MediaStreamFsm _mediaStreamFsm = new MediaStreamFsm();
        private readonly PesStream _pesStream = new PesStream();
        private readonly object _stateLock = new object();
        private bool _isClosed = true;
        private readonly SingleThreadSignalTaskScheduler _taskScheduler;
        private float _bufferingProgress;
        private TaskCompletionSource<object> _closeCompleted;
        private int _isDisposed;
        private volatile int _pendingOperations;
        private TimeSpan _pendingSeekTarget;
        private TimeSpan? _seekTarget;
        private TsMediaStreamSource.SourceState _state;
        private MediaStreamDescription _videoStreamDescription;
        private MediaStreamDescription _audioStreamDescription;
        private TsMediaStreamSource.Operation _streamOpenFlags;
        private IStreamSource _videoStreamSource;
        private IStreamSource _audioStreamSource;
        private readonly IMediaStreamControl _streamControl;

        private IStreamSource VideoStreamSource
        {
            get
            {
                return this._videoStreamSource;
            }
            set
            {
                lock (this._stateLock)
                {
                    if (null == value)
                    {
                        this._streamOpenFlags &= ~TsMediaStreamSource.Operation.Video;
                        this._videoStreamSource = (IStreamSource)null;
                    }
                    else
                    {
                        this._streamOpenFlags |= TsMediaStreamSource.Operation.Video;
                        this._videoStreamSource = value;
                    }
                }
            }
        }

        private IStreamSource AudioStreamSource
        {
            get
            {
                return this._audioStreamSource;
            }
            set
            {
                lock (this._stateLock)
                {
                    if (null == value)
                        this._streamOpenFlags &= ~TsMediaStreamSource.Operation.Audio;
                    else
                        this._streamOpenFlags |= TsMediaStreamSource.Operation.Audio;
                    this._audioStreamSource = value;
                }
            }
        }

        private bool IsDisposed
        {
            get
            {
                return 0 != this._isDisposed;
            }
        }

        private TsMediaStreamSource.SourceState State
        {
            get
            {
                lock (this._stateLock)
                    return this._state;
            }
            set
            {
                lock (this._stateLock)
                {
                    if (this._state == value)
                        return;
                    this._state = value;
                }
                this.CheckPending();
            }
        }

        public TimeSpan? SeekTarget
        {
            get
            {
                lock (this._stateLock)
                    return this._seekTarget;
            }
            set
            {
                lock (this._stateLock)
                    this._seekTarget = value;
            }
        }

        public TsMediaStreamSource(IMediaStreamControl mediaStreamControl)
        {
            if (null == mediaStreamControl)
                throw new ArgumentNullException("mediaStreamControl");
            this._streamControl = mediaStreamControl;
            this._mediaStreamFsm.Reset();
            this._taskScheduler = new SingleThreadSignalTaskScheduler("TsMediaStreamSource", new Action(this.SignalHandler));
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
                return;
            Debug.WriteLine("TsMediaStreamSource.Dispose()");
            this.ValidateEvent(MediaStreamFsm.MediaEvent.DisposeCalled);
            TaskCompletionSource<object> completionSource;
            lock (this._stateLock)
            {
                this._isClosed = true;
                completionSource = this._closeCompleted;
                this._closeCompleted = (TaskCompletionSource<object>)null;
            }
            if (null != completionSource)
                completionSource.TrySetResult((object)string.Empty);
            if (null != this._taskScheduler)
                this._taskScheduler.Dispose();
            this.ForceClose();
            this._pesStream.Dispose();
        }

        private void ForceClose()
        {
            TsMediaStreamSource.Operation operation = this.HandleOperation(TsMediaStreamSource.Operation.Audio | TsMediaStreamSource.Operation.Video | TsMediaStreamSource.Operation.Seek);
            if (TsMediaStreamSource.Operation.None != (operation & TsMediaStreamSource.Operation.Seek))
            {
                this.ValidateEvent(MediaStreamFsm.MediaEvent.CallingReportSeekCompleted);
                this.ReportSeekCompleted(0L);
            }
            if ((operation & TsMediaStreamSource.Operation.Video) != TsMediaStreamSource.Operation.None && null != this._videoStreamDescription)
                this.SendLastStreamSample(this._videoStreamDescription);
            if ((operation & TsMediaStreamSource.Operation.Audio) == TsMediaStreamSource.Operation.None || null == this._audioStreamDescription)
                return;
            this.SendLastStreamSample(this._audioStreamDescription);
        }

        public void ReportError(string message)
        {
            Task task = Task.Factory.StartNew((Action)(() => this.ErrorOccurred(message)), CancellationToken.None, TaskCreationOptions.None, (TaskScheduler)this._taskScheduler);
            TaskCollector.Default.Add(task, "TsMediaStreamSource ReportError");
        }

        public Task CloseAsync()
        {
            Debug.WriteLine("TsMediaStreamSource.CloseAsync(): close {0}", this._closeCompleted == null ? (object)"<none>" : (object)this._closeCompleted.Task.Status.ToString());
            bool flag;
            TaskCompletionSource<object> closeCompleted;
            lock (this._stateLock)
            {
                this._isClosed = true;
                flag = TsMediaStreamSource.SourceState.Closed == this._state;
                if (!flag)
                    this._state = TsMediaStreamSource.SourceState.WaitForClose;
                closeCompleted = this._closeCompleted;
                if (closeCompleted != null && closeCompleted.Task.IsCompleted)
                {
                    closeCompleted = (TaskCompletionSource<object>)null;
                    this._closeCompleted = (TaskCompletionSource<object>)null;
                }
            }
            if (this._streamOpenFlags == TsMediaStreamSource.Operation.None || flag)
            {
                if (null != closeCompleted)
                    closeCompleted.TrySetResult((object)string.Empty);
                return TplTaskExtensions.CompletedTask;
            }
            if (null == closeCompleted)
                return TplTaskExtensions.CompletedTask;
            this.CheckPending();
            Task task = TaskEx.Delay(7000).ContinueWith((Action<Task>)(t =>
            {
                if (!closeCompleted.TrySetCanceled())
                    return;
                Debug.WriteLine("TsMediaStreamSource.CloseAsync() close timeout (remember to set MediaElement.Source to null before removing it from the visual tree)");
                this.FireCloseMediaHandler();
            }));
            TaskCollector.Default.Add(task, "TsMediaStreamSource CloseAsync timeout");
            return (Task)closeCompleted.Task;
        }

        [Conditional("DEBUG")]
        public void ValidateEvent(MediaStreamFsm.MediaEvent mediaEvent)
        {
            this._mediaStreamFsm.ValidateEvent(mediaEvent);
        }

        public void CheckForSamples()
        {
            if (0 == (this._pendingOperations & 3))
                return;
            this._taskScheduler.Signal();
        }

        private void CheckPending()
        {
            if (0 == this._pendingOperations)
                return;
            this._taskScheduler.Signal();
        }

        private async Task SeekHandler()
        {
            this._taskScheduler.ThrowIfNotOnThread();
            TimeSpan seekTimestamp;
            lock (this._stateLock)
            {
                if (!this._isClosed)
                    seekTimestamp = this._pendingSeekTarget;
                else
                    goto label_11;
            }
            try
            {
                TimeSpan position = await this._streamControl.SeekAsync(seekTimestamp, CancellationToken.None).ConfigureAwait(true);
                this._taskScheduler.ThrowIfNotOnThread();
                if (!this._isClosed)
                {
                    this.ValidateEvent(MediaStreamFsm.MediaEvent.CallingReportSeekCompleted);
                    this.ReportSeekCompleted(position.Ticks);
                    Debug.WriteLine("TsMediaStreamSource.SeekHandler({0}) completed, actual: {1}", (object)seekTimestamp, (object)position);
                    this.State = TsMediaStreamSource.SourceState.Play;
                    this._bufferingProgress = -1f;
                }
                else
                    goto label_11;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TsMediaStreamSource.SeekHandler({0}) failed: {1}", (object)seekTimestamp, (object)ex.Message);
                this.ErrorOccurred("Seek failed: " + ex.Message);
                this._taskScheduler.ThrowIfNotOnThread();
            }
            this.CheckPending();
            label_11:;
        }

        private void SignalHandler()
        {
            this._taskScheduler.ThrowIfNotOnThread();
            TsMediaStreamSource.Operation operation1 = TsMediaStreamSource.Operation.None;
            TsMediaStreamSource.Operation operation2 = TsMediaStreamSource.Operation.None;
            try
            {
                if (this._isClosed)
                {
                    this.ForceClose();
                }
                else
                {
                    bool flag;
                    do
                    {
                        if (TsMediaStreamSource.Operation.None == this.HandleOperation(TsMediaStreamSource.Operation.Seek))
                        {
                            if (TsMediaStreamSource.SourceState.Play == this.State)
                            {
                                operation1 = this.HandleOperation(TsMediaStreamSource.Operation.Audio | TsMediaStreamSource.Operation.Video);
                                operation2 |= operation1;
                                if (TsMediaStreamSource.Operation.None != operation2)
                                {
                                    TsMediaStreamSource.Operation operation3 = this._streamOpenFlags;
                                    bool canCallReportBufferingProgress = operation3 == (operation2 & operation3);
                                    flag = false;
                                    if (TsMediaStreamSource.Operation.None != (operation2 & TsMediaStreamSource.Operation.Video) && null != this.VideoStreamSource && this.SendStreamSample(this.VideoStreamSource, this._videoStreamDescription, canCallReportBufferingProgress))
                                    {
                                        operation2 &= ~TsMediaStreamSource.Operation.Video;
                                        flag = true;
                                    }
                                    if (TsMediaStreamSource.Operation.None != (operation2 & TsMediaStreamSource.Operation.Audio) && null != this.AudioStreamSource && this.SendStreamSample(this.AudioStreamSource, this._audioStreamDescription, canCallReportBufferingProgress))
                                    {
                                        operation2 &= ~TsMediaStreamSource.Operation.Audio;
                                        flag = true;
                                    }
                                }
                                else
                                    goto label_3;
                            }
                            else
                                goto label_8;
                        }
                        else
                            goto label_4;
                    }
                    while (flag);
                    goto label_20;
                    label_4:
                    if (TsMediaStreamSource.Operation.None != operation1)
                        operation2 |= operation1;
                    Task task = this.SeekHandler();
                    TaskCollector.Default.Add(task, "TsMediaStreamSource.SignalHandler SeekHandler()");
                    return;
                    label_8:
                    return;
                    label_3:
                    return;
                    label_20:;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TsMediaStreamSource.SignalHandler() failed: " + ExceptionExtensions.ExtendedMessage(ex));
            }
            finally
            {
                if (TsMediaStreamSource.Operation.None != operation2)
                {
                    Debug.WriteLine("TsMediaStreamSource.SignalHandler() re-requesting " + (object)operation2);
                    this.RequestOperation(operation2);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (0 != this._isDisposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        private bool SendStreamSample(IStreamSource source, MediaStreamDescription mediaStreamDescription, bool canCallReportBufferingProgress)
        {
            this._taskScheduler.ThrowIfNotOnThread();
            TsPesPacket nextSample = source.GetNextSample();
            if (null == nextSample)
            {
                if (source.IsEof)
                    return this.SendLastStreamSample(mediaStreamDescription);
                if (canCallReportBufferingProgress)
                {
                    float? bufferingProgress = source.BufferingProgress;
                    if (bufferingProgress.HasValue)
                    {
                        if ((double)Math.Abs(this._bufferingProgress - bufferingProgress.Value) < 0.05)
                            return false;
                        string format = "Sample {0} buffering {1:F2}%";
                        object[] objArray1 = new object[2]
                        {
              (object) mediaStreamDescription.Type,
              null
                        };
                        object[] objArray2 = objArray1;
                        int index = 1;
                        float? nullable = bufferingProgress;
                        // ISSUE: variable of a boxed type
                        var local = (ValueType)(nullable.HasValue ? new float?(nullable.GetValueOrDefault() * 100f) : new float?());
                        objArray2[index] = (object)local;
                        object[] objArray3 = objArray1;
                        Debug.WriteLine(format, objArray3);
                        this._bufferingProgress = bufferingProgress.Value;
                        this.ValidateEvent(MediaStreamFsm.MediaEvent.CallingReportSampleCompleted);
                        this.ReportGetSampleProgress((double)bufferingProgress.Value);
                    }
                    else
                    {
                        Debug.WriteLine("Sample {0} not buffering", (object)mediaStreamDescription.Type);
                        nextSample = source.GetNextSample();
                    }
                }
                if (null == nextSample)
                    return false;
            }
            this._bufferingProgress = -1f;
            try
            {
                this._pesStream.Packet = nextSample;
                MediaStreamSample mediaStreamSample = new MediaStreamSample(mediaStreamDescription, (Stream)this._pesStream, 0L, (long)nextSample.Length, nextSample.PresentationTimestamp.Ticks, (IDictionary<MediaSampleAttributeKeys, string>)TsMediaStreamSource.NoMediaSampleAttributes);
                this.ValidateEvent(MediaStreamFsm.MediaEvent.CallingReportSampleCompleted);
                this.ReportGetSampleCompleted(mediaStreamSample);
            }
            finally
            {
                this._pesStream.Packet = (TsPesPacket)null;
                source.FreeSample(nextSample);
            }
            return true;
        }

        private bool SendLastStreamSample(MediaStreamDescription mediaStreamDescription)
        {
            this._taskScheduler.ThrowIfNotOnThread();
            this.ReportGetSampleProgress(1.0);
            MediaStreamSample mediaStreamSample = new MediaStreamSample(mediaStreamDescription, (Stream)null, 0L, 0L, 0L, (IDictionary<MediaSampleAttributeKeys, string>)TsMediaStreamSource.NoMediaSampleAttributes);
            Debug.WriteLine("Sample {0} is null", (object)mediaStreamDescription.Type);
            switch (mediaStreamDescription.Type)
            {
                case MediaStreamType.Audio:
                    this.AudioStreamSource = (IStreamSource)null;
                    break;
                case MediaStreamType.Video:
                    this.VideoStreamSource = (IStreamSource)null;
                    break;
                default:
                    Debug.Assert(false, "Unknown stream type: " + (object)mediaStreamDescription.Type);
                    break;
            }
            bool flag = this.VideoStreamSource == null && null == this.AudioStreamSource;
            if (flag)
            {
                Debug.WriteLine("TsMediaStreamSource.SendLastStreamSample() All streams closed");
                lock (this._stateLock)
                {
                    this._isClosed = true;
                    if (TsMediaStreamSource.SourceState.Closed != this._state)
                        this._state = TsMediaStreamSource.SourceState.WaitForClose;
                }
            }
            this.ValidateEvent(MediaStreamFsm.MediaEvent.CallingReportSampleCompleted);
            this.ReportGetSampleCompleted(mediaStreamSample);
            if (flag)
                this.ValidateEvent(MediaStreamFsm.MediaEvent.StreamsClosed);
            return true;
        }

        protected override async void OpenMediaAsync()
        {
            Debug.WriteLine("TsMediaStreamSource.OpenMediaAsync()");
            this.ValidateEvent(MediaStreamFsm.MediaEvent.OpenMediaAsyncCalled);
            this.ThrowIfDisposed();
            lock (this._stateLock)
            {
                this._isClosed = false;
                this._state = TsMediaStreamSource.SourceState.Open;
                Debug.Assert(null == this._closeCompleted, "TsMediaStreamSource.OpenMediaAsync() stream is already playing");
                this._closeCompleted = new TaskCompletionSource<object>();
            }
            this._bufferingProgress = -1f;
            try
            {
                IMediaStreamConfiguration configuration = await this._streamControl.OpenAsync(CancellationToken.None).ConfigureAwait(false);
                this.Configure(configuration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TsMediaStreamSource.OpenMediaAsync() failed: " + ex.Message);
                this.ReportError("Unable to open stream " + ex.Message);
            }
        }

        private void Configure(IMediaStreamConfiguration configuration)
        {
            ICollection<MediaStreamDescription> descriptions = configuration.Descriptions;
            Debug.WriteLine("TsMediaStreamSource: ReportOpenMediaCompleted ({0} streams)", (object)descriptions.Count);
            this.VideoStreamSource = configuration.VideoStreamSource;
            this.AudioStreamSource = configuration.AudioStreamSource;
            IDictionary<MediaSourceAttributesKeys, string> attributes = configuration.Attributes;
            foreach (KeyValuePair<MediaSourceAttributesKeys, string> keyValuePair in (IEnumerable<KeyValuePair<MediaSourceAttributesKeys, string>>)attributes)
                Debug.WriteLine("TsMediaStreamSource: ReportOpenMediaCompleted {0} = {1}", (object)keyValuePair.Key, (object)keyValuePair.Value);
            foreach (MediaStreamDescription streamDescription in (IEnumerable<MediaStreamDescription>)descriptions)
            {
                switch (streamDescription.Type)
                {
                    case MediaStreamType.Audio:
                        this._audioStreamDescription = streamDescription;
                        break;
                    case MediaStreamType.Video:
                        this._videoStreamDescription = streamDescription;
                        break;
                }
            }
            bool canSeek = configuration.Duration.HasValue;
            Task task = Task.Factory.StartNew((Action)(() =>
            {
                this._taskScheduler.ThrowIfNotOnThread();
                this.ValidateEvent(canSeek ? MediaStreamFsm.MediaEvent.CallingReportOpenMediaCompleted : MediaStreamFsm.MediaEvent.CallingReportOpenMediaCompletedLive);
                this.ReportOpenMediaCompleted(attributes, (IEnumerable<MediaStreamDescription>)descriptions);
                this.State = canSeek ? TsMediaStreamSource.SourceState.Seek : TsMediaStreamSource.SourceState.Play;
            }), CancellationToken.None, TaskCreationOptions.None, (TaskScheduler)this._taskScheduler);
            TaskCollector.Default.Add(task, "TsMediaStreamSource CompleteConfigure");
        }

        protected override void SeekAsync(long seekToTime)
        {
            TimeSpan seekTimestamp = TimeSpan.FromTicks(seekToTime);
            Debug.WriteLine("TsMediaStreamSource.SeekAsync({0})", (object)seekTimestamp);
            this.ValidateEvent(MediaStreamFsm.MediaEvent.SeekAsyncCalled);
            this.StartSeek(seekTimestamp);
        }

        private void StartSeek(TimeSpan seekTimestamp)
        {
            lock (this._stateLock)
            {
                if (this._isClosed)
                    return;
                this._state = TsMediaStreamSource.SourceState.Seek;
                this._pendingSeekTarget = this._seekTarget ?? seekTimestamp;
            }
            this.RequestOperationAndSignal(TsMediaStreamSource.Operation.Seek);
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            this.RequestOperationAndSignal(TsMediaStreamSource.LookupOperation(mediaStreamType));
        }

        protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
        {
            Debug.WriteLine("TsMediaStreamSource.SwitchMediaStreamAsync()");
            throw new NotImplementedException();
        }

        protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            Debug.WriteLine("TsMediaStreamSource.GetDiagnosticAsync({0})", (object)diagnosticKind);
            throw new NotImplementedException();
        }

        protected override void CloseMedia()
        {
            Debug.WriteLine("TsMediaStreamSource.CloseMedia()");
            this.ValidateEvent(MediaStreamFsm.MediaEvent.CloseMediaCalled);
            lock (this._stateLock)
            {
                this._isClosed = true;
                this._state = TsMediaStreamSource.SourceState.Closed;
            }
            Task task = Task.Factory.StartNew(new Action(this.CloseMediaHandler), CancellationToken.None, TaskCreationOptions.None, (TaskScheduler)this._taskScheduler);
            TaskCollector.Default.Add(task, "TsMediaStreamSource CloseMedia");
        }

        private void CloseMediaHandler()
        {
            Debug.WriteLine("TsMediaStreamSource.CloseMediaHandler()");
            this._taskScheduler.ThrowIfNotOnThread();
            TaskCompletionSource<object> completionSource;
            lock (this._stateLock)
                completionSource = this._closeCompleted;
            if (null != completionSource)
                completionSource.TrySetResult((object)string.Empty);
            this.FireCloseMediaHandler();
        }

        private void FireCloseMediaHandler()
        {
            Task task = this._streamControl.CloseAsync(CancellationToken.None);
            TaskCollector.Default.Add(task, "TsMediaStreamSource CloseMediaHandler");
        }

        private static TsMediaStreamSource.Operation LookupOperation(MediaStreamType mediaStreamType)
        {
            switch (mediaStreamType)
            {
                case MediaStreamType.Audio:
                    return TsMediaStreamSource.Operation.Audio;
                case MediaStreamType.Video:
                    return TsMediaStreamSource.Operation.Video;
                default:
                    Debug.Assert(false);
                    return TsMediaStreamSource.Operation.None;
            }
        }

        private void RequestOperationAndSignal(TsMediaStreamSource.Operation operation)
        {
            if (!this.RequestOperation(operation))
                return;
            this._taskScheduler.Signal();
        }

        private bool RequestOperation(TsMediaStreamSource.Operation operation)
        {
            int num1 = (int)operation;
            int comparand = this._pendingOperations;
            while (true)
            {
                int num2 = comparand | num1;
                if (num2 != comparand)
                {
                    int num3 = Interlocked.CompareExchange(ref this._pendingOperations, num2, comparand);
                    if (num3 != comparand)
                        comparand = num3;
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return false;
            label_3:
            return true;
        }

        private TsMediaStreamSource.Operation HandleOperation(TsMediaStreamSource.Operation operation)
        {
            int num1 = (int)operation;
            int comparand = this._pendingOperations;
            while (true)
            {
                int num2 = comparand & ~num1;
                if (num2 != comparand)
                {
                    int num3 = Interlocked.CompareExchange(ref this._pendingOperations, num2, comparand);
                    if (num3 != comparand)
                        comparand = num3;
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return TsMediaStreamSource.Operation.None;
            label_3:
            return (TsMediaStreamSource.Operation)(comparand & num1);
        }

        [Flags]
        public enum Operation
        {
            None = 0,
            Audio = 1,
            Video = 2,
            Seek = 4,
        }

        private enum SourceState
        {
            Idle,
            Open,
            Seek,
            Play,
            Closed,
            WaitForClose,
        }
    }
}
