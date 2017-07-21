using System;
using System.Net;
using SM.Media.Core.Content;

namespace SM.Media.Core.Web.WebRequestReader
{
  public interface IHttpWebRequests
  {
    HttpWebRequest CreateWebRequest(Uri url, Uri referrer = null, string method = null, ContentType contentType = null, bool allowBuffering = true, long? fromBytes = null, long? toBytes = null);

    bool SetIfModifiedSince(HttpWebRequest request, string ifModifiedSince);

    bool SetIfNoneMatch(HttpWebRequest request, string etag);

    bool SetCacheControl(HttpWebRequest request, string cacheControl);
  }
}
