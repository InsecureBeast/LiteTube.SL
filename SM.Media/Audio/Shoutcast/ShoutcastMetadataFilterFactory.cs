// Decompiled with JetBrains decompiler
// Type: SM.Media.Audio.Shoutcast.ShoutcastMetadataFilterFactory
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Audio;
using SM.Media.Metadata;
using System;
using System.Text;

namespace SM.Media.Audio.Shoutcast
{
  public class ShoutcastMetadataFilterFactory : IShoutcastMetadataFilterFactory
  {
    private readonly IShoutcastEncodingSelector _shoutcastEncodingSelector;

    public ShoutcastMetadataFilterFactory(IShoutcastEncodingSelector shoutcastEncodingSelector)
    {
      if (null == shoutcastEncodingSelector)
        throw new ArgumentNullException("shoutcastEncodingSelector");
      this._shoutcastEncodingSelector = shoutcastEncodingSelector;
    }

    public IAudioParser Create(ISegmentMetadata segmentMetadata, IAudioParser audioParser, Action<ITrackMetadata> reportMetadata, int interval)
    {
      Encoding encoding = this._shoutcastEncodingSelector.GetEncoding(segmentMetadata.Url);
      return (IAudioParser) new ShoutcastMetadataFilter(audioParser, reportMetadata, interval, encoding);
    }
  }
}
