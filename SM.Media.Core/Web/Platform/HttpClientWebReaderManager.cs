using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Utility;
using SM.Media.Core.Web.ClientReader;

namespace SM.Media.Core.Web.Platform
{
    public class HttpClientWebReaderManager : IWebReaderManager, IDisposable
    {
        private readonly IContentTypeDetector _contentTypeDetector;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRetryManager _retryManager;
        private int _disposed;

        public HttpClientWebReaderManager(IHttpClientFactory httpClientFactory, IContentTypeDetector contentTypeDetector, IRetryManager retryManager)
        {
            if (null == httpClientFactory)
                throw new ArgumentNullException("httpClientFactory");
            if (null == contentTypeDetector)
                throw new ArgumentNullException("contentTypeDetector");
            if (null == retryManager)
                throw new ArgumentNullException("retryManager");
            this._httpClientFactory = httpClientFactory;
            this._contentTypeDetector = contentTypeDetector;
            this._retryManager = retryManager;
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref this._disposed, 1))
                return;
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        public virtual IWebReader CreateReader(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null)
        {
            return (IWebReader)this.CreateHttpClientWebReader(url, contentKind, parent, contentType);
        }

        public virtual IWebCache CreateWebCache(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null)
        {
            return (IWebCache)new HttpClientWebCache(this.CreateHttpClientWebReader(url, contentKind, parent, contentType), this._retryManager);
        }

        public virtual async Task<ContentType> DetectContentTypeAsync(Uri url, ContentKind contentKind, CancellationToken cancellationToken, IWebReader parent = null)
        {
            ContentType contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>)this._contentTypeDetector.GetContentType(url, (string)null, (string)null));
            ContentType contentType1;
            if ((ContentType)null != contentType)
            {
                Debug.WriteLine("HttpClientWebReaderManager.DetectContentTypeAsync() url ext \"{0}\" type {1}", (object)url, (object)contentType);
                contentType1 = contentType;
            }
            else
            {
                Uri referrer = HttpClientWebReaderManager.GetReferrer(parent);
                using (HttpClient client = this._httpClientFactory.CreateClient(url, referrer, ContentKind.Unknown, (ContentType)null))
                {
                    try
                    {
                        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, url))
                        {
                            using (HttpResponseMessage httpResponseMessage = await HttpClientExtensions.SendAsync(client, request, HttpCompletionOption.ResponseHeadersRead, cancellationToken, referrer, new long?(), new long?()).ConfigureAwait(false))
                            {
                                if (httpResponseMessage.IsSuccessStatusCode)
                                {
                                    contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>)HttpClientContentTypeDetectorExtensions.GetContentType(this._contentTypeDetector, request.RequestUri, httpResponseMessage.Content.Headers, HttpContentExtensions.FileName(httpResponseMessage.Content)));
                                    if ((ContentType)null != contentType)
                                    {
                                        Debug.WriteLine("HttpClientWebReaderManager.DetectContentTypeAsync() url HEAD \"{0}\" type {1}", (object)url, (object)contentType);
                                        contentType1 = contentType;
                                        goto label_49;
                                    }
                                }
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                    }
                    try
                    {
                        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            using (HttpResponseMessage httpResponseMessage = await HttpClientExtensions.SendAsync(client, request, HttpCompletionOption.ResponseHeadersRead, cancellationToken, referrer, new long?(0L), new long?(0L)).ConfigureAwait(false))
                            {
                                if (httpResponseMessage.IsSuccessStatusCode)
                                {
                                    contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>)HttpClientContentTypeDetectorExtensions.GetContentType(this._contentTypeDetector, request.RequestUri, httpResponseMessage.Content.Headers, HttpContentExtensions.FileName(httpResponseMessage.Content)));
                                    if ((ContentType)null != contentType)
                                    {
                                        Debug.WriteLine("HttpClientWebReaderManager.DetectContentTypeAsync() url range GET \"{0}\" type {1}", (object)url, (object)contentType);
                                        contentType1 = contentType;
                                        goto label_49;
                                    }
                                }
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                    }
                    try
                    {
                        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
                        {
                            using (HttpResponseMessage httpResponseMessage = await HttpClientExtensions.SendAsync(client, request, HttpCompletionOption.ResponseHeadersRead, cancellationToken, referrer, new long?(), new long?()).ConfigureAwait(false))
                            {
                                if (httpResponseMessage.IsSuccessStatusCode)
                                {
                                    contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>)HttpClientContentTypeDetectorExtensions.GetContentType(this._contentTypeDetector, request.RequestUri, httpResponseMessage.Content.Headers, HttpContentExtensions.FileName(httpResponseMessage.Content)));
                                    if ((ContentType)null != contentType)
                                    {
                                        Debug.WriteLine("HttpClientWebReaderManager.DetectContentTypeAsync() url GET \"{0}\" type {1}", (object)url, (object)contentType);
                                        contentType1 = contentType;
                                        goto label_49;
                                    }
                                }
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                    }
                }
                Debug.WriteLine("HttpClientWebReaderManager.DetectContentTypeAsync() url header \"{0}\" unknown type", (object)url);
                contentType1 = (ContentType)null;
            }
            label_49:
            return contentType1;
        }

        protected virtual HttpClientWebReader CreateHttpClientWebReader(Uri url, ContentKind contentKind, IWebReader parent = null, ContentType contentType = null)
        {
            url = HttpClientWebReaderManager.GetUrl(url, parent);
            if ((ContentType)null == contentType && (Uri)null != url)
                contentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>)this._contentTypeDetector.GetContentType(url, (string)null, (string)null));
            return new HttpClientWebReader((IWebReaderManager)this, this.CreateHttpClient(url, parent, contentKind, contentType), contentType, this._contentTypeDetector);
        }

        protected virtual HttpClient CreateHttpClient(Uri url, IWebReader parent, ContentKind contentKind, ContentType contentType)
        {
            url = HttpClientWebReaderManager.GetUrl(url, parent);
            Uri referrer = HttpClientWebReaderManager.GetReferrer(parent);
            if ((Uri)null != referrer)
                url = new Uri(referrer, url);
            return this._httpClientFactory.CreateClient(url, referrer, contentKind, contentType);
        }

        protected static Uri GetUrl(Uri url, IWebReader parent)
        {
            if ((Uri)null == url && null != parent)
                url = parent.RequestUri ?? parent.BaseAddress;
            return url;
        }

        protected static Uri GetReferrer(IWebReader parent)
        {
            return parent == null ? (Uri)null : parent.RequestUri ?? parent.BaseAddress;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                ;
        }
    }
}
