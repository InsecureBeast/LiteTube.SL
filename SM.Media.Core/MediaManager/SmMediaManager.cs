using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Buffering;
using SM.Media.Core.Content;
using SM.Media.Core.MediaParser;
using SM.Media.Core.Segments;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.Utility;

namespace SM.Media.Core.MediaManager
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
        MediaManagerState state = this.State;
        return state == MediaManagerState.Idle || MediaManagerState.Closed == state;
      }
    }

    private bool IsRunning
    {
      get
      {
        MediaManagerState state = this.State;
        return MediaManagerState.OpenMedia == state || MediaManagerState.Opening == state || MediaManagerState.Playing == state || MediaManagerState.Seeking == state;
      }
    }

    public MediaManagerState State
    {
      get
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          return this._mediaState;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
      private set
      {
        this.SetMediaState(value, (string) null);
      }
    }

    public TimeSpan? SeekTarget
    {
      get
      {
        return this._mediaStreamConfigurator.SeekTarget;
      }
      set
      {
        this._mediaStreamConfigurator.SeekTarget = value;
      }
    }

    public ContentType ContentType { get; set; }

    public Task PlayingTask
    {
      get
      {
        return (Task) this._playbackTaskCompletionSource.Task;
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
      this._segmentReaderManagerFactory = segmentReaderManagerFactory;
      this._mediaStreamConfigurator = mediaStreamConfigurator;
      this._bufferingManagerFactory = bufferingManagerFactory;
      this._mediaParserFactory = mediaParserFactory;
      this._programStreamsHandler = mediaManagerParameters.ProgramStreamsHandler;
      this._playbackCancellationTokenSource.Cancel();
      this._playbackTaskCompletionSource.TrySetResult((object) null);
      this._reportStateTask = new SignalTask(new Func<Task>(this.ReportState));
    }

    public void Dispose()
    {
      Debug.WriteLine("SmMediaManager.Dispose()");
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      if (null != this.OnStateChange)
      {
        Debug.WriteLine("SmMediaManager.Dispose(): OnStateChange is not null");
        if (Debugger.IsAttached)
          Debugger.Break();
        this.OnStateChange = (EventHandler<MediaManagerStateEventArgs>) null;
      }
      this._mediaStreamConfigurator.MediaManager = (IMediaManager) null;
      this.CloseAsync().Wait();
      using (this._reportStateTask)
      {
        using (this._asyncLock)
          ;
      }
      using (this._playbackCancellationTokenSource)
        ;
      using (this._closeCancellationTokenSource)
        ;
    }

    public async Task<IMediaStreamConfigurator> OpenMediaAsync(ICollection<Uri> source, CancellationToken cancellationToken)
    {
      Debug.WriteLine("SmMediaManager.OpenMediaAsync()");
      if (null == source)
        throw new ArgumentNullException("source");
      if (source.Count == 0 || Enumerable.Any<Uri>((IEnumerable<Uri>) source, (Func<Uri, bool>) (s => (Uri) null == s)))
        throw new ArgumentException("No valid URLs", "source");
      source = (ICollection<Uri>) Enumerable.ToArray<Uri>((IEnumerable<Uri>) source);
      IMediaStreamConfigurator streamConfigurator;
      using (await this._asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
      {
        if (!this.IsClosed)
          await this.CloseAsync().ConfigureAwait(false);
        this._playbackTaskCompletionSource = new TaskCompletionSource<object>();
        this.State = MediaManagerState.OpenMedia;
        await this.OpenAsync(source).ConfigureAwait(false);
        streamConfigurator = this._mediaStreamConfigurator;
      }
      return streamConfigurator;
    }

    public async Task StopMediaAsync(CancellationToken cancellationToken)
    {
      Debug.WriteLine("SmMediaManager.StopMediaAsync()");
      if (this.IsRunning)
      {
        using (await this._asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
          await this.CloseAsync().ConfigureAwait(false);
      }
    }

    public async Task CloseMediaAsync()
    {
      Debug.WriteLine("SmMediaManager.CloseMediaAsync()");
      if (!this.IsClosed)
      {
        try
        {
          using (await this._asyncLock.LockAsync(CancellationToken.None).ConfigureAwait(false))
            await this.CloseAsync().ConfigureAwait(false);
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
      Debug.WriteLine("SmMediaManager.SeekMediaAsync({0})", (object) position);
      TimeSpan timeSpan;
      using (await this._asyncLock.LockAsync(this._closeCancellationTokenSource.Token).ConfigureAwait(false))
        timeSpan = await this.SeekAsync(position).ConfigureAwait(false);
      return timeSpan;
    }

    private async Task CloseAsync()
    {
      Debug.WriteLine("SmMediaManager.CloseAsync()");
      TaskCompletionSource<object> closeTaskCompletionSource = new TaskCompletionSource<object>();
      TaskCompletionSource<object> currentCloseTaskCompletionSource = Interlocked.CompareExchange<TaskCompletionSource<object>>(ref this._closeTaskCompletionSource, closeTaskCompletionSource, (TaskCompletionSource<object>) null);
      if (null != currentCloseTaskCompletionSource)
      {
        object obj = await currentCloseTaskCompletionSource.Task.ConfigureAwait(false);
        Debug.WriteLine("SmMediaManager.CloseAsync() completed by other caller");
      }
      else
      {
        this.State = MediaManagerState.Closing;
        TaskCompletionSource<object> playbackTaskCompletionSource = this._playbackTaskCompletionSource;
        this._closeCancellationTokenSource.Cancel();
        ConfiguredTaskAwaitable configuredTaskAwaitable = this.CloseCleanupAsync().ConfigureAwait(false);
        await configuredTaskAwaitable;
        this.State = MediaManagerState.Closed;
        configuredTaskAwaitable = this._reportStateTask.WaitAsync().ConfigureAwait(false);
        await configuredTaskAwaitable;
        Debug.WriteLine("SmMediaManager.CloseAsync() completed");
        Interlocked.CompareExchange<TaskCompletionSource<object>>(ref this._closeTaskCompletionSource, (TaskCompletionSource<object>) null, closeTaskCompletionSource);
        Task task = TaskEx.Run((Action) (() =>
        {
          closeTaskCompletionSource.TrySetResult((object) null);
          playbackTaskCompletionSource.TrySetResult((object) null);
        }));
        TaskCollector.Default.Add(task, "SmMediaManager close");
      }
    }

    private async Task CloseCleanupAsync()
    {
      Debug.WriteLine("SmMediaManager.CloseCleanupAsync()");
      List<Task> tasks = new List<Task>();
      ISegmentReaderManager readerManager = this._readerManager;
      if (null != readerManager)
      {
        this._readerManager = (ISegmentReaderManager) null;
        tasks.Add(readerManager.StopAsync());
      }
      IMediaStreamConfigurator msc = this._mediaStreamConfigurator;
      if (null != msc)
        tasks.Add(msc.CloseAsync());
      if (this._readers != null && this._readers.Length > 0)
        tasks.Add(this.CloseReadersAsync());
      if (null != this._playTask)
        tasks.Add(this._playTask);
      if (tasks.Count > 0)
      {
        while (Enumerable.Any<Task>((IEnumerable<Task>) tasks, (Func<Task, bool>) (t => !t.IsCompleted)))
        {
          try
          {
            Task t = TaskEx.Delay(2500);
            Debug.WriteLine("SmMediaManager.CloseCleanupAsync() waiting for tasks");
            Task task = await TaskEx.WhenAny(t, TaskEx.WhenAll((IEnumerable<Task>) tasks)).ConfigureAwait(false);
            Debug.WriteLine("SmMediaManager.CloseCleanupAsync() finished tasks");
          }
          catch (Exception ex)
          {
            Debug.WriteLine("SmMediaManager.CloseCleanupAsync() play task failed: " + ExceptionExtensions.ExtendedMessage(ex));
          }
        }
      }
      if (null != msc)
        msc.MediaManager = (IMediaManager) null;
      this.DisposeReaders();
      if (null != readerManager)
        DisposeExtensions.DisposeSafe((IDisposable) readerManager);
    }

    private Task ReportState()
    {
      Debug.WriteLine("SmMediaManager.ReportState() state {0} message {1}", (object) this._mediaState, (object) this._mediaStateMessage);
      bool lockTaken = false;
      object obj = null;
      MediaManagerState state;
      string message;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        state = this._mediaState;
        message = this._mediaStateMessage;
        this._mediaStateMessage = (string) null;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      EventHandler<MediaManagerStateEventArgs> eventHandler = this.OnStateChange;
      if (null != eventHandler)
        eventHandler((object) this, new MediaManagerStateEventArgs(state, message));
      if (null != message)
      {
        IMediaStreamConfigurator streamConfigurator = this._mediaStreamConfigurator;
        if (null != streamConfigurator)
          streamConfigurator.ReportError(message);
      }
      return TplTaskExtensions.CompletedTask;
    }

    private void ResetCancellationToken()
    {
      Debug.WriteLine("SmMediaManager.ResetCancellationToken()");
      if (this._closeCancellationTokenSource.IsCancellationRequested)
      {
        CancellationTokenExtensions.CancelDisposeSafe(this._closeCancellationTokenSource);
        this._closeCancellationTokenSource = new CancellationTokenSource();
      }
      if (!this._playbackCancellationTokenSource.IsCancellationRequested)
        return;
      CancellationTokenExtensions.CancelDisposeSafe(this._playbackCancellationTokenSource);
      this._playbackCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._closeCancellationTokenSource.Token);
    }

    private void SetMediaState(MediaManagerState state, string message)
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (state == this._mediaState)
          return;
        Debug.WriteLine("SmMediaManager.SetMediaState() {0} -> {1}", (object) this._mediaState, (object) state);
        this._mediaState = state;
        if (null != message)
          this._mediaStateMessage = message;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      this._reportStateTask.Fire();
    }

    private void StartReaders()
    {
      CancellationToken token = this._playbackCancellationTokenSource.Token;
      Task<Task> task = TaskEx.WhenAll(Enumerable.Select<IMediaReader, Task>((IEnumerable<IMediaReader>) this._readers, (Func<IMediaReader, Task>) (r => (Task) r.ReadAsync(token)))).ContinueWith<Task>((Func<Task, Task>) (async t =>
      {
        AggregateException ex = t.Exception;
        if (null != ex)
        {
          Debug.WriteLine("SmMediaManager.StartReaders() ReadAsync failed: " + ExceptionExtensions.ExtendedMessage((Exception) ex));
          this.SetMediaState(MediaManagerState.Error, ExceptionExtensions.ExtendedMessage((Exception) ex));
          bool lockTaken = false;
          object obj = null;
          try
          {
            Monitor.Enter(obj = this._lock, ref lockTaken);
            if (null != this._closeTaskCompletionSource)
              goto label_10;
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit(obj);
          }
          try
          {
            await this.CloseMediaAsync().ConfigureAwait(false);
          }
          catch (Exception ex1)
          {
            Debug.WriteLine("SmMediaManager.StartReaders() ReadAsync close media failed " + (object) ex1);
          }
        }
label_10:;
      }), token, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
      TaskCollector.Default.Add((Task) task, "SmMediaManager.StartReaders() cleanup tasks");
    }

    private async Task OpenAsync(ICollection<Uri> source)
    {
      Debug.WriteLine("SmMediaManager.OpenAsync() state " + (object) this.State);
      this.State = MediaManagerState.Opening;
      ++this._openCount;
      this.ResetCancellationToken();
      Task<IMediaReader>[] readerTasks = (Task<IMediaReader>[]) null;
      Exception exception;
      try
      {
        this._mediaStreamConfigurator.Initialize();
        ((SmMediaManager) this)._readerManager = await this._segmentReaderManagerFactory.CreateAsync((ISegmentManagerParameters) new SegmentManagerParameters()
        {
          Source = source
        }, this.ContentType, this._playbackCancellationTokenSource.Token).ConfigureAwait(false);
        if (null == this._readerManager)
        {
          Debug.WriteLine("SmMediaManager.OpenAsync() unable to create reader manager");
          this.SetMediaState(MediaManagerState.Error, "Unable to create reader");
          goto label_15;
        }
        else
        {
          readerTasks = Enumerable.ToArray<Task<IMediaReader>>(Enumerable.Select<ISegmentManagerReaders, Task<IMediaReader>>((IEnumerable<ISegmentManagerReaders>) this._readerManager.SegmentManagerReaders, new Func<ISegmentManagerReaders, Task<IMediaReader>>(this.CreateReaderPipeline)));
          this._readers = await TaskEx.WhenAll<IMediaReader>((IEnumerable<Task<IMediaReader>>) readerTasks).ConfigureAwait(false);
          foreach (IMediaReader mediaReader in this._readers)
            mediaReader.IsEnabled = true;
          //bool flag;
          //if (!flag)
          //  ;

          TimeSpan timeSpan = await SegmentReaderManagerExtensions.StartAsync(this._readerManager, this._playbackCancellationTokenSource.Token).ConfigureAwait(false);
          this._mediaStreamConfigurator.MediaManager = (IMediaManager) this;
          this.StartReaders();
          goto label_15;
        }
      }
      catch (OperationCanceledException ex)
      {
        this.SetMediaState(MediaManagerState.Error, "Media play canceled");
        exception = (Exception) ex;
      }
      catch (Exception ex)
      {
        Debug.WriteLine("SmMediaManager.OpenAsync() failed: " + ex.Message);
        this.SetMediaState(MediaManagerState.Error, "Unable to play media");
        exception = (Exception) new AggregateException(ex.Message, ex);
      }
      await this.CleanupFailedOpenAsync(readerTasks);
      throw exception;
label_15:;
    }

    private async Task CleanupFailedOpenAsync(Task<IMediaReader>[] readerTasks)
    {
      Debug.WriteLine("SmMediaManager.CleanupFailedOpenAsync() state " + (object) this.State);
      this._playbackCancellationTokenSource.Cancel();
      if (this._readers == null && null != readerTasks)
      {
        this._readers = Enumerable.ToArray<IMediaReader>(Enumerable.Select<Task<IMediaReader>, IMediaReader>(Enumerable.Where<Task<IMediaReader>>((IEnumerable<Task<IMediaReader>>) readerTasks, (Func<Task<IMediaReader>, bool>) (r =>
        {
          if (null == r)
            return false;
          AggregateException exception = r.Exception;
          if (null == exception)
            return r.IsCompleted;
          Debug.WriteLine("SmMediaManager.CleanupFailedOpenAsync(): reader create failed: " + exception.Message);
          return false;
        })), (Func<Task<IMediaReader>, IMediaReader>) (r => r.Result)));
        await this.CloseReadersAsync().ConfigureAwait(false);
        this.DisposeReaders();
      }
      if (null != this._readerManager)
      {
        DisposeExtensions.DisposeSafe((IDisposable) this._readerManager);
        this._readerManager = (ISegmentReaderManager) null;
      }
    }

    private async Task<IMediaReader> CreateReaderPipeline(ISegmentManagerReaders segmentManagerReaders)
    {
      MediaReader reader = new MediaReader(this._bufferingManagerFactory(), this._mediaParserFactory, segmentManagerReaders, (IBlockingPool<WorkBuffer>) new WorkBufferBlockingPool(8));
      await reader.InitializeAsync(segmentManagerReaders, new Action(this.CheckConfigurationCompleted), new Action(this._mediaStreamConfigurator.CheckForSamples), this._playbackCancellationTokenSource.Token, this._programStreamsHandler).ConfigureAwait(false);
      return (IMediaReader) reader;
    }

    private void CheckConfigurationCompleted()
    {
      MediaManagerState state = this.State;
      if (MediaManagerState.Opening != state && MediaManagerState.OpenMedia != state || (this._readers == null || Enumerable.Any<IMediaReader>((IEnumerable<IMediaReader>) this._readers, (Func<IMediaReader, bool>) (r => !r.IsConfigured))))
        return;
      this._playTask = MediaStreamSourceExtensions.PlayAsync(this._mediaStreamConfigurator, Enumerable.SelectMany<IMediaReader, IMediaParserMediaStream>((IEnumerable<IMediaReader>) this._readers, (Func<IMediaReader, IEnumerable<IMediaParserMediaStream>>) (r => (IEnumerable<IMediaParserMediaStream>) r.MediaStreams)), this._readerManager.Duration, this._closeCancellationTokenSource.Token);
      this.State = MediaManagerState.Playing;
      int openCount = this._openCount;
      this._playTask.ContinueWith<Task>((Func<Task, Task>) (async t =>
      {
        AggregateException taskException = t.Exception;
        if (null != taskException)
          Debug.WriteLine("SmMediaManager.CheckConfigurationComplete() play task failed: " + taskException.Message);
        try
        {
          using (await this._asyncLock.LockAsync(CancellationToken.None).ConfigureAwait(false))
          {
            if (openCount == this._openCount)
              await this.CloseAsync().ConfigureAwait(false);
            else
              goto label_11;
          }
        }
        catch (Exception ex)
        {
          Debug.WriteLine("SmMediaManager.CheckConfigurationComplete() play continuation failed: " + ex.Message);
        }
label_11:;
      }));
    }

    private async Task CloseReadersAsync()
    {
      Debug.WriteLine("SmMediaManager.CloseReadersAsync() closing readers");
      if (this._readers == null || this._readers.Length < 1)
      {
        Debug.WriteLine("SmMediaManager.CloseReadersAsync() no readers");
      }
      else
      {
        try
        {
          IEnumerable<Task> tasks = Enumerable.Where<Task>(Enumerable.Select<IMediaReader, Task>((IEnumerable<IMediaReader>) this._readers, (Func<IMediaReader, Task>) (async reader =>
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
          })), (Func<Task, bool>) (t => null != t));
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
      IMediaReader[] mediaReaderArray = this._readers;
      this._readers = (IMediaReader[]) null;
      if (mediaReaderArray == null || mediaReaderArray.Length < 1)
        return;
      foreach (IDisposable disposable in mediaReaderArray)
        DisposeExtensions.DisposeBackground(disposable, "SmMediaManager dispose reader");
      Debug.WriteLine("SmMediaManager.DisposeReaders() completed");
    }

    private bool IsSeekInRange(TimeSpan position)
    {
      return Enumerable.All<IMediaReader>((IEnumerable<IMediaReader>) this._readers, (Func<IMediaReader, bool>) (reader => reader.IsBuffered(position)));
    }

    private async Task<TimeSpan> SeekAsync(TimeSpan position)
    {
      Debug.WriteLine("SmMediaManager.SeekAsync()");
      TimeSpan timeSpan;
      if (this._playbackCancellationTokenSource.IsCancellationRequested)
      {
        timeSpan = TimeSpan.MinValue;
      }
      else
      {
        try
        {
          if (this.IsSeekInRange(position))
          {
            timeSpan = position;
            goto label_16;
          }
          else
          {
            IMediaReader[] readers = this._readers;
            if (readers == null || readers.Length < 1)
            {
              timeSpan = TimeSpan.MinValue;
              goto label_16;
            }
            else
            {
              await TaskEx.WhenAll(Enumerable.Select<IMediaReader, Task>((IEnumerable<IMediaReader>) readers, (Func<IMediaReader, Task>) (reader => reader.StopAsync()))).ConfigureAwait(false);
              if (this._playbackCancellationTokenSource.IsCancellationRequested)
              {
                timeSpan = TimeSpan.MinValue;
                goto label_16;
              }
              else
              {
                foreach (IMediaReader mediaReader in readers)
                  mediaReader.IsEnabled = true;
                //bool flag;
                //if (!flag)
                //  ;
                this.State = MediaManagerState.Seeking;
                TimeSpan actualPosition = await this._readerManager.SeekAsync(position, this._playbackCancellationTokenSource.Token).ConfigureAwait(false);
                this.StartReaders();
                timeSpan = actualPosition;
                goto label_16;
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
label_16:
      return timeSpan;
    }
  }
}
