﻿using System;

namespace LiteTube.StreamVideo.Metadata
{
  public interface IMetadataSink
  {
    void ReportStreamMetadata(TimeSpan timestamp, IStreamMetadata streamMetadata);

    void ReportSegmentMetadata(TimeSpan timestamp, ISegmentMetadata segmentMetadata);

    void ReportTrackMetadata(ITrackMetadata trackMetadata);

    void ReportConfigurationMetadata(IConfigurationMetadata configurationMetadata);
  }
}
