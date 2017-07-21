// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.PositionStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
{
  internal sealed class PositionStream : Stream
  {
    private readonly Stream _parent;
    private long _position;

    public override bool CanRead
    {
      get
      {
        return this._parent.CanRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return this._parent.CanSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return this._parent.CanWrite;
      }
    }

    public override long Length
    {
      get
      {
        return this._parent.Length;
      }
    }

    public override long Position
    {
      get
      {
        return this._position;
      }
      set
      {
        this._position = value;
      }
    }

    public override bool CanTimeout
    {
      get
      {
        return this._parent.CanTimeout;
      }
    }

    public PositionStream(Stream parent)
    {
      if (parent == null)
        throw new ArgumentNullException("parent");
      this._parent = parent;
    }

    public override void Flush()
    {
      this._parent.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int num = this._parent.Read(buffer, offset, count);
      this._position += (long) num;
      return num;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      int length = await this._parent.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
      this._position += (long) length;
      return length;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long num = this._parent.Seek(offset, origin);
      this._position = num;
      return num;
    }

    public override void SetLength(long value)
    {
      this._parent.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this._parent.Write(buffer, offset, count);
    }

    public override int GetHashCode()
    {
      return this._parent.GetHashCode();
    }

    public override int ReadByte()
    {
      int num = this._parent.ReadByte();
      if (-1 == num)
        return num;
      ++this._position;
      return num;
    }

    public override void WriteByte(byte value)
    {
      this._parent.WriteByte(value);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing)
        return;
      this._parent.Dispose();
    }
  }
}
