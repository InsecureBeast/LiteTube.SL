using System;
using System.Net;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public class HttpClientFactoryParameters : IHttpClientFactoryParameters
    {
        public Uri Referrer { get; set; }

        public ICredentials Credentials { get; set; }

        public CookieContainer CookieContainer { get; set; }
    }
}
