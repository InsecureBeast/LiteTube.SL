using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Segments
{
  internal static class SegmentReaderManagerExtensions
  {
    public static async Task<TimeSpan> StartAsync(this ISegmentReaderManager segmentManager, CancellationToken cancellationToken)
    {
      await segmentManager.StartAsync().ConfigureAwait(false);
      return await segmentManager.SeekAsync(TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
    }
  }
}
