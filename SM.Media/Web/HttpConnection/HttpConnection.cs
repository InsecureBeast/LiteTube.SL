// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.HttpConnection
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnection
{
  public class HttpConnection : IHttpConnection, IDisposable
  {
    private readonly List<Tuple<string, string>> _headers = new List<Tuple<string, string>>();
    private readonly Encoding _headerDecoding;
    private readonly IHttpHeaderSerializer _httpHeaderSerializer;
    private int _disposed;
    private HttpStatus _httpStatus;
    private ISocket _socket;

    public HttpConnection(IHttpHeaderSerializer httpHeaderSerializer, IHttpEncoding httpEncoding, ISocket socket)
    {
      if (null == httpHeaderSerializer)
        throw new ArgumentNullException("httpHeaderSerializer");
      if (null == socket)
        throw new ArgumentNullException("socket");
      this._httpHeaderSerializer = httpHeaderSerializer;
      this._headerDecoding = httpEncoding.HeaderDecoding;
      this._socket = socket;
    }

    public void Dispose()
    {
      if (0 != Interlocked.Exchange(ref this._disposed, 1))
        return;
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public virtual Task ConnectAsync(Uri url, CancellationToken cancellationToken)
    {
      return this._socket.ConnectAsync(url, cancellationToken);
    }

    public async Task<IHttpConnectionResponse> GetAsync(HttpConnectionRequest request, bool closeConnection, CancellationToken cancellationToken)
    {
      this.StartRequest();
      if (closeConnection)
        request.KeepAlive = false;
      byte[] requestHeader = this.SerializeHeader("GET", request);
      Task<int> writeHeaderTask = this.WriteSocketAsync(requestHeader, 0, requestHeader.Length, cancellationToken);
      HttpReader httpReader = new HttpReader(new HttpReader.ReadAsyncDelegate(this.ReadSocketAsync), this._headerDecoding);
      IHttpConnectionResponse connectionResponse;
      try
      {
        string statusLine = await HttpReaderExtensions.ReadNonBlankLineAsync((IHttpReader) httpReader, cancellationToken).ConfigureAwait(false);
        this.ParseStatusLine(statusLine);
        await this.ReadHeadersAsync(httpReader, cancellationToken).ConfigureAwait(false);
        int num = await writeHeaderTask.ConfigureAwait(false);
        writeHeaderTask = (Task<int>) null;
        Stream stream = this._httpStatus.ChunkedEncoding ? (Stream) new ChunkedStream((IHttpReader) httpReader) : (Stream) new ContentLengthStream((IHttpReader) httpReader, this._httpStatus.ContentLength);
        HttpConnectionResponse response = new HttpConnectionResponse(request.Url, closeConnection ? (IHttpConnection) this : (IHttpConnection) null, (IHttpReader) httpReader, stream, Enumerable.ToLookup<Tuple<string, string>, string, string>((IEnumerable<Tuple<string, string>>) this._headers, (Func<Tuple<string, string>, string>) (kv => kv.Item1), (Func<Tuple<string, string>, string>) (kv => kv.Item2), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), (IHttpStatus) this._httpStatus);
        httpReader = (HttpReader) null;
        connectionResponse = (IHttpConnectionResponse) response;
      }
      finally
      {
        if (null != httpReader)
          httpReader.Dispose();
        if (null != writeHeaderTask)
          TaskCollector.Default.Add((Task) writeHeaderTask, "HttpConnection GetAsync writer");
      }
      return connectionResponse;
    }

    public virtual void Close()
    {
      this._socket.Close();
    }

    private byte[] SerializeHeader(string method, HttpConnectionRequest request)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        this._httpHeaderSerializer.WriteHeader((Stream) memoryStream, method, request);
        return memoryStream.ToArray();
      }
    }

    private Task<int> ReadSocketAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
    {
      ISocket socket = this._socket;
      if (null == socket)
        throw new ObjectDisposedException(this.GetType().FullName);
      return socket.ReadAsync(buffer, offset, length, cancellationToken);
    }

    private Task<int> WriteSocketAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
    {
      ISocket socket = this._socket;
      if (null == socket)
        throw new ObjectDisposedException(this.GetType().FullName);
      return socket.WriteAsync(buffer, offset, length, cancellationToken);
    }

    private void StartRequest()
    {
      this._httpStatus = new HttpStatus();
      this._headers.Clear();
    }

    private async Task ReadHeadersAsync(HttpReader httpReader, CancellationToken cancellationToken)
    {
      while (true)
      {
        Tuple<string, string> nameValue;
        string value;
        do
        {
          nameValue = await HttpReaderExtensions.ReadHeaderAsync(httpReader, cancellationToken).ConfigureAwait(false);
          if (null != nameValue)
          {
            this._headers.Add(nameValue);
            value = nameValue.Item2;
          }
          else
            goto label_12;
        }
        while (string.IsNullOrEmpty(value));
        string name = nameValue.Item1;
        if (string.Equals(name, "Content-Length", StringComparison.OrdinalIgnoreCase))
        {
          long result;
          if (long.TryParse(value, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
            this._httpStatus.ContentLength = new long?(result);
        }
        else if (string.Equals(name, "Transfer-Encoding", StringComparison.OrdinalIgnoreCase))
        {
          int length = value.IndexOf(';');
          if (string.Equals(length > 1 ? value.Substring(0, length).Trim() : value, "chunked", StringComparison.OrdinalIgnoreCase))
            this._httpStatus.ChunkedEncoding = true;
        }
      }
label_12:;
    }

    private void ParseStatusLine(string statusLine)
    {
      if (statusLine.StartsWith("HTTP", StringComparison.Ordinal))
      {
        this.ParseRealHttp(statusLine);
        this._httpStatus.IsHttp = true;
      }
      else
      {
        string[] strArray = statusLine.Split(new char[1]
        {
          ' '
        }, 3, StringSplitOptions.RemoveEmptyEntries);
        this._httpStatus.Version = strArray[0];
        if (strArray.Length < 2 || strArray.Length > 3)
          throw new WebException("Invalid status line: " + statusLine);
        int result;
        if (!int.TryParse(strArray[1], NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
          throw new WebException("Invalid status code: " + statusLine);
        this._httpStatus.StatusCode = (HttpStatusCode) result;
        if (strArray.Length <= 2)
          return;
        string str = strArray[2].Trim();
        if (str.Length > 0)
          this._httpStatus.ResponsePhrase = str;
      }
    }

    private void ParseRealHttp(string statusLine)
    {
      int length1 = statusLine.IndexOf('/');
      if (length1 < 1 || length1 + 1 >= statusLine.Length)
        throw new WebException("Invalid status line: " + statusLine);
      int num1 = statusLine.IndexOf(' ', length1 + 1);
      if (num1 < 1 || num1 + 1 >= statusLine.Length)
        throw new WebException("Invalid status line: " + statusLine);
      int num2 = statusLine.IndexOf(' ', num1 + 1);
      Debug.Assert(length1 + 1 < num1, "Unable to parse status line 1");
      Debug.Assert(num2 < 0 || num2 > num1, "Unable to parse status line 2");
      if (!string.Equals(statusLine.Substring(0, length1), "HTTP", StringComparison.Ordinal))
        throw new WebException("Invalid protocol: " + statusLine);
      string str = statusLine.Substring(length1 + 1, num1 - length1 - 1);
      this._httpStatus.Version = str;
      int length2 = str.IndexOf('.');
      if (length2 < 1 || length2 + 1 >= str.Length)
        throw new WebException("Invalid protocol: " + statusLine);
      string s1 = str.Substring(0, length2);
      string s2 = str.Substring(length2 + 1, str.Length - length2 - 1);
      int result1;
      if (!int.TryParse(s1, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1))
        throw new WebException("Invalid protocol version: " + statusLine);
      this._httpStatus.VersionMajor = result1;
      if (!int.TryParse(s2, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1))
        throw new WebException("Invalid protocol version: " + statusLine);
      this._httpStatus.VersionMinor = result1;
      int result2;
      if (!int.TryParse(num2 > num1 + 1 ? statusLine.Substring(num1 + 1, num2 - num1 - 1) : statusLine.Substring(num1 + 1), NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result2))
        throw new WebException("Invalid status code: " + statusLine);
      if (result2 < 100 || result2 > 999)
        throw new WebException("Invalid status code: " + statusLine);
      this._httpStatus.StatusCode = (HttpStatusCode) result2;
      this._httpStatus.ResponsePhrase = num2 > num1 ? statusLine.Substring(num2 + 1) : (string) null;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      ISocket socket = this._socket;
      if (null == socket)
        return;
      this._socket = (ISocket) null;
      socket.Dispose();
    }
  }
}
