// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebRequestReader.IHttpWebRequests
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;
using System.Net;

namespace SM.Media.Web.WebRequestReader
{
  public interface IHttpWebRequests
  {
    HttpWebRequest CreateWebRequest(Uri url, Uri referrer = null, string method = null, ContentType contentType = null, bool allowBuffering = true, long? fromBytes = null, long? toBytes = null);

    bool SetIfModifiedSince(HttpWebRequest request, string ifModifiedSince);

    bool SetIfNoneMatch(HttpWebRequest request, string etag);

    bool SetCacheControl(HttpWebRequest request, string cacheControl);
  }
}
