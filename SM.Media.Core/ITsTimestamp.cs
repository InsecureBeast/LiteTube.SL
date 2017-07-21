using System;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media
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
