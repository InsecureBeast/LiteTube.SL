using System;
using System.Text;
using SM.Media.Core.Audio.Shoutcast;

namespace SM.Media.Core.Buffering
{
  public class Utf8ShoutcastEncodingSelector : IShoutcastEncodingSelector
  {
    public Encoding GetEncoding(Uri url)
    {
      return Encoding.UTF8;
    }
  }
}
