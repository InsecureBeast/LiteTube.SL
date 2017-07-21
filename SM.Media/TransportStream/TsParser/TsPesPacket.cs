// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.TsPesPacket
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.TransportStream.TsParser.Utility;
using System;
using System.Threading;

namespace SM.Media.TransportStream.TsParser
{
  public class TsPesPacket
  {
    public readonly int PacketId = Interlocked.Increment(ref TsPesPacket._packetCount);
    internal BufferInstance BufferEntry;
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
