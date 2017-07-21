// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnectionReader.HttpConnectionWebCache
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility;
using SM.Media.Web;
using SM.Media.Web.HttpConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnectionReader
{
  public class HttpConnectionWebCache : IWebCache
  {
    private const string NoCache = "no-cache";
    private readonly IRetryManager _retryManager;
    private readonly HttpConnectionWebReader _webReader;
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

    public HttpConnectionWebCache(HttpConnectionWebReader webReader, IRetryManager retryManager)
    {
      if (webReader == null)
        throw new ArgumentNullException("webReader");
      if (null == retryManager)
        throw new ArgumentNullException("retryManager");
      this._webReader = webReader;
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
      HttpConnectionRequest request = this.CreateRequest();
      using (IHttpConnectionResponse response = await this._webReader.SendAsync(request, true, cancellationToken, webResponse).ConfigureAwait(false))
      {
        if (response.IsSuccessStatusCode)
        {
          this._firstRequestCompleted = true;
          Func<Uri, byte[], TCached> func1 = factory;
          Uri responseUri = response.ResponseUri;
          Func<Uri, byte[], TCached> func2 = func1;
          HttpConnectionWebCache connectionWebCache = this;
          byte[] numArray = await this.FetchObject(response, cancellationToken).ConfigureAwait(false);
          // ISSUE: variable of a boxed type
          __Boxed<TCached> local = (object) func2(responseUri, numArray);
          connectionWebCache._cachedObject = (object) local;
        }
        else
        {
          HttpStatusCode statusCode = response.Status.StatusCode;
          if (HttpStatusCode.NotModified != statusCode)
          {
            if (RetryPolicy.IsRetryable(statusCode))
            {
              if (await retry.CanRetryAfterDelayAsync(cancellationToken).ConfigureAwait(false))
                goto label_12;
            }
            this._cachedObject = (object) null;
            response.EnsureSuccessStatusCode();
            throw new WebException("Unable to fetch " + (object) request.Url);
          }
        }
      }
    }

    private async Task<byte[]> FetchObject(IHttpConnectionResponse response, CancellationToken cancellationToken)
    {
      this._lastModified = Enumerable.FirstOrDefault<string>(response.Headers["Last-Modified"]);
      this._etag = Enumerable.FirstOrDefault<string>(response.Headers["ETag"]);
      this._cacheControl = Enumerable.FirstOrDefault<string>(response.Headers["CacheControl"]);
      byte[] numArray;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        await response.ContentReadStream.CopyToAsync((Stream) memoryStream, 4096, cancellationToken).ConfigureAwait(false);
        numArray = memoryStream.ToArray();
      }
      return numArray;
    }

    private HttpConnectionRequest CreateRequest()
    {
      Uri uri = this.WebReader.BaseAddress;
      bool flag = false;
      if (null == this._cachedObject)
      {
        this._lastModified = (string) null;
        this._etag = (string) null;
      }
      if (null != this._lastModified)
        flag = true;
      if (null != this._etag)
        flag = true;
      if (this._firstRequestCompleted && (!flag && null == this._cacheControl))
        this._noCache = "nocache=" + Guid.NewGuid().ToString("N");
      if (null != this._noCache)
      {
        UriBuilder uriBuilder = new UriBuilder(uri);
        uriBuilder.Query = !string.IsNullOrEmpty(uriBuilder.Query) ? uriBuilder.Query.Substring(1) + "&" + this._noCache : this._noCache;
        uri = uriBuilder.Uri;
      }
      HttpConnectionRequest webRequest = this._webReader.CreateWebRequest(uri, (Uri) null);
      List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
      if (null != this._lastModified)
        list.Add(new KeyValuePair<string, string>("If-Modified-Since", this._lastModified));
      if (null != this._etag)
        list.Add(new KeyValuePair<string, string>("If-None-Match", this._etag));
      if (!flag)
        list.Add(new KeyValuePair<string, string>("Cache-Control", "no-cache"));
      if (list.Count > 0)
        webRequest.Headers = (IEnumerable<KeyValuePair<string, string>>) list;
      return webRequest;
    }
  }
}
