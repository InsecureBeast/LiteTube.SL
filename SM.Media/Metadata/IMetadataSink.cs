// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.IMetadataSink
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Metadata
{
  public interface IMetadataSink
  {
    void ReportStreamMetadata(TimeSpan timestamp, IStreamMetadata streamMetadata);

    void ReportSegmentMetadata(TimeSpan timestamp, ISegmentMetadata segmentMetadata);

    void ReportTrackMetadata(ITrackMetadata trackMetadata);

    void ReportConfigurationMetadata(IConfigurationMetadata configurationMetadata);
  }
}
