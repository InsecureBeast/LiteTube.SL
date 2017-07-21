using System;

namespace SM.Media.Core.TransportStream.TsParser.Utility
{
  public interface ITsPesPacketPool : IDisposable
  {
    TsPesPacket AllocatePesPacket(int minSize);

    TsPesPacket AllocatePesPacket(BufferInstance bufferEntry);

    TsPesPacket CopyPesPacket(TsPesPacket packet, int index, int length);

    void FreePesPacket(TsPesPacket packet);
  }
}
