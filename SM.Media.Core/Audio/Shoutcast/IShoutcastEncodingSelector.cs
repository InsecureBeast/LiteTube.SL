using System;
using System.Text;

namespace SM.Media.Core.Audio.Shoutcast
{
  public interface IShoutcastEncodingSelector
  {
    Encoding GetEncoding(Uri url);
  }
}
