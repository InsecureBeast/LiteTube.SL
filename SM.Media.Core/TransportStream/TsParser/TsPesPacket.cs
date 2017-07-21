using System;
using System.Threading;

namespace SM.Media.Core.TransportStream.TsParser
{
  public class TsPesPacket
  {
    public readonly int PacketId = Interlocked.Increment(ref TsPesPacket._packetCount);
    internal Utility.BufferInstance BufferEntry;
    public int Index;
    public int Length;
    public TimeSpan PresentationTimestamp;
    public TimeSpan? DecodeTimestamp;
    public TimeSpan? Duration;
    private static int _packetCount;

    public byte[] Buffer
    {
      get
      {
        return this.BufferEntry.Buffer;
      }
    }

    public void Clear()
    {
      this.Index = this.Length = 0;
      this.PresentationTimestamp = TimeSpan.Zero;
      this.DecodeTimestamp = new TimeSpan?();
      this.Duration = new TimeSpan?();
    }

    public override string ToString()
    {
      return string.Format("Packet({0}) index {1} length {2} duration {3} timestamp {4}/{5} buffer {6}", (object) this.PacketId, (object) this.Index, (object) this.Length, (object) this.Duration, (object) this.PresentationTimestamp, (object) this.DecodeTimestamp, (object) this.BufferEntry);
    }
  }
}
