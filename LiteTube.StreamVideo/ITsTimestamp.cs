using System;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo
{
  public interface ITsTimestamp
  {
    TimeSpan StartPosition { get; set; }

    TimeSpan? Offset { get; }

    void Flush();

    bool ProcessPackets();

    void RegisterMediaStream(MediaStream mediaStream, Func<TsPesPacket, TimeSpan?> getDuration);
  }
}
