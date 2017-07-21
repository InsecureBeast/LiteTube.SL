// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.HttpEncoding
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility.TextEncodings;
using System.Text;

namespace SM.Media.Web.HttpConnection
{
  public sealed class HttpEncoding : IHttpEncoding
  {
    private readonly Encoding _decoding;
    private readonly Encoding _encoding;

    public Encoding HeaderDecoding
    {
      get
      {
        return this._decoding;
      }
    }

    public Encoding HeaderEncoding
    {
      get
      {
        return this._encoding;
      }
    }

    public HttpEncoding(ISmEncodings encodings)
    {
      this._encoding = encodings.AsciiEncoding;
      this._decoding = encodings.Latin1Encoding;
    }
  }
}
