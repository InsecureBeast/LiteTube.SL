using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Buffering;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;
using WebResponse = LiteTube.StreamVideo.Web.WebResponse;

namespace LiteTube.StreamVideo.MediaManager
{
  public class SingleStreamMediaManager : IMediaManager, IDisposable
  {
    private readonly object _lock = new object();
    private readonly Func<IBufferingManager> _bufferingManagerFactory;
    private readonly IMediaParserFactory _mediaParserFactory;
    private readonly IMediaStreamConfigurator _mediaStreamConfigurator;
    private readonly SignalTask _reportStateTask;
    private readonly IWebMetadataFactory _webMetadataFactory;
    private readonly IWebReaderManager _webReaderManager;
    private int _isDisposed;
    private MediaManagerState _mediaState;
    private string _mediaStateMessage;
    private CancellationTokenSource _playCancellationTokenSource;
    private Task _playTask;

    public TimeSpan? SeekTarget { get; set; }

    public ContentType ContentType { get; set; }

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

    public Task PlayingTask
    {
      get
      {
        Task task = (Task) null;
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          task = this._playTask;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
        return task ?? TplTaskExtensions.CompletedTask;
      }
    }

    public event EventHandler<MediaManagerStateEventArgs> OnStateChange;

    public SingleStreamMediaManager(Func<IBufferingManager> bufferingManagerFactory, IMediaParserFactory mediaParserFactory, IMediaStreamConfigurator mediaStreamConfigurator, IWebMetadataFactory webMetadataFactory, IWebReaderManager webReaderManager)
    {
      if (null == bufferingManagerFactory)
        throw new ArgumentNullException("bufferingManagerFactory");
      if (null == mediaParserFactory)
        throw new ArgumentNullException("mediaParserFactory");
      if (null == mediaStreamConfigurator)
        throw new ArgumentNullException("mediaStreamConfigurator");
      if (null == webMetadataFactory)
        throw new ArgumentNullException("webMetadataFactory");
      if (null == webReaderManager)
        throw new ArgumentNullException("webReaderManager");
      this._bufferingManagerFactory = bufferingManagerFactory;
      this._mediaParserFactory = mediaParserFactory;
      this._mediaStreamConfigurator = mediaStreamConfigurator;
      this._webMetadataFactory = webMetadataFactory;
      this._webReaderManager = webReaderManager;
      this._reportStateTask = new SignalTask(new Func<Task>(this.ReportState));
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public async Task<IMediaStreamConfigurator> OpenMediaAsync(ICollection<Uri> source, CancellationToken cancellationToken)
    {
      this.State = MediaManagerState.OpenMedia;
      WebResponse response = new WebResponse();
      IMediaStreamConfigurator streamConfigurator;
      using (IWebReader rootReader = WebReaderManagerExtensions.CreateRootReader(this._webReaderManager, (ContentType) null))
      {
        using (IEnumerator<Uri> enumerator = source.GetEnumerator())
        {
label_25:
          if (enumerator.MoveNext())
          {
            Uri url = enumerator.Current;
            IWebReader webReader = (IWebReader) null;
            IWebStreamResponse webStream = (IWebStreamResponse) null;
            CancellationTokenSource playCancellationTokenSource = (CancellationTokenSource) null;
            Task playTask = (Task) null;
            try
            {
              webReader = WebReaderExtensions.CreateChild(rootReader, url, ContentKind.Unknown, this.ContentType);
              webStream = await webReader.GetWebStreamAsync((Uri) null, false, cancellationToken, (Uri) null, new long?(), new long?(), response).ConfigureAwait(false);
              if (webStream.IsSuccessStatusCode)
              {
                ContentType contentType = response.ContentType;
                if (!((ContentType) null == contentType))
                {
                  if ((ContentType) null == contentType || ContentKind.Playlist == contentType.Kind)
                    throw new FileNotFoundException("Content not supported with this media manager");
                  TaskCompletionSource<bool> configurationTaskCompletionSource = new TaskCompletionSource<bool>();
                  playCancellationTokenSource = new CancellationTokenSource();
                  CancellationTokenSource localPlayCancellationTokenSource = playCancellationTokenSource;
                  Task cancelPlayTask = configurationTaskCompletionSource.Task.ContinueWith((Action<Task<bool>>) (t =>
                  {
                    if (!t.IsFaulted && !t.IsCanceled)
                      return;
                    localPlayCancellationTokenSource.Cancel();
                  }));
                  TaskCollector.Default.Add(cancelPlayTask, "SingleStreamMediaManager play cancellation");
                  IWebReader localWebReader = webReader;
                  IWebStreamResponse localWebStream = webStream;
                  CancellationToken playCancellationToken = playCancellationTokenSource.Token;
                  playTask = TaskEx.Run((Func<Task>) (() => this.SimplePlayAsync(contentType, localWebReader, localWebStream, response, configurationTaskCompletionSource, playCancellationToken)), playCancellationToken);
                  bool lockTaken = false;
                  object obj = null;
                  try
                  {
                    Monitor.Enter(obj = this._lock, ref lockTaken);
                    this._playCancellationTokenSource = playCancellationTokenSource;
                    playCancellationTokenSource = (CancellationTokenSource) null;
                    this._playTask = playTask;
                    playTask = (Task) null;
                  }
                  finally
                  {
                    if (lockTaken)
                      Monitor.Exit(obj);
                  }
                  bool isConfigured = await configurationTaskCompletionSource.Task.ConfigureAwait(false);
                  if (isConfigured)
                  {
                    webReader = (IWebReader) null;
                    webStream = (IWebStreamResponse) null;
                    streamConfigurator = this._mediaStreamConfigurator;
                    goto label_33;
                  }
                  else
                    goto label_25;
                }
                else
                  goto label_25;
              }
              else
                goto label_25;
            }
            finally
            {
              if (null != webStream)
                webStream.Dispose();
              if (null != webReader)
                webReader.Dispose();
              if (null != playCancellationTokenSource)
                playCancellationTokenSource.Cancel();
              if (null != playTask)
                TaskCollector.Default.Add(playTask, "SingleStreamMediaManager play task");
            }
          }
        }
      }
      throw new FileNotFoundException();
label_33:
      return streamConfigurator;
    }

    public Task StopMediaAsync(CancellationToken cancellationToken)
    {
      bool lockTaken = false;
      object obj = null;
      Task task;
      CancellationTokenSource cancellationTokenSource;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        task = this._playTask;
        cancellationTokenSource = this._playCancellationTokenSource;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null != cancellationTokenSource)
        cancellationTokenSource.Cancel();
      return task ?? TplTaskExtensions.CompletedTask;
    }

    public Task CloseMediaAsync()
    {
      return this.StopMediaAsync(CancellationToken.None);
    }

    public Task<TimeSpan> SeekMediaAsync(TimeSpan position)
    {
      return TaskEx.FromResult<TimeSpan>(position);
    }

    protected virtual void Dispose(bool disposing)
    {
      Debug.WriteLine("SingleStreamMediaManager.Dispose(bool)");
      if (!disposing)
        return;
      if (null != this.OnStateChange)
      {
        Debug.WriteLine("SingleStreamMediaManager.Dispose(bool): OnStateChange is not null");
        if (Debugger.IsAttached)
          Debugger.Break();
        this.OnStateChange = (EventHandler<MediaManagerStateEventArgs>) null;
      }
      this._mediaStreamConfigurator.MediaManager = (IMediaManager) null;
      this._reportStateTask.Dispose();
      bool lockTaken = false;
      object obj = null;
      CancellationTokenSource cancellationTokenSource;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        cancellationTokenSource = this._playCancellationTokenSource;
        this._playCancellationTokenSource = (CancellationTokenSource) null;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null == cancellationTokenSource)
        return;
      cancellationTokenSource.Dispose();
    }

    private Task ReportState()
    {
      Debug.WriteLine("SingleStreamMediaManager.ReportState() state {0} message {1}", (object) this._mediaState, (object) this._mediaStateMessage);
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

    private void SetMediaState(MediaManagerState state, string message)
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (state == this._mediaState)
          return;
        Debug.WriteLine("SingleStreamMediaState.SetMediaState() {0} -> {1}", (object) this._mediaState, (object) state);
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

    private void CancelPlayback()
    {
      bool lockTaken = false;
      object obj = null;
      CancellationTokenSource cancellationTokenSource;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        cancellationTokenSource = this._playCancellationTokenSource;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (null == cancellationTokenSource || cancellationTokenSource.IsCancellationRequested)
        return;
      cancellationTokenSource.Cancel();
    }

    private async Task SimplePlayAsync(ContentType contentType, IWebReader webReader, IWebStreamResponse webStreamResponse, WebResponse webResponse, TaskCompletionSource<bool> configurationTaskCompletionSource, CancellationToken cancellationToken)
    {
      try
      {
        this._mediaStreamConfigurator.Initialize();
        this._mediaStreamConfigurator.MediaManager = (IMediaManager) this;
        IMediaParser mediaParser = await this._mediaParserFactory.CreateAsync((IMediaParserParameters) new MediaParserParameters(), contentType, cancellationToken).ConfigureAwait(false);
        if (null == mediaParser)
          throw new NotSupportedException("Unsupported content type: " + (object) contentType);
        this.State = MediaManagerState.Opening;
        EventHandler configurationComplete = (EventHandler) null;
        configurationComplete = (EventHandler) ((sender, args) =>
        {
          mediaParser.ConfigurationComplete -= configurationComplete;
          configurationTaskCompletionSource.TrySetResult(true);
        });
        mediaParser.ConfigurationComplete += configurationComplete;
        using (IBufferingManager bufferingManager = this._bufferingManagerFactory())
        {
          SingleStreamMediaManager.QueueThrottle throttle = new SingleStreamMediaManager.QueueThrottle();
          bufferingManager.Initialize((IQueueThrottling) throttle, new Action(this._mediaStreamConfigurator.CheckForSamples));
          mediaParser.Initialize(bufferingManager, (Action<IProgramStreams>) null);
          IStreamMetadata streamMetadata = this._webMetadataFactory.CreateStreamMetadata(webResponse, (ContentType) null);
          mediaParser.InitializeStream(streamMetadata);
          Task reader = (Task) null;
          try
          {
            using (webReader)
            {
              try
              {
                if (null == webStreamResponse)
                  webStreamResponse = await webReader.GetWebStreamAsync((Uri) null, false, cancellationToken, (Uri) null, new long?(), new long?(), webResponse).ConfigureAwait(false);
                reader = this.ReadResponseAsync(mediaParser, webStreamResponse, webResponse, throttle, cancellationToken);
                Task task = await TaskEx.WhenAny((Task) configurationTaskCompletionSource.Task, CancellationTokenExtensions.AsTask(cancellationToken)).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                await MediaStreamSourceExtensions.PlayAsync(this._mediaStreamConfigurator, (IEnumerable<IMediaParserMediaStream>) mediaParser.MediaStreams, new TimeSpan?(), cancellationToken).ConfigureAwait(false);
                this.State = MediaManagerState.Playing;
                await reader.ConfigureAwait(false);
                reader = (Task) null;
              }
              finally
              {
                if (null != webStreamResponse)
                  webStreamResponse.Dispose();
              }
            }
          }
          catch (OperationCanceledException ex)
          {
          }
          catch (Exception ex)
          {
            string message = ExceptionExtensions.ExtendedMessage(ex);
            Debug.WriteLine("SingleStreamMediaManager.SimplePlayAsync() failed: " + message);
            this.SetMediaState(MediaManagerState.Error, message);
          }
          this.State = MediaManagerState.Closing;
          if (null != reader)
          {
            try
            {
              await reader.ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
            }
            catch (Exception ex)
            {
              Debug.WriteLine("SingleStreamMediaManager.SimplePlayAsync() reader failed: " + ExceptionExtensions.ExtendedMessage(ex));
            }
          }
          mediaParser.ConfigurationComplete -= configurationComplete;
          mediaParser.EnableProcessing = false;
          mediaParser.FlushBuffers();
          bufferingManager.Flush();
          bufferingManager.Shutdown((IQueueThrottling) throttle);
          await this._mediaStreamConfigurator.CloseAsync().ConfigureAwait(false);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("SingleStreamMediaManager.SimplePlayAsync() cleanup failed: " + ExceptionExtensions.ExtendedMessage(ex));
      }
      this._mediaStreamConfigurator.MediaManager = (IMediaManager) null;
      if (!configurationTaskCompletionSource.Task.IsCompleted)
        configurationTaskCompletionSource.TrySetCanceled();
      this.State = MediaManagerState.Closed;
      await this._reportStateTask.WaitAsync().ConfigureAwait(false);
    }

    private async Task ReadResponseAsync(IMediaParser mediaParser, IWebStreamResponse webStreamResponse, WebResponse webResponse, SingleStreamMediaManager.QueueThrottle throttle, CancellationToken cancellationToken)
    {
      byte[] buffer = new byte[16384];
      Task cancellationTask = CancellationTokenExtensions.AsTask(cancellationToken);
      try
      {
        ISegmentMetadata segmentMetadata = this._webMetadataFactory.CreateSegmentMetadata(webResponse, (ContentType) null);
        mediaParser.StartSegment(segmentMetadata);
        using (Stream stream = await webStreamResponse.GetStreamAsync(cancellationToken).ConfigureAwait(false))
        {
          while (true)
          {
            Task waitTask = throttle.WaitAsync();
            if (!waitTask.IsCompleted)
            {
              Task task = await TaskEx.WhenAny(waitTask, cancellationTask).ConfigureAwait(false);
              cancellationToken.ThrowIfCancellationRequested();
            }
            int length = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            if (length > 0)
              mediaParser.ProcessData(buffer, 0, length);
            else
              break;
          }
        }
      }
      finally
      {
        mediaParser.ProcessEndOfData();
      }
    }

    private sealed class QueueThrottle : IQueueThrottling
    {
      private readonly AsyncManualResetEvent _event = new AsyncManualResetEvent(false);

      public void Pause()
      {
        this._event.Reset();
      }

      public void Resume()
      {
        this._event.Set();
      }

      public Task WaitAsync()
      {
        return this._event.WaitAsync();
      }
    }
  }
}
