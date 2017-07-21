// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebRequestReader.HttpWebRequestsBase
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace SM.Media.Web.WebRequestReader
{
  public abstract class HttpWebRequestsBase : IHttpWebRequests
  {
    private static bool _canSetAllowReadStreamBuffering = true;
    private readonly CookieContainer _cookieContainer;
    private readonly ICredentials _credentials;

    protected HttpWebRequestsBase(ICredentials credentials, CookieContainer cookieContainer)
    {
      this._credentials = credentials;
      this._cookieContainer = cookieContainer;
    }

    public virtual HttpWebRequest CreateWebRequest(Uri url, Uri referrer = null, string method = null, ContentType contentType = null, bool allowBuffering = true, long? fromBytes = null, long? toBytes = null)
    {
      HttpWebRequest http = WebRequest.CreateHttp(url);
      if (null != method)
        http.Method = method;
      this.SetDefaultCookies(http);
      this.SetDefaultCredentials(http);
      this.SetReferrer(http, referrer);
      this.SetContentType(http, contentType);
      this.SetBuffering(http, allowBuffering);
      this.SetRange(http, fromBytes, toBytes);
      return http;
    }

    public abstract bool SetIfModifiedSince(HttpWebRequest request, string ifModifiedSince);

    public abstract bool SetIfNoneMatch(HttpWebRequest request, string etag);

    public abstract bool SetCacheControl(HttpWebRequest request, string cacheControl);

    protected abstract void SetRange(HttpWebRequest request, long? fromBytes, long? toBytes);

    protected virtual void SetBuffering(HttpWebRequest request, bool allowBuffering)
    {
      if (!HttpWebRequestsBase._canSetAllowReadStreamBuffering || request.AllowReadStreamBuffering == allowBuffering)
        return;
      try
      {
        request.AllowReadStreamBuffering = allowBuffering;
      }
      catch (InvalidOperationException ex)
      {
        Debug.WriteLine("HttpWebRequestsBase.SetBuffering() unable to set AllowReadStreamBuffering to {0}: {1}", (object) (bool) (allowBuffering ? 1 : 0), (object) ex.Message);
        HttpWebRequestsBase._canSetAllowReadStreamBuffering = false;
      }
    }

    protected virtual void SetContentType(HttpWebRequest request, ContentType contentType)
    {
      if (!((ContentType) null != contentType))
        return;
      if (contentType.AlternateMimeTypes != null && contentType.AlternateMimeTypes.Count > 0)
        request.Accept = string.Join(", ", Enumerable.Concat<string>((IEnumerable<string>) new string[1]
        {
          contentType.MimeType
        }, (IEnumerable<string>) contentType.AlternateMimeTypes));
      else
        request.Accept = contentType.MimeType;
    }

    public abstract bool SetReferrer(HttpWebRequest request, Uri referrer);

    protected virtual bool SetCredentials(HttpWebRequest request, ICredentials credentials)
    {
      request.Credentials = credentials;
      return true;
    }

    protected virtual bool SetDefaultCredentials(HttpWebRequest request)
    {
      return this.SetCredentials(request, this._credentials);
    }

    protected virtual bool SetCookies(HttpWebRequest request, CookieContainer cookieContainer)
    {
      if (cookieContainer == null || !request.SupportsCookieContainer)
        return false;
      request.CookieContainer = cookieContainer;
      return true;
    }

    protected virtual bool SetDefaultCookies(HttpWebRequest request)
    {
      return this.SetCookies(request, this._cookieContainer);
    }
  }
}
