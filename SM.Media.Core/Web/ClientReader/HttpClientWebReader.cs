using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Utility;

namespace SM.Media.Core.Web.ClientReader
{
    public sealed class HttpClientWebReader : IWebReader, IDisposable
    {
        private readonly IContentTypeDetector _contentTypeDetector;
        private readonly HttpClient _httpClient;
        private readonly IWebReaderManager _webReaderManager;

        public Uri BaseAddress
        {
            get
            {
                return this._httpClient.BaseAddress;
            }
        }

        public Uri RequestUri { get; private set; }

        public ContentType ContentType { get; private set; }

        public IWebReaderManager Manager
        {
            get
            {
                return this._webReaderManager;
            }
        }

        public HttpClientWebReader(IWebReaderManager webReaderManager, HttpClient httpClient, ContentType contentType, IContentTypeDetector contentTypeDetector)
        {
            if (null == webReaderManager)
                throw new ArgumentNullException("webReaderManager");
            if (null == httpClient)
                throw new ArgumentNullException("httpClient");
            if (contentTypeDetector == null)
                throw new ArgumentNullException("contentTypeDetector");
            this._webReaderManager = webReaderManager;
            this._httpClient = httpClient;
            this.ContentType = contentType;
            this._contentTypeDetector = contentTypeDetector;
        }

        public void Dispose()
        {
            this._httpClient.Dispose();
        }

        public async Task<IWebStreamResponse> GetWebStreamAsync(Uri url, bool waitForContent, CancellationToken cancellationToken, Uri referrer = null, long? from = null, long? to = null, WebResponse webResponse = null)
        {
            HttpCompletionOption completionOption = waitForContent ? HttpCompletionOption.ResponseContentRead : HttpCompletionOption.ResponseHeadersRead;
            IWebStreamResponse webStreamResponse;
            if ((Uri)null == referrer && !from.HasValue && !to.HasValue)
            {
                HttpResponseMessage response = await this._httpClient.GetAsync(url, completionOption, cancellationToken).ConfigureAwait(false);
                this.Update(url, response, webResponse);
                webStreamResponse = (IWebStreamResponse)new HttpClientWebStreamResponse(response);
            }
            else
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                HttpResponseMessage response = await HttpClientExtensions.SendAsync(this._httpClient, request, completionOption, cancellationToken, referrer, from, to).ConfigureAwait(false);
                this.Update(url, response, webResponse);
                webStreamResponse = (IWebStreamResponse)new HttpClientWebStreamResponse(request, response);
            }
            return webStreamResponse;
        }

        public async Task<byte[]> GetByteArrayAsync(Uri url, CancellationToken cancellationToken, WebResponse webResponse = null)
        {
            byte[] numArray;
            using (HttpResponseMessage response = await this._httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                this.Update(url, response, webResponse);
                numArray = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            return numArray;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption responseContentRead, CancellationToken cancellationToken, WebResponse webResponse = null)
        {
            Uri url = request.RequestUri;
            HttpResponseMessage response = await this._httpClient.SendAsync(request, responseContentRead, cancellationToken).ConfigureAwait(false);
            this.Update(url, response, webResponse);
            return response;
        }

        private void Update(Uri url, HttpResponseMessage response, WebResponse webResponse)
        {
            if (!response.IsSuccessStatusCode)
                return;
            if (null != webResponse)
            {
                webResponse.RequestUri = response.RequestMessage.RequestUri;
                webResponse.ContentLength = response.Content.Headers.ContentLength;
                webResponse.Headers = Enumerable.Concat<KeyValuePair<string, IEnumerable<string>>>((IEnumerable<KeyValuePair<string, IEnumerable<string>>>)response.Headers, (IEnumerable<KeyValuePair<string, IEnumerable<string>>>)response.Content.Headers);
                webResponse.ContentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>)HttpClientContentTypeDetectorExtensions.GetContentType(this._contentTypeDetector, response.RequestMessage.RequestUri, response.Content.Headers, HttpContentExtensions.FileName(response.Content)));
            }
            if (url != this.BaseAddress)
                return;
            this.RequestUri = response.RequestMessage.RequestUri;
            if (!((ContentType)null == this.ContentType))
                return;
            this.ContentType = EnumerableExtensions.SingleOrDefaultSafe<ContentType>((IEnumerable<ContentType>)HttpClientContentTypeDetectorExtensions.GetContentType(this._contentTypeDetector, this.RequestUri, response.Content.Headers, HttpContentExtensions.FileName(response.Content)));
        }

        public override string ToString()
        {
            string str = (ContentType)null == this.ContentType ? "<unknown>" : this.ContentType.ToString();
            if ((Uri)null != this.RequestUri && this.RequestUri != this.BaseAddress)
                return string.Format("HttpWebReader {0} [{1}] ({2})", (object)this.BaseAddress, (object)this.RequestUri, (object)str);
            return string.Format("HttpWebReader {0} ({1})", new object[2]
            {
        (object) this.BaseAddress,
        (object) str
            });
        }
    }
}
