using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Buffering;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Segments;
using LiteTube.StreamVideo.TransportStream.TsParser;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.MediaManager
{
  internal sealed class MediaReader : IMediaReader, IDisposable
  {
    private readonly IMediaParserFactory _mediaParserFactory;
    private IBlockingPool<WorkBuffer> _blockingPool;
    private IBufferingManager _bufferingManager;
    private CallbackReader _callbackReader;
    private Action _checkConfiguration;
    private int _isDisposed;
    private bool _isEnabled;
    private IMediaParser _mediaParser;
    private QueueWorker<WorkBuffer> _queueWorker;
    private ISegmentManagerReaders _segmentReaders;

    public bool IsConfigured { get; private set; }

    public bool IsEnabled
    {
      get
      {
        return this._isEnabled;
      }
      set
      {
        this._isEnabled = value;
        this._mediaParser.EnableProcessing = value;
        this._queueWorker.IsEnabled = value;
      }
    }

    public ICollection<IMediaParserMediaStream> MediaStreams
    {
      get
      {
        if (!this.IsConfigured)
          throw new InvalidOperationException("MediaStreams are not available until after IsConfigured is true.");
        return this._mediaParser.MediaStreams;
      }
    }

    public MediaReader(IBufferingManager bufferingManager, IMediaParserFactory mediaParserFactory, ISegmentManagerReaders segmentReaders, IBlockingPool<WorkBuffer> blockingPool)
    {
      if (null == bufferingManager)
        throw new ArgumentNullException("bufferingManager");
      if (null == mediaParserFactory)
        throw new ArgumentNullException("mediaParserFactory");
      this._bufferingManager = bufferingManager;
      this._mediaParserFactory = mediaParserFactory;
      this._blockingPool = blockingPool;
      this._segmentReaders = segmentReaders;
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      using (this._callbackReader)
        ;
      using (this._queueWorker)
        ;
      using (this._blockingPool)
        ;
      using (this._mediaParser)
        ;
      this._callbackReader = (CallbackReader) null;
      this._queueWorker = (QueueWorker<WorkBuffer>) null;
      this._blockingPool = (IBlockingPool<WorkBuffer>) null;
      this._mediaParser = (IMediaParser) null;
      this._bufferingManager = (IBufferingManager) null;
      this._segmentReaders = (ISegmentManagerReaders) null;
    }

    public Task<long> ReadAsync(CancellationToken cancellationToken)
    {
      this._mediaParser.StartPosition = this._segmentReaders.Manager.StartPosition;
      this._bufferingManager.Flush();
      return this._callbackReader.ReadAsync(cancellationToken);
    }

    public async Task CloseAsync()
    {
      this._queueWorker.IsEnabled = false;
      try
      {
        await this.StopReadingAsync().ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        Debug.WriteLine("MediaReader.CloseAsync(): stop reading failed: " + ex.Message);
      }
      QueueWorker<WorkBuffer> queue = this._queueWorker;
      if (null != queue)
      {
        try
        {
          await queue.CloseAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("MediaReader.CloseAsync(): queue close failed: " + ex.Message);
        }
      }
      this.FlushBuffers();
      if (null != this._mediaParser)
        this._mediaParser.ProcessEndOfData();
      if (this._bufferingManager != null && null != queue)
        this._bufferingManager.Shutdown((IQueueThrottling) queue);
    }

    public async Task StopAsync()
    {
      this._queueWorker.IsEnabled = false;
      ConfiguredTaskAwaitable configuredTaskAwaitable = this.StopReadingAsync().ConfigureAwait(false);
      await configuredTaskAwaitable;
      QueueWorker<WorkBuffer> queue = this._queueWorker;
      if (null != queue)
      {
        try
        {
          configuredTaskAwaitable = queue.ClearAsync().ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
        catch (Exception ex)
        {
          Debug.WriteLine("MediaReader.StopAsync(): queue clear failed: " + ex.Message);
        }
      }
      this.FlushBuffers();
    }

    public bool IsBuffered(TimeSpan position)
    {
      return this._bufferingManager.IsSeekAlreadyBuffered(position);
    }

    public async Task InitializeAsync(ISegmentManagerReaders segmentManagerReaders, Action checkConfiguration, Action checkForSamples, CancellationToken cancellationToken, Action<IProgramStreams> programStreamsHandler)
    {
      this._checkConfiguration = checkConfiguration;
      Task startReaderTask = this._segmentReaders.Manager.StartAsync();
      MediaReader localReader = this;
      QueueWorker<WorkBuffer> queueWorker = new QueueWorker<WorkBuffer>((Action<WorkBuffer>) (wi =>
      {
        IMediaParser mediaParser = localReader._mediaParser;
        if (null == wi)
        {
          mediaParser.ProcessEndOfData();
        }
        else
        {
          if (null != wi.Metadata)
          {
            mediaParser.StartSegment(wi.Metadata);
            wi.Metadata = (ISegmentMetadata) null;
          }
          mediaParser.ProcessData(wi.Buffer, 0, wi.Length);
        }
      }), (Action<WorkBuffer>) (buffer => this._blockingPool.Free(buffer)));
      this._queueWorker = queueWorker;
      this._callbackReader = new CallbackReader(segmentManagerReaders.Readers, new Action<WorkBuffer>(queueWorker.Enqueue), this._blockingPool);
      this._bufferingManager.Initialize((IQueueThrottling) queueWorker, checkForSamples);
      try
      {
        await startReaderTask.ConfigureAwait(false);
        ContentType contentType = this._segmentReaders.Manager.ContentType;
        if ((ContentType) null == contentType)
        {
          Debug.WriteLine("MediaReader.CreateReaderPipeline() unable to determine content type, defaulting to transport stream");
          contentType = ContentTypes.TransportStream;
        }
        else if (ContentTypes.Binary == contentType)
        {
          Debug.WriteLine("MediaReader.CreateReaderPipeline() detected binary content, defaulting to transport stream");
          contentType = ContentTypes.TransportStream;
        }
        MediaParserParameters mediaParserParameters = new MediaParserParameters();
        this._mediaParser = await this._mediaParserFactory.CreateAsync((IMediaParserParameters) mediaParserParameters, contentType, cancellationToken).ConfigureAwait(false);
        if (null == this._mediaParser)
          throw new NotSupportedException("Unsupported content type: " + (object) contentType);
        this._mediaParser.ConfigurationComplete += new EventHandler(this.ConfigurationComplete);
        this._mediaParser.Initialize(this._bufferingManager, programStreamsHandler);
        this._mediaParser.InitializeStream(this._segmentReaders.Manager.StreamMetadata);
      }
      catch (Exception ex)
      {
        this._bufferingManager.Shutdown((IQueueThrottling) queueWorker);
        throw;
      }
    }

    private void ConfigurationComplete(object sender, EventArgs eventArgs)
    {
      this._mediaParser.ConfigurationComplete -= new EventHandler(this.ConfigurationComplete);
      this.IsConfigured = true;
      this._checkConfiguration();
    }

    private async Task StopReadingAsync()
    {
      CallbackReader callbackReader = this._callbackReader;
      if (null != callbackReader)
      {
        try
        {
          await callbackReader.StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          Debug.WriteLine("MediaReader.StopReadingAsync(): callback reader stop failed: " + ex.Message);
        }
      }
    }

    private void FlushBuffers()
    {
      if (null != this._mediaParser)
      {
        this._mediaParser.EnableProcessing = false;
        this._mediaParser.FlushBuffers();
      }
      if (null == this._bufferingManager)
        return;
      this._bufferingManager.Flush();
    }
  }
}
