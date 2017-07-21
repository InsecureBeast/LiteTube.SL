// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.StatusCodeWebException
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Net;

namespace SM.Media.Web
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
