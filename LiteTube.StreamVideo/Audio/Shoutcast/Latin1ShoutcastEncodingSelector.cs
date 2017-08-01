using System;
using System.Text;
using LiteTube.StreamVideo.Utility.TextEncodings;

namespace LiteTube.StreamVideo.Audio.Shoutcast
{
  public class Latin1ShoutcastEncodingSelector : IShoutcastEncodingSelector
  {
    private readonly Encoding _latin1;

    public Latin1ShoutcastEncodingSelector(ISmEncodings encodings)
    {
      if (null == encodings)
        throw new ArgumentNullException("encodings");
      this._latin1 = encodings.Latin1Encoding;
    }

    public Encoding GetEncoding(Uri url)
    {
      return this._latin1;
    }
  }
}
