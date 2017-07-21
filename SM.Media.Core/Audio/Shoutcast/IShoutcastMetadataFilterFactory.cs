using System;
using SM.Media.Core.Metadata;

namespace SM.Media.Core.Audio.Shoutcast
{
  public interface IShoutcastMetadataFilterFactory
  {
    IAudioParser Create(ISegmentMetadata segmentMetadata, IAudioParser audioParser, Action<ITrackMetadata> reportMetadata, int interval);
  }
}
