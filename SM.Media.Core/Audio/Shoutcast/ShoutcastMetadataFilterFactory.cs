using System;
using System.Text;
using SM.Media.Core.Metadata;

namespace SM.Media.Core.Audio.Shoutcast
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
