using System.Net.Http;
using System.Net.Http.Headers;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public static class HttpContentExtensions
    {
        public static string FileName(this HttpContent httpContent)
        {
            if (null == httpContent)
                return (string)null;
            ContentDispositionHeaderValue contentDisposition = httpContent.Headers.ContentDisposition;
            if (null == contentDisposition)
                return (string)null;
            return contentDisposition.FileName;
        }
    }
}
