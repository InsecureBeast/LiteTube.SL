using System;
using System.IO;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
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
