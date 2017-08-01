using System;
using System.Text;

namespace LiteTube.StreamVideo.Audio.Shoutcast
{
  public interface IShoutcastEncodingSelector
  {
    Encoding GetEncoding(Uri url);
  }
}
