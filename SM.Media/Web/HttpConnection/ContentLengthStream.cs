// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.ContentLengthStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnection
{
  public class ContentLengthStream : AsyncReaderStream
  {
    private readonly long? _contentLength;
    private readonly IHttpReader _reader;

    public ContentLengthStream(IHttpReader reader, long? contentLength)
    {
      if (null == reader)
        throw new ArgumentNullException("reader");
      this._reader = reader;
      this._contentLength = contentLength;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      if (null == buffer)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset > buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 1 || count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count");
      long? nullable = this._contentLength;
      if (nullable.HasValue)
      {
        nullable = this._contentLength;
        long num = nullable.Value - this.Position;
        if ((long) count > num)
          count = (int) num;
      }
      int num1;
      if (count < 1)
      {
        num1 = 0;
      }
      else
      {
        int length = await this._reader.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        if (length > 0)
          this.IncrementPosition((long) length);
        num1 = length;
      }
      return num1;
    }
  }
}
