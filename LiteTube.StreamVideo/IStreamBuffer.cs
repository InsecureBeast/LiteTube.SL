using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo
{
  public interface IStreamBuffer : IStreamSource, IDisposable
  {
    bool TryEnqueue(ICollection<TsPesPacket> packet);
  }
}
