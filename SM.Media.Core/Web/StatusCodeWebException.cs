using System;
using System.Net;

namespace SM.Media.Core.Web
{
  public class StatusCodeWebException : WebException
  {
    private readonly HttpStatusCode _statusCode;

    public HttpStatusCode StatusCode
    {
      get
      {
        return this._statusCode;
      }
    }

    public StatusCodeWebException(HttpStatusCode statusCode, string message, Exception innerException = null)
      : base(message, innerException)
    {
      this._statusCode = statusCode;
    }

    public static void ThrowIfNotSuccess(HttpStatusCode statusCode, string message)
    {
      int num = (int) statusCode;
      if (num < 200 || num >= 300)
        throw new StatusCodeWebException(statusCode, message, (Exception) null);
    }
  }
}
