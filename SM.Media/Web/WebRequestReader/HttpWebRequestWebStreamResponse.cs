﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebRequestReader.HttpWebRequestWebStreamResponse
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Web;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.WebRequestReader
{
  public sealed class HttpWebRequestWebStreamResponse : IWebStreamResponse, IDisposable
  {
    private readonly int _httpStatusCode;
    private readonly HttpWebRequest _request;
    private readonly HttpWebResponse _response;
    private Task<Stream> _streamTask;

    public bool IsSuccessStatusCode
    {
      get
      {
        return null != this._response;
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
        return this._httpStatusCode;
      }
    }

    public long? ContentLength
    {
      get
      {
        return this._response == null ? new long?() : (this._response.ContentLength >= 0L ? new long?(this._response.ContentLength) : new long?());
      }
    }

    public HttpWebRequestWebStreamResponse(HttpWebRequest request, HttpWebResponse response)
    {
      if (null == response)
        throw new ArgumentNullException("response");
      this._request = request;
      this._response = response;
      this._httpStatusCode = (int) this._response.StatusCode;
    }

    public HttpWebRequestWebStreamResponse(System.Net.HttpStatusCode statusCode)
    {
      this._httpStatusCode = (int) statusCode;
    }

    public void Dispose()
    {
      if (this._streamTask != null && this._streamTask.IsCompleted)
        this._streamTask.Result.Dispose();
      using (this._response)
        ;
    }

    public void EnsureSuccessStatusCode()
    {
      if (this._httpStatusCode < 200 || this._httpStatusCode >= 300)
        throw new WebException("Invalid status: " + (object) this._httpStatusCode);
    }

    public Task<Stream> GetStreamAsync(CancellationToken cancellationToken)
    {
      if (null != this._streamTask)
        return this._streamTask;
      using (cancellationToken.Register((Action<object>) (r => ((WebRequest) r).Abort()), (object) this._request))
      {
        this._streamTask = TaskEx.FromResult<Stream>(this._response.GetResponseStream());
        return this._streamTask;
      }
    }
  }
}
