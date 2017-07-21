// Decompiled with JetBrains decompiler
// Type: SM.Media.Audio.Shoutcast.Latin1ShoutcastEncodingSelector
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility.TextEncodings;
using System;
using System.Text;

namespace SM.Media.Audio.Shoutcast
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
