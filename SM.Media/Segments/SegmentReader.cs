// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SegmentReader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Content;
using SM.Media.Metadata;
using SM.Media.Utility;
using SM.Media.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
{
  internal sealed class SegmentReader : ISegmentReader, IDisposable
  {
    private readonly CancellationTokenSource _disposedCancellationTokenSource = new CancellationTokenSource();
    private readonly IPlatformServices _platformServices;
    private readonly IRetryManager _retryManager;
    private readonly ISegment _segment;
    private readonly IWebMetadataFactory _webMetadataFactory;
    private readonly IWebReader _webReader;
    private Uri _actualUrl;
    private long? _endOffset;
    private long? _expectedBytes;
    private int _isDisposed;
    private Stream _readStream;
    private IWebStreamResponse _response;
    private Stream _responseStream;
    private long? _startOffset;

    public Uri Url
    {
      get
      {
        return this._segment.Url;
      }
    }

    public bool IsEof { get; private set; }

    public SegmentReader(ISegment segment, IWebReader webReader, IWebMetadataFactory webMetadataFactory, IRetryManager retryManager, IPlatformServices platformServices)
    {
      if (null == segment)
        throw new ArgumentNullException("segment");
      if (null == webReader)
        throw new ArgumentNullException("webReader");
      if (null == webMetadataFactory)
        throw new ArgumentNullException("webMetadataFactory");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      if (null == platformServices)
        throw new ArgumentNullException("platformServices");
      this._segment = segment;
      this._webReader = webReader;
      this._webMetadataFactory = webMetadataFactory;
      this._retryManager = retryManager;
      this._platformServices = platformServices;
      if (segment.Offset < 0L || segment.Length <= 0L)
        return;
      this._startOffset = new long?(segment.Offset);
      this._endOffset = new long?(segment.Offset + segment.Length - 1L);
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._isDisposed, 1))
        return;
      try
      {
        this._disposedCancellationTokenSource.Cancel();
      }
      catch (Exception ex)
      {
        Debug.WriteLine("SegmentReader.Dispose() Cancellation failed: " + ex.Message);
      }
      this.Close();
    }

    public async Task<int> ReadAsync(byte[] buffer, int offset, int length, Action<ISegmentMetadata> setMetadata, CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      int index = 0;
      int thresholdSize = length - length / 4;
      int retryCount = 3;
      int delay = 200;
      int num1;
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._disposedCancellationTokenSource.Token, cancellationToken))
      {
        do
        {
          if (null == this._readStream)
            await this.OpenStream(setMetadata, cancellationToken).ConfigureAwait(false);
          Debug.Assert(null != this._readStream);
          bool retry = false;
          try
          {
            int count = await this._readStream.ReadAsync(buffer, offset + index, length - index, linkedTokenSource.Token).ConfigureAwait(false);
            if (count < 1)
            {
              if (!this.IsLengthValid())
                throw new WebException(string.Format("Read length mismatch mismatch ({0} expected)", (object) this._expectedBytes));
              this.IsEof = true;
              this.Close();
              num1 = index;
              goto label_26;
            }
            else
            {
              retryCount = 3;
              if (!this._startOffset.HasValue)
              {
                this._startOffset = new long?((long) count);
              }
              else
              {
                SegmentReader segmentReader = this;
                long? nullable1 = segmentReader._startOffset;
                long num2 = (long) count;
                long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() + num2) : new long?();
                segmentReader._startOffset = nullable2;
              }
              index += count;
            }
          }
          catch (OperationCanceledException ex)
          {
            this.Close();
            throw;
          }
          catch (Exception ex)
          {
            Debug.WriteLine("Read of {0} failed at {1}: {2}", (object) this._segment.Url, (object) this._startOffset, (object) ex.Message);
            this.Close();
            if (--retryCount <= 0)
              throw;
            else
              retry = true;
          }
          if (retry)
          {
            int actualDelay = (int) ((double) delay * (0.5 + this._platformServices.GetRandomNumber()));
            delay += delay;
            await TaskEx.Delay(actualDelay, linkedTokenSource.Token).ConfigureAwait(false);
          }
        }
        while (index < thresholdSize);
      }
      num1 = index;
label_26:
      return num1;
    }

    public void Close()
    {
      bool flag = object.ReferenceEquals((object) this._readStream, (object) this._responseStream);
      try
      {
        Stream stream = this._readStream;
        if (null != stream)
        {
          this._readStream = (Stream) null;
          using (stream)
            ;
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("SegmentReader.Close() readStream cleanup failed: " + ex.Message);
      }
      if (!flag)
      {
        try
        {
          Stream stream = this._responseStream;
          if (null != stream)
          {
            this._responseStream = (Stream) null;
            using (stream)
              ;
          }
        }
        catch (Exception ex)
        {
          Debug.WriteLine("SegmentReader.Close() responseStream cleanup failed: " + ex.Message);
        }
      }
      try
      {
        IWebStreamResponse webStreamResponse = this._response;
        if (null == webStreamResponse)
          return;
        this._response = (IWebStreamResponse) null;
        using (webStreamResponse)
          ;
      }
      catch (Exception ex)
      {
        Debug.WriteLine("SegmentReader.Close() response cleanup failed: " + ex.Message);
      }
    }

    private void ThrowIfDisposed()
    {
      if (0 != this._isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    private bool IsLengthValid()
    {
      try
      {
        long position = this._responseStream.Position;
        int num1;
        if (this._expectedBytes.HasValue)
        {
          long? nullable = this._expectedBytes;
          long num2 = position;
          num1 = nullable.GetValueOrDefault() != num2 ? 1 : (!nullable.HasValue ? 1 : 0);
        }
        else
          num1 = 0;
        return num1 == 0;
      }
      catch (NotSupportedException ex)
      {
        return true;
      }
    }

    private Task OpenStream(Action<ISegmentMetadata> setMetadata, CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      SM.Media.Web.WebResponse webResponse = new SM.Media.Web.WebResponse();
      IRetry retry = this._retryManager.CreateRetry(2, 200, new Func<Exception, bool>(RetryPolicy.IsWebExceptionRetryable));
      return RetryExtensions.CallAsync(retry, (Func<Task>) (async () =>
      {
        long? nullable1;
        long? nullable2;
        while (true)
        {
          if (this._startOffset.HasValue && this._endOffset.HasValue)
          {
            nullable1 = this._endOffset;
            nullable2 = this._startOffset;
            nullable1 = nullable1.HasValue & nullable2.HasValue ? new long?(nullable1.GetValueOrDefault() - nullable2.GetValueOrDefault()) : new long?();
            long? nullable3;
            if (!nullable1.HasValue)
            {
              nullable2 = new long?();
              nullable3 = nullable2;
            }
            else
              nullable3 = new long?(nullable1.GetValueOrDefault() + 1L);
            this._expectedBytes = nullable3;
          }
          else
            this._expectedBytes = new long?();
          this._response = await this._webReader.GetWebStreamAsync(this._actualUrl ?? this._segment.Url, false, cancellationToken, this._segment.ParentUrl, this._startOffset, this._endOffset, webResponse).ConfigureAwait(false);
          if (!this._response.IsSuccessStatusCode)
          {
            HttpStatusCode statusCode = (HttpStatusCode) this._response.HttpStatusCode;
            if (HttpStatusCode.NotFound != statusCode && !RetryPolicy.IsRetryable(statusCode))
              this._response.EnsureSuccessStatusCode();
            bool canRetry = await retry.CanRetryAfterDelayAsync(cancellationToken).ConfigureAwait(false);
            if (!canRetry)
            {
              if ((Uri) null != this._actualUrl && this._actualUrl != this._segment.Url)
                this._actualUrl = (Uri) null;
              else
                this._response.EnsureSuccessStatusCode();
            }
            this._response.Dispose();
            this._response = (IWebStreamResponse) null;
          }
          else
            break;
        }
        this._actualUrl = this._response.ActualUrl;
        long? contentLength = this._response.ContentLength;
        if (!this._endOffset.HasValue)
        {
          nullable1 = contentLength;
          long? nullable3;
          if (!nullable1.HasValue)
          {
            nullable2 = new long?();
            nullable3 = nullable2;
          }
          else
            nullable3 = new long?(nullable1.GetValueOrDefault() - 1L);
          this._endOffset = nullable3;
        }
        if (!this._expectedBytes.HasValue)
          this._expectedBytes = contentLength;
        SegmentReader segmentReader1 = this;
        ConfiguredTaskAwaitable<Stream> configuredTaskAwaitable = this._response.GetStreamAsync(cancellationToken).ConfigureAwait(false);
        PositionStream positionStream = new PositionStream(await configuredTaskAwaitable);
        segmentReader1._responseStream = (Stream) positionStream;
        Task<Stream> filterStreamTask = this._segment.CreateFilterAsync(this._responseStream, cancellationToken);
        if (null != filterStreamTask)
        {
          SegmentReader segmentReader2 = this;
          configuredTaskAwaitable = filterStreamTask.ConfigureAwait(false);
          Stream stream = await configuredTaskAwaitable;
          segmentReader2._readStream = stream;
        }
        else
          this._readStream = this._responseStream;
        ISegmentMetadata segmentMetadata = this._webMetadataFactory.CreateSegmentMetadata(webResponse, (ContentType) null);
        setMetadata(segmentMetadata);
      }), cancellationToken);
    }

    public override string ToString()
    {
      if (this._segment.Offset <= 0L && this._segment.Length <= 0L)
        return this.Url.ToString();
      return string.Format("{0} [{1}-{2}]", (object) this.Url, (object) this._segment.Offset, (object) (this._segment.Offset + this._segment.Length));
    }
  }
}
