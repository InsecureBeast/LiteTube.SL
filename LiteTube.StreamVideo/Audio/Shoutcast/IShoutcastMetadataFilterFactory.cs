using System;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.Audio.Shoutcast
{
  public interface IShoutcastMetadataFilterFactory
  {
    IAudioParser Create(ISegmentMetadata segmentMetadata, IAudioParser audioParser, Action<ITrackMetadata> reportMetadata, int interval);
  }
}
