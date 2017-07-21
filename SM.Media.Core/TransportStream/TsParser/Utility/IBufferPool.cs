using System;

namespace SM.Media.Core.TransportStream.TsParser.Utility
{
  public interface IBufferPool : IDisposable
  {
    BufferInstance Allocate(int minSize);

    void Free(BufferInstance bufferInstance);
  }
}
