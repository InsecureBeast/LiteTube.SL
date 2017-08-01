using System;

namespace LiteTube.StreamVideo.TransportStream.TsParser.Utility
{
  public interface IBufferPool : IDisposable
  {
    BufferInstance Allocate(int minSize);

    void Free(BufferInstance bufferInstance);
  }
}
