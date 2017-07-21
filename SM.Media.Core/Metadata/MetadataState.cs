using System;

namespace SM.Media.Core.Metadata
{
  public class MetadataState
  {
    public IConfigurationMetadata ConfigurationMetadata { get; set; }

    public ISegmentMetadata SegmentMetadata { get; set; }

    public TimeSpan SegmentTimestamp { get; set; }

    public IStreamMetadata StreamMetadata { get; set; }

    public TimeSpan StreamTimestamp { get; set; }

    public ITrackMetadata TrackMetadata { get; set; }
  }
}
