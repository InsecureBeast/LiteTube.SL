using System;
using System.Text;

namespace LiteTube.StreamVideo.Audio.Shoutcast
{
  public class Utf8ShoutcastEncodingSelector : IShoutcastEncodingSelector
  {
    public Encoding GetEncoding(Uri url)
    {
      return Encoding.UTF8;
    }
  }
}
