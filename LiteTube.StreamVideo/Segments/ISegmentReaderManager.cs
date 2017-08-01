using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Segments
{
  public interface ISegmentReaderManager : IDisposable
  {
    ICollection<ISegmentManagerReaders> SegmentManagerReaders { get; }

    TimeSpan? Duration { get; }

    Task<TimeSpan> SeekAsync(TimeSpan timestamp, CancellationToken cancellationToken);

    Task StartAsync();

    Task StopAsync();
  }
}
