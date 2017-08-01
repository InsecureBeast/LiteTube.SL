using System;
using System.IO;
using System.Linq;
using System.Net;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public class HttpConnectionResponse : IHttpConnectionResponse, IDisposable
  {
    private readonly ILookup<string, string> _headers;
    private readonly IHttpStatus _status;
    private readonly Uri _url;
    private IHttpConnection _connection;
    private IHttpReader _reader;
    private Stream _stream;

    public ILookup<string, string> Headers
    {
      get
      {
        return this._headers;
      }
    }

    public Stream ContentReadStream
    {
      get
      {
        return this._stream;
      }
    }

    public IHttpStatus Status
    {
      get
      {
        return this._status;
      }
    }

    public Uri ResponseUri
    {
      get
      {
        return this._url;
      }
    }

    public bool IsSuccessStatusCode
    {
      get
      {
        return this.Status != null && this.Status.IsSuccessStatusCode;
      }
    }

    public HttpConnectionResponse(Uri url, IHttpConnection connection, IHttpReader reader, Stream stream, ILookup<string, string> headers, IHttpStatus status)
    {
      if ((Uri) null == url)
        throw new ArgumentNullException("url");
      if (null == stream)
        throw new ArgumentNullException("stream");
      if (null == headers)
        throw new ArgumentNullException("headers");
      if (null == status)
        throw new ArgumentNullException("status");
      this._url = url;
      this._reader = reader;
      this._stream = stream;
      this._headers = headers;
      this._status = status;
      this._connection = connection;
    }

    public void Dispose()
    {
      Stream stream = this._stream;
      if (null != stream)
      {
        this._stream = (Stream) null;
        stream.Dispose();
      }
      IHttpReader httpReader = this._reader;
      if (null != httpReader)
      {
        this._reader = (IHttpReader) null;
        httpReader.Dispose();
      }
      IHttpConnection httpConnection = this._connection;
      if (null == httpConnection)
        return;
      this._connection = httpConnection;
      httpConnection.Dispose();
    }

    public void EnsureSuccessStatusCode()
    {
      if (null == this.Status)
        throw new WebException("No status available", WebExceptionStatus.UnknownError);
      HttpStatusExtensions.EnsureSuccessStatusCode(this.Status);
    }
  }
}
