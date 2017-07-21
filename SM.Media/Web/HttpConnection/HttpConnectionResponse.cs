// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.HttpConnectionResponse
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.IO;
using System.Linq;
using System.Net;

namespace SM.Media.Web.HttpConnection
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
