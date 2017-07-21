// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.AsyncReaderStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.IO;
using System.Threading;

namespace SM.Media.Web.HttpConnection
{
  public abstract class AsyncReaderStream : Stream
  {
    private long _position;

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
        return false;
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
        throw new NotSupportedException();
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
        throw new NotSupportedException();
      }
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      return this.ReadAsync(buffer, offset, count, CancellationToken.None).Result;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    protected void IncrementPosition(long length)
    {
      if (length < 0L)
        throw new ArgumentException("length cannot be negative", "length");
      this._position += length;
    }
  }
}
