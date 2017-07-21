using System;
using System.Collections.Generic;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core
{
  public interface IStreamBuffer : IStreamSource, IDisposable
  {
    bool TryEnqueue(ICollection<TsPesPacket> packet);
  }
}
