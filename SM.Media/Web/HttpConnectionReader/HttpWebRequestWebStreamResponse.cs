// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnectionReader.HttpConnectionWebStreamResponse
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Web;
using SM.Media.Web.HttpConnection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnectionReader
{
  public sealed class HttpConnectionWebStreamResponse : IWebStreamResponse, IDisposable
  {
    private readonly IHttpStatus _httpStatus;
    private readonly IHttpConnectionResponse _response;

    public bool IsSuccessStatusCode
    {
      get
      {
        return this._httpStatus.IsSuccessStatusCode;
      }
    }

    public Uri ActualUrl
    {
      get
      {
        return this._response == null ? (Uri) null : this._response.ResponseUri;
      }
    }

    public int HttpStatusCode
    {
      get
      {
        return (int) this._httpStatus.StatusCode;
      }
    }

    public long? ContentLength
    {
      get
      {
        long? nullable1;
        if (this._response != null)
        {
          long? nullable2 = this._response.Status.ContentLength;
          if ((nullable2.GetValueOrDefault() < 0L ? 0 : (nullable2.HasValue ? 1 : 0)) == 0)
          {
            nullable2 = new long?();
            nullable1 = nullable2;
          }
          else
            nullable1 = this._response.Status.ContentLength;
        }
        else
          nullable1 = new long?();
        return nullable1;
      }
    }

    public HttpConnectionWebStreamResponse(IHttpConnectionResponse response)
    {
      if (null == response)
        throw new ArgumentNullException("response");
      if (null == response.Status)
        throw new ArgumentException("Not status in response", "response");
      this._response = response;
      this._httpStatus = this._response.Status;
    }

    public HttpConnectionWebStreamResponse(IHttpStatus httpStatus)
    {
      if (null == httpStatus)
        throw new ArgumentNullException("httpStatus");
      this._httpStatus = httpStatus;
    }

    public void Dispose()
    {
      using (this._response)
        ;
    }

    public void EnsureSuccessStatusCode()
    {
      HttpStatusExtensions.EnsureSuccessStatusCode(this._httpStatus);
    }

    public Task<Stream> GetStreamAsync(CancellationToken cancellationToken)
    {
      return TaskEx.FromResult<Stream>(this._response.ContentReadStream);
    }
  }
}
