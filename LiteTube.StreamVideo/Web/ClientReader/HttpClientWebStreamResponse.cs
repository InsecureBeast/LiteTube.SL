using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public sealed class HttpClientWebStreamResponse : IWebStreamResponse, IDisposable
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpResponseMessage _response;
        private Stream _stream;

        public bool IsSuccessStatusCode
        {
            get
            {
                return this._response.IsSuccessStatusCode;
            }
        }

        public Uri ActualUrl
        {
            get
            {
                return this._response.RequestMessage.RequestUri;
            }
        }

        public int HttpStatusCode
        {
            get
            {
                return (int)this._response.StatusCode;
            }
        }

        public long? ContentLength
        {
            get
            {
                return this._response.Content.Headers.ContentLength;
            }
        }

        public HttpClientWebStreamResponse(HttpResponseMessage response)
        {
            if (null == response)
                throw new ArgumentNullException("response");
            this._response = response;
        }

        public HttpClientWebStreamResponse(HttpRequestMessage request, HttpResponseMessage response)
          : this(response)
        {
            this._request = request;
        }

        public void Dispose()
        {
            using (this._stream)
                ;
            this._response.Dispose();
            using (this._request)
                ;
        }

        public void EnsureSuccessStatusCode()
        {
            this._response.EnsureSuccessStatusCode();
        }

        public async Task<Stream> GetStreamAsync(CancellationToken cancellationToken)
        {
            if (null == this._stream)
            {
                using (cancellationToken.Register((Action<object>)(r =>
                {
                    if (null == r)
                        return;
                    ((HttpRequestMessage)r).Dispose();
                }), (object)this._request, false))
                    ((HttpClientWebStreamResponse)this)._stream = await this._response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            return this._stream;
        }
    }
}
