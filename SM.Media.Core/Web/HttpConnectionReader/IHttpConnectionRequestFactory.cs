using System;
using System.Collections.Generic;
using SM.Media.Core.Content;
using SM.Media.Core.Web.HttpConnection;

namespace SM.Media.Core.Web.HttpConnectionReader
{
  public interface IHttpConnectionRequestFactory
  {
    HttpConnectionRequest CreateRequest(Uri url, Uri referrer, ContentType contentType, long? fromBytes, long? toBytes, IEnumerable<KeyValuePair<string, string>> headers);
  }
}
