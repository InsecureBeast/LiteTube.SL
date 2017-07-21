using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Web.ClientReader
{
    public class HttpClientWebCache : IWebCache
    {
        private static readonly CacheControlHeaderValue NoCacheHeader = new CacheControlHeaderValue()
        {
            NoCache = true
        };
        private readonly IRetryManager _retryManager;
        private readonly HttpClientWebReader _webReader;
        private CacheControlHeaderValue _cacheControl;
        private object _cachedObject;
        private EntityTagHeaderValue _etag;
        private bool _firstRequestCompleted;
        private DateTimeOffset? _lastModified;
        private string _noCache;

        public IWebReader WebReader
        {
            get
            {
                return (IWebReader)this._webReader;
            }
        }

        public HttpClientWebCache(HttpClientWebReader webReader, IRetryManager retryManager)
        {
            if (webReader == null)
                throw new ArgumentNullException("webReader");
            if (null == retryManager)
                throw new ArgumentNullException("retryManager");
            this._webReader = webReader;
            this._retryManager = retryManager;
        }

        public async Task<TCached> ReadAsync<TCached>(Func<Uri, byte[], TCached> factory, CancellationToken cancellationToken, WebResponse webResponse = null) where TCached : class
        {
            if (null == (object)(this._cachedObject as TCached))
                this._cachedObject = (object)null;
            IRetry retry = RetryManagerExtensions.CreateWebRetry(this._retryManager, 2, 250);
            await RetryExtensions.CallAsync(retry, (Func<Task>)(() => this.Fetch<TCached>(retry, factory, webResponse, cancellationToken)), cancellationToken).ConfigureAwait(false);
            return this._cachedObject as TCached;
        }

        private async Task Fetch<TCached>(IRetry retry, Func<Uri, byte[], TCached> factory, WebResponse webResponse, CancellationToken cancellationToken) where TCached : class
        {
            while (true)
            {
                using (HttpRequestMessage request = this.CreateRequest())
                {
                    using (HttpResponseMessage response = await this._webReader.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken, webResponse).ConfigureAwait(false))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            this._firstRequestCompleted = true;
                            Func<Uri, byte[], TCached> func1 = factory;
                            Uri requestUri = response.RequestMessage.RequestUri;
                            Func<Uri, byte[], TCached> func2 = func1;
                            HttpClientWebCache httpClientWebCache = this;
                            byte[] numArray = await this.FetchObject(response).ConfigureAwait(false);
                            // ISSUE: variable of a boxed type
                            var local = (object)func2(requestUri, numArray);
                            httpClientWebCache._cachedObject = (object)local;
                            break;
                        }
                        if (HttpStatusCode.NotModified != response.StatusCode)
                        {
                            if (RetryPolicy.IsRetryable(response.StatusCode))
                            {
                                if (await retry.CanRetryAfterDelayAsync(cancellationToken).ConfigureAwait(false))
                                    goto label_16;
                            }
                            this._cachedObject = (object)null;
                            response.EnsureSuccessStatusCode();
                        }
                        else
                            break;
                    }
                }
                label_16:;
            }
        }

        private async Task<byte[]> FetchObject(HttpResponseMessage response)
        {
            this._lastModified = response.Content.Headers.LastModified;
            this._etag = response.Headers.ETag;
            this._cacheControl = response.Headers.CacheControl;
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        private HttpRequestMessage CreateRequest()
        {
            Uri uri = this.WebReader.BaseAddress;
            bool flag = false;
            if (null != this._cachedObject)
            {
                if (this._lastModified.HasValue)
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
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            if (this._cachedObject != null && flag)
            {
                if (this._lastModified.HasValue)
                    httpRequestMessage.Headers.IfModifiedSince = this._lastModified;
                if (null != this._etag)
                    httpRequestMessage.Headers.IfNoneMatch.Add(this._etag);
            }
            if (!flag)
                httpRequestMessage.Headers.CacheControl = HttpClientWebCache.NoCacheHeader;
            return httpRequestMessage;
        }
    }
}
