using System;
using System.Net;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public interface IHttpClientFactoryParameters
    {
        Uri Referrer { get; }

        ICredentials Credentials { get; }

        CookieContainer CookieContainer { get; }
    }
}
