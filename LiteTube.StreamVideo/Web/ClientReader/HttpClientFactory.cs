using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public class HttpClientFactory : IHttpClientFactory, IDisposable
    {
        private readonly CookieContainer _cookieContainer;
        private readonly ICredentials _credentials;
        private readonly Func<HttpClientHandler> _httpClientHandlerFactory;
        private readonly Uri _referrer;
        private readonly ProductInfoHeaderValue _userAgent;
        private readonly IWebReaderManagerParameters _webReaderManagerParameters;
        private int _disposed;

        public HttpClientFactory(IHttpClientFactoryParameters parameters, IWebReaderManagerParameters webReaderManagerParameters, IProductInfoHeaderValueFactory userAgentFactory, Func<HttpClientHandler> httpClientHandlerFactory)
        {
            if (null == parameters)
                throw new ArgumentNullException("parameters");
            if (null == webReaderManagerParameters)
                throw new ArgumentNullException("webReaderManagerParameters");
            if (null == userAgentFactory)
                throw new ArgumentNullException("userAgentFactory");
            if (null == httpClientHandlerFactory)
                throw new ArgumentNullException("httpClientHandlerFactory");
            this._referrer = parameters.Referrer;
            this._userAgent = userAgentFactory.Create();
            this._credentials = parameters.Credentials;
            this._cookieContainer = parameters.CookieContainer;
            this._webReaderManagerParameters = webReaderManagerParameters;
            this._httpClientHandlerFactory = httpClientHandlerFactory;
        }

        public void Dispose()
        {
            if (0 != Interlocked.Exchange(ref this._disposed, 1))
                return;
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        public virtual HttpClient CreateClient(Uri baseAddress, Uri referrer = null, ContentKind contentKind = ContentKind.Unknown, ContentType contentType = null)
        {
            if ((Uri)null == referrer && baseAddress != this._referrer)
                referrer = this._referrer;
            HttpClient httpClient = this.CreateHttpClient(baseAddress, referrer);
            if ((ContentType)null != contentType)
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType.MimeType));
                if (null != contentType.AlternateMimeTypes)
                {
                    foreach (string mediaType in (IEnumerable<string>)contentType.AlternateMimeTypes)
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                }
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.1));
            }
            return httpClient;
        }

        protected virtual HttpMessageHandler CreateClientHandler()
        {
            HttpClientHandler httpClientHandler = this._httpClientHandlerFactory();
            if (httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
            if (null != this._credentials)
                httpClientHandler.Credentials = this._credentials;
            if (null != this._cookieContainer)
                httpClientHandler.CookieContainer = this._cookieContainer;
            else
                httpClientHandler.UseCookies = false;
            return (HttpMessageHandler)httpClientHandler;
        }

        protected virtual HttpClient CreateHttpClient(Uri baseAddress, Uri referrer)
        {
            HttpClient httpClient = new HttpClient(this.CreateClientHandler());
            HttpRequestHeaders defaultRequestHeaders = httpClient.DefaultRequestHeaders;
            if ((Uri)null != baseAddress)
                httpClient.BaseAddress = baseAddress;
            if ((Uri)null != referrer)
            {
                if ((Uri)null == baseAddress)
                    httpClient.BaseAddress = referrer;
                defaultRequestHeaders.Referrer = referrer;
            }
            if (null != this._userAgent)
                defaultRequestHeaders.UserAgent.Add(this._userAgent);
            if (null != this._webReaderManagerParameters.DefaultHeaders)
            {
                foreach (KeyValuePair<string, string> keyValuePair in this._webReaderManagerParameters.DefaultHeaders)
                {
                    try
                    {
                        defaultRequestHeaders.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("HttpClientFactory.CreateHttpClient({0}) header {1}={2} failed: {3}", (object)baseAddress, (object)keyValuePair.Key, (object)keyValuePair.Value, (object)ExceptionExtensions.ExtendedMessage(ex));
                    }
                }
            }
            return httpClient;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                ;
        }
    }
}
