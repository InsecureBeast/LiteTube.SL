using System;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Metadata;

namespace SM.Media.Core.Segments
{
  public interface ISegmentReader : IDisposable
  {
    Uri Url { get; }

    bool IsEof { get; }

    Task<int> ReadAsync(byte[] buffer, int offset, int length, Action<ISegmentMetadata> setMetadata, CancellationToken cancellationToken);

    void Close();
  }
}
