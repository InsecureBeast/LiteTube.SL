using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Segments
{
  public interface ISegment
  {
    Uri Url { get; }

    Uri ParentUrl { get; }

    long Offset { get; }

    long Length { get; }

    TimeSpan? Duration { get; }

    long? MediaSequence { get; }

    Task<Stream> CreateFilterAsync(Stream stream, CancellationToken cancellationToken);
  }
}
