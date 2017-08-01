using System.Text;
using LiteTube.StreamVideo.Utility.TextEncodings;

namespace LiteTube.StreamVideo.Web.HttpConnection
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
