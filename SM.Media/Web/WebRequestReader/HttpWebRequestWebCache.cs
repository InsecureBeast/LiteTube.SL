﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebRequestReader.HttpWebRequestWebCache
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility;
using SM.Media.Web;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.WebRequestReader
{
  public class HttpWebRequestWebCache : IWebCache
  {
    private const string NoCache = "no-cache";
    private readonly IHttpWebRequests _httpWebRequests;
    private readonly IRetryManager _retryManager;
    private readonly HttpWebRequestWebReader _webReader;
    private string _cacheControl;
    private object _cachedObject;
    private string _etag;
    private bool _firstRequestCompleted;
    private string _lastModified;
    private string _noCache;

    public IWebReader WebReader
    {
      get
      {
        return (IWebReader) this._webReader;
      }
    }

    public HttpWebRequestWebCache(HttpWebRequestWebReader webReader, IHttpWebRequests httpWebRequests, IRetryManager retryManager)
    {
      if (webReader == null)
        throw new ArgumentNullException("webReader");
      if (null == httpWebRequests)
        throw new ArgumentNullException("httpWebRequests");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._webReader = webReader;
      this._httpWebRequests = httpWebRequests;
      this._retryManager = retryManager;
    }

    public async Task<TCached> ReadAsync<TCached>(Func<Uri, byte[], TCached> factory, CancellationToken cancellationToken, SM.Media.Web.WebResponse webResponse = null) where TCached : class
    {
      if (null == (object) (this._cachedObject as TCached))
        this._cachedObject = (object) null;
      IRetry retry = RetryManagerExtensions.CreateWebRetry(this._retryManager, 2, 250);
      await RetryExtensions.CallAsync(retry, (Func<Task>) (() => this.Fetch<TCached>(retry, factory, webResponse, cancellationToken)), cancellationToken).ConfigureAwait(false);
      return this._cachedObject as TCached;
    }

    private async Task Fetch<TCached>(IRetry retry, Func<Uri, byte[], TCached> factory, SM.Media.Web.WebResponse webResponse, CancellationToken cancellationToken) where TCached : class
    {
label_12:
      HttpWebRequest request = this.CreateRequest();
      using (HttpWebResponse response = await this._webReader.SendAsync(request, true, cancellationToken, webResponse).ConfigureAwait(false))
      {
        int statusCode = (int) response.StatusCode;
        if (statusCode >= 200 && statusCode < 300)
        {
          this._firstRequestCompleted = true;
          Func<Uri, byte[], TCached> func1 = factory;
          Uri responseUri = response.ResponseUri;
          Func<Uri, byte[], TCached> func2 = func1;
          HttpWebRequestWebCache webRequestWebCache = this;
          byte[] numArray = await this.FetchObject(response, cancellationToken).ConfigureAwait(false);
          // ISSUE: variable of a boxed type
          __Boxed<TCached> local = (object) func2(responseUri, numArray);
          webRequestWebCache._cachedObject = (object) local;
        }
        else if (HttpStatusCode.NotModified != response.StatusCode)
        {
          if (RetryPolicy.IsRetryable(response.StatusCode))
          {
            if (await retry.CanRetryAfterDelayAsync(cancellationToken).ConfigureAwait(false))
              goto label_12;
          }
          this._cachedObject = (object) null;
          throw new WebException("Unable to fetch");
        }
      }
    }

    private Task<byte[]> FetchObject(HttpWebResponse response, CancellationToken cancellationToken)
    {
      this._lastModified = response.Headers["Last-Modified"];
      this._etag = response.Headers["ETag"];
      this._cacheControl = response.Headers["CacheControl"];
      return HttpWebRequestExtensions.ReadAsByteArrayAsync(response, cancellationToken);
    }

    private HttpWebRequest CreateRequest()
    {
      Uri uri = this.WebReader.BaseAddress;
      bool flag = false;
      if (null != this._cachedObject)
      {
        if (null != this._lastModified)
          flag = true;
        if (null != this._etag)
          flag = true;
      }
      if (this._firstRequestCompleted && (!flag && null == this._cacheControl))
        this._noCache = "nocache=" + Guid.NewGuid().ToString("N");
      if (null != this._noCache)
      {
        UriBuilder uriBuilder = new UriBuilder(uri);
        uriBuilder.Query = !string.IsNullOrEmpty(uriBuilder.Query) ? uriBuilder.Query.Substring(1) + "&" + this._noCache : this._noCache;
        uri = uriBuilder.Uri;
      }
      HttpWebRequest webRequest = this._webReader.CreateWebRequest(uri);
      if (this._cachedObject != null && flag)
      {
        flag = false;
        if (this._httpWebRequests.SetIfModifiedSince(webRequest, this._lastModified))
          flag = true;
        if (this._httpWebRequests.SetIfNoneMatch(webRequest, this._etag))
          flag = true;
      }
      if (!flag)
        this._httpWebRequests.SetCacheControl(webRequest, "no-cache");
      return webRequest;
    }
  }
}
