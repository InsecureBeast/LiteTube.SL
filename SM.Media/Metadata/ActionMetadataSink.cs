// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.ActionMetadataSink
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Metadata
{
  public class ActionMetadataSink : MetadataSink
  {
    private readonly Action _updateAction;

    public ActionMetadataSink(Action updateAction)
    {
      if (null == updateAction)
        throw new ArgumentNullException("updateAction");
      this._updateAction = updateAction;
    }

    public override void Reset()
    {
      base.Reset();
      this._updateAction();
    }

    public override void ReportStreamMetadata(TimeSpan timestamp, IStreamMetadata streamMetadata)
    {
      base.ReportStreamMetadata(timestamp, streamMetadata);
      this._updateAction();
    }

    public override void ReportSegmentMetadata(TimeSpan timestamp, ISegmentMetadata segmentMetadata)
    {
      base.ReportSegmentMetadata(timestamp, segmentMetadata);
      this._updateAction();
    }

    public override void ReportTrackMetadata(ITrackMetadata trackMetadata)
    {
      base.ReportTrackMetadata(trackMetadata);
      this._updateAction();
    }

    public override void ReportConfigurationMetadata(IConfigurationMetadata configurationMetadata)
    {
      base.ReportConfigurationMetadata(configurationMetadata);
      this._updateAction();
    }
  }
}
