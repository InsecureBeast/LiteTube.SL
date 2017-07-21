using SM.Media.Core.Utility;

namespace SM.Media.Core.Segments
{
  public interface ISegmentManagerReaders
  {
    ISegmentManager Manager { get; }

    IAsyncEnumerable<ISegmentReader> Readers { get; }
  }
}
