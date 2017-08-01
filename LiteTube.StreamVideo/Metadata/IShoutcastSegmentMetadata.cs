namespace LiteTube.StreamVideo.Metadata
{
  public interface IShoutcastSegmentMetadata : ISegmentMetadata
  {
    bool SupportsIcyMetadata { get; }

    int? IcyMetaInt { get; }
  }
}
