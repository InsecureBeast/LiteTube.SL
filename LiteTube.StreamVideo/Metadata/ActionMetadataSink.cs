using System;

namespace LiteTube.StreamVideo.Metadata
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
