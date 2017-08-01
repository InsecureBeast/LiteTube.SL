using System;
using LiteTube.StreamVideo.Metadata;

namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  public interface ITsDecoder : IDisposable
  {
    bool EnableProcessing { get; set; }

    void ParseEnd();

    void Parse(byte[] buffer, int offset, int length);

    void Initialize(Func<TsStreamType, uint, IMediaStreamMetadata, TsPacketizedElementaryStream> pesStreamFactory, Action<IProgramStreams> programStreamsHandler);

    void FlushBuffers();

    void UnregisterHandler(uint pid);

    void RegisterHandler(uint pid, Action<TsPacket> add);

    TsPacketizedElementaryStream CreateStream(TsStreamType streamType, uint pid, IMediaStreamMetadata mediaStreamMetadata);
  }
}
