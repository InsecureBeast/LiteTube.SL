using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Segments
{
  public interface ISegmentManagerReaders
  {
    ISegmentManager Manager { get; }

    IAsyncEnumerable<ISegmentReader> Readers { get; }
  }
}
