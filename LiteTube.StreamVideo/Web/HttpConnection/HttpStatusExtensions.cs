using System;

namespace LiteTube.StreamVideo.Web.HttpConnection
{
  public static class HttpStatusExtensions
  {
    public static void EnsureSuccessStatusCode(this IHttpStatus httpStatus)
    {
      if (null == httpStatus)
        throw new ArgumentNullException("httpStatus");
      if (!httpStatus.IsSuccessStatusCode)
        throw new StatusCodeWebException(httpStatus.StatusCode, httpStatus.ResponsePhrase, (Exception) null);
    }
  }
}
