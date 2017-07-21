// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.HttpStatus
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Net;

namespace SM.Media.Web.HttpConnection
{
  public sealed class HttpStatus : IHttpStatus
  {
    public bool ChunkedEncoding { get; set; }

    public long? ContentLength { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public int VersionMajor { get; set; }

    public int VersionMinor { get; set; }

    public string ResponsePhrase { get; set; }

    public string Version { get; set; }

    public bool IsHttp { get; set; }

    public bool IsSuccessStatusCode
    {
      get
      {
        int num = (int) this.StatusCode;
        return num >= 200 && num <= 299;
      }
    }
  }
}
