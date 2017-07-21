// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.RetryPolicy
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace SM.Media.Utility
{
  public static class RetryPolicy
  {
    private static HttpStatusCode[] RetryCodes = Enumerable.ToArray<HttpStatusCode>((IEnumerable<HttpStatusCode>) Enumerable.OrderBy<HttpStatusCode, HttpStatusCode>((IEnumerable<HttpStatusCode>) new HttpStatusCode[3]
    {
      HttpStatusCode.GatewayTimeout,
      HttpStatusCode.RequestTimeout,
      HttpStatusCode.InternalServerError
    }, (Func<HttpStatusCode, HttpStatusCode>) (v => v)));
    private static readonly WebExceptionStatus[] WebRetryCodes = Enumerable.ToArray<WebExceptionStatus>((IEnumerable<WebExceptionStatus>) Enumerable.OrderBy<WebExceptionStatus, WebExceptionStatus>((IEnumerable<WebExceptionStatus>) new WebExceptionStatus[2]
    {
      WebExceptionStatus.ConnectFailure,
      WebExceptionStatus.SendFailure
    }, (Func<WebExceptionStatus, WebExceptionStatus>) (v => v)));

    public static bool IsRetryable(HttpStatusCode code)
    {
      return Array.BinarySearch<HttpStatusCode>(RetryPolicy.RetryCodes, code) >= 0;
    }

    public static bool IsRetryable(WebExceptionStatus code)
    {
      return Array.BinarySearch<WebExceptionStatus>(RetryPolicy.WebRetryCodes, code) >= 0;
    }

    public static bool IsWebExceptionRetryable(Exception ex)
    {
      WebException webException = ex as WebException;
      if (null == webException)
        return false;
      if (RetryPolicy.IsRetryable(webException.Status))
        return true;
      HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
      if (null != httpWebResponse)
        return RetryPolicy.IsRetryable(httpWebResponse.StatusCode);
      StatusCodeWebException codeWebException = webException as StatusCodeWebException;
      if (null != codeWebException)
        return RetryPolicy.IsRetryable(codeWebException.StatusCode);
      return false;
    }

    public static void ChangeRetryableStatusCodes(IEnumerable<HttpStatusCode> addCodes, IEnumerable<HttpStatusCode> removeCodes)
    {
      Dictionary<HttpStatusCode, bool> dictionary = new Dictionary<HttpStatusCode, bool>();
      while (true)
      {
        HttpStatusCode[] comparand = RetryPolicy.RetryCodes;
        foreach (HttpStatusCode index in comparand)
          dictionary[index] = true;
        if (null != addCodes)
        {
          foreach (HttpStatusCode index in addCodes)
            dictionary[index] = true;
        }
        if (null != removeCodes)
        {
          foreach (HttpStatusCode key in removeCodes)
            dictionary.Remove(key);
        }
        HttpStatusCode[] httpStatusCodeArray = Enumerable.ToArray<HttpStatusCode>((IEnumerable<HttpStatusCode>) Enumerable.OrderBy<HttpStatusCode, HttpStatusCode>((IEnumerable<HttpStatusCode>) dictionary.Keys, (Func<HttpStatusCode, HttpStatusCode>) (v => v)));
        if (comparand != Interlocked.CompareExchange<HttpStatusCode[]>(ref RetryPolicy.RetryCodes, httpStatusCodeArray, comparand))
          dictionary.Clear();
        else
          break;
      }
    }
  }
}
