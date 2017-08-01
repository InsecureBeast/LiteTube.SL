using System;
using System.Diagnostics;

namespace LiteTube.StreamVideo.TransportStream.TsParser.Utility
{
  public sealed class TsPesPacketPool : ITsPesPacketPool, IDisposable
  {
    private readonly ObjectPool<TsPesPacket> _packetPool = new ObjectPool<TsPesPacket>();
    private readonly IBufferPool _bufferPool;

    public TsPesPacketPool(IBufferPool bufferPool)
    {
      if (null == bufferPool)
        throw new ArgumentNullException("bufferPool");
      this._bufferPool = bufferPool;
    }

    public void Dispose()
    {
      using (this._packetPool)
        ;
    }

    public TsPesPacket AllocatePesPacket(int minSize)
    {
      if (minSize < 1)
        throw new ArgumentOutOfRangeException("minSize", "minSize must be positive: " + (object) minSize);
      return this.AllocatePacketWithOwnedBuffer(this._bufferPool.Allocate(minSize));
    }

    public TsPesPacket AllocatePesPacket(BufferInstance bufferEntry)
    {
      if (null == bufferEntry)
        throw new ArgumentNullException("bufferEntry");
      bufferEntry.Reference();
      return this.AllocatePacketWithOwnedBuffer(bufferEntry);
    }

    public TsPesPacket CopyPesPacket(TsPesPacket packet, int index, int length)
    {
      if (packet == null)
        throw new ArgumentNullException("packet");
      if (index < 0 || index < packet.Index)
        throw new ArgumentOutOfRangeException("index");
      if (length < 0 || index + length > packet.Index + packet.Length)
        throw new ArgumentOutOfRangeException("length");
      Debug.Assert(packet.Index >= 0);
      Debug.Assert(packet.Index + packet.Length <= packet.Buffer.Length);
      TsPesPacket tsPesPacket = this.AllocatePesPacket(packet.BufferEntry);
      tsPesPacket.Index = index;
      tsPesPacket.Length = length;
      return tsPesPacket;
    }

    public void FreePesPacket(TsPesPacket packet)
    {
      if (null == packet)
        throw new ArgumentNullException("packet");
      BufferInstance bufferInstance = packet.BufferEntry;
      if (null != bufferInstance)
      {
        for (int index = packet.Index; index < packet.Index + packet.Length; ++index)
          packet.Buffer[index] = (byte) 204;
        packet.BufferEntry = (BufferInstance) null;
        this._bufferPool.Free(bufferInstance);
      }
      packet.Index = int.MinValue;
      packet.Length = int.MinValue;
      packet.PresentationTimestamp = TimeSpan.MaxValue;
      packet.DecodeTimestamp = new TimeSpan?(TimeSpan.MaxValue);
      this._packetPool.Free(packet);
    }

    private TsPesPacket AllocatePacketWithOwnedBuffer(BufferInstance bufferEntry)
    {
      TsPesPacket tsPesPacket = this._packetPool.Allocate();
      tsPesPacket.BufferEntry = bufferEntry;
      tsPesPacket.Clear();
      return tsPesPacket;
    }

    public void Clear()
    {
      this._packetPool.Clear();
    }
  }
}
