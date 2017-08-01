using System;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Utility;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Segments
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
