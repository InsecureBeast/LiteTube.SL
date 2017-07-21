// Decompiled with JetBrains decompiler
// Type: SM.Media.Pes.PesStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.TransportStream.TsParser;
using System;
using System.IO;

namespace SM.Media.Pes
{
  public sealed class PesStream : Stream
  {
    private int _location;
    private TsPesPacket _packet;

    public TsPesPacket Packet
    {
      get
      {
        return this._packet;
      }
      set
      {
        this._packet = value;
        this._location = 0;
      }
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return true;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override long Length
    {
      get
      {
        return (long) this.Packet.Length;
      }
    }

    public override long Position
    {
      get
      {
        return (long) this._location;
      }
      set
      {
        this.Seek(value, SeekOrigin.Begin);
      }
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      TsPesPacket packet = this.Packet;
      count = Math.Min(count, packet.Length - this._location);
      if (count < 1)
        return 0;
      Array.Copy((Array) packet.Buffer, packet.Index + this._location, (Array) buffer, offset, count);
      this._location += count;
      return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          if (offset > (long) this.Packet.Length || offset < 0L)
            throw new ArgumentOutOfRangeException("offset");
          this._location = (int) offset;
          break;
        case SeekOrigin.Current:
          long num = (long) this._location + offset;
          if (num < 0L || num > (long) this.Packet.Length)
            throw new ArgumentOutOfRangeException("offset");
          this._location = (int) num;
          break;
        case SeekOrigin.End:
          if (offset > (long) this.Packet.Length || offset < 0L)
            throw new ArgumentOutOfRangeException("offset");
          this._location = this.Packet.Length - (int) offset;
          break;
        default:
          throw new ArgumentException("origin");
      }
      return (long) this._location;
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }
  }
}
