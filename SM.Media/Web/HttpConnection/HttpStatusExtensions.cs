// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.HttpStatusExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Web;
using System;

namespace SM.Media.Web.HttpConnection
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
