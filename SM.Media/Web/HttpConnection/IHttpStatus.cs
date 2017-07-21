// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.IHttpStatus
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Net;

namespace SM.Media.Web.HttpConnection
{
  public interface IHttpStatus
  {
    bool ChunkedEncoding { get; }

    long? ContentLength { get; }

    HttpStatusCode StatusCode { get; }

    int VersionMajor { get; }

    int VersionMinor { get; }

    string ResponsePhrase { get; }

    string Version { get; }

    bool IsHttp { get; }

    bool IsSuccessStatusCode { get; }
  }
}
