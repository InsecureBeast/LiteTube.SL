// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.ITsDecoder
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using System;

namespace SM.Media.TransportStream.TsParser
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
