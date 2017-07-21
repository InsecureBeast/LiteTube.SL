using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Segments
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
