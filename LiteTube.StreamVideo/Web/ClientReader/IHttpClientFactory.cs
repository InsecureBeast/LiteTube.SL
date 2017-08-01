using System;
using System.Net.Http;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public interface IHttpClientFactory : IDisposable
    {
        HttpClient CreateClient(Uri baseAddress, Uri referrer = null, ContentKind contentKind = ContentKind.Unknown, ContentType contentType = null);
    }
}
