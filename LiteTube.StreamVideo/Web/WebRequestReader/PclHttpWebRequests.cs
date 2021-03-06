﻿using System;
using System.Diagnostics;
using System.Net;

namespace LiteTube.StreamVideo.Web.WebRequestReader
{
  public class PclHttpWebRequests : HttpWebRequestsBase
  {
    public PclHttpWebRequests(ICredentials credentials = null, CookieContainer cookieContainer = null)
      : base(credentials, cookieContainer)
    {
    }

    public override bool SetReferrer(HttpWebRequest request, Uri referrer)
    {
      try
      {
        if ((Uri) null != referrer)
          request.Headers[HttpRequestHeader.Referer] = referrer.ToString();
        return true;
      }
      catch (Exception ex)
      {
        Debug.WriteLine("PclHttpWebRequests.SetReferrer() unable to set referrer: " + ex.Message);
        return false;
      }
    }

    public override bool SetIfModifiedSince(HttpWebRequest request, string ifModifiedSince)
    {
      try
      {
        if (null != ifModifiedSince)
          request.Headers[HttpRequestHeader.IfModifiedSince] = ifModifiedSince;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public override bool SetIfNoneMatch(HttpWebRequest request, string etag)
    {
      try
      {
        if (null != etag)
          request.Headers[HttpRequestHeader.IfNoneMatch] = etag;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public override bool SetCacheControl(HttpWebRequest request, string cacheControl)
    {
      try
      {
        if (null != cacheControl)
          request.Headers[HttpRequestHeader.CacheControl] = cacheControl;
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    protected override void SetRange(HttpWebRequest request, long? fromBytes, long? toBytes)
    {
      if (!fromBytes.HasValue && !toBytes.HasValue)
        return;
      request.Headers[HttpRequestHeader.Range] = string.Format("bytes={0}-{1}", new object[2]
      {
        fromBytes.HasValue ? (object) fromBytes.ToString() : (object) string.Empty,
        toBytes.HasValue ? (object) toBytes.ToString() : (object) string.Empty
      });
    }
  }
}
