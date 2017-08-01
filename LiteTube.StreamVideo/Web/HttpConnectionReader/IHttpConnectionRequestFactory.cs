using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Web.HttpConnection;

namespace LiteTube.StreamVideo.Web.HttpConnectionReader
{
  public interface IHttpConnectionRequestFactory
  {
    HttpConnectionRequest CreateRequest(Uri url, Uri referrer, ContentType contentType, long? fromBytes, long? toBytes, IEnumerable<KeyValuePair<string, string>> headers);
  }
}
