using System;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
  public interface IPesHandlers : IDisposable
  {
    PesStreamHandler GetPesHandler(TsStreamType streamType, uint pid, IMediaStreamMetadata mediaStreamMetadata, Action<TsPesPacket> nextHandler);
  }
}
