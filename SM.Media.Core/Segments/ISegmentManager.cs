using System;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;
using SM.Media.Core.Utility;
using SM.Media.Core.Web;

namespace SM.Media.Core.Segments
{
  public interface ISegmentManager : IDisposable
  {
    IWebReader WebReader { get; }

    TimeSpan StartPosition { get; }

    TimeSpan? Duration { get; }

    ContentType ContentType { get; }

    IAsyncEnumerable<ISegment> Playlist { get; }

    IStreamMetadata StreamMetadata { get; }

    Task StartAsync();

    Task<TimeSpan> SeekAsync(TimeSpan timestamp);

    Task StopAsync();
  }
}
