using System;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.Pes
{
  public interface IPesHandlers : IDisposable
  {
    PesStreamHandler GetPesHandler(TsStreamType streamType, uint pid, IMediaStreamMetadata mediaStreamMetadata, Action<TsPesPacket> nextHandler);
  }
}
