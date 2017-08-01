using System;

namespace LiteTube.StreamVideo.TransportStream.TsParser.Utility
{
  public interface ITsPesPacketPool : IDisposable
  {
    TsPesPacket AllocatePesPacket(int minSize);

    TsPesPacket AllocatePesPacket(BufferInstance bufferEntry);

    TsPesPacket CopyPesPacket(TsPesPacket packet, int index, int length);

    void FreePesPacket(TsPesPacket packet);
  }
}
