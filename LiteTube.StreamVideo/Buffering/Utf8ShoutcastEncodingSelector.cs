using System;
using System.Text;
using LiteTube.StreamVideo.Audio.Shoutcast;

namespace LiteTube.StreamVideo.Buffering
{
  public class Utf8ShoutcastEncodingSelector : IShoutcastEncodingSelector
  {
    public Encoding GetEncoding(Uri url)
    {
      return Encoding.UTF8;
    }
  }
}
