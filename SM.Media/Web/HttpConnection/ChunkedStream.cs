// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.ChunkedStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnection
{
  public class ChunkedStream : AsyncReaderStream
  {
    private readonly IHttpReader _reader;
    private long _chunkRead;
    private long? _chunkSize;
    private bool _eof;

    public ChunkedStream(IHttpReader reader)
    {
      if (null == reader)
        throw new ArgumentNullException("reader");
      this._reader = reader;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      if (null == buffer)
        throw new ArgumentNullException("buffer");
      if (offset < 0 || offset > buffer.Length)
        throw new ArgumentOutOfRangeException("offset");
      if (count < 1 || count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException("count");
      int totalLength = 0;
      int count0 = count;
      do
      {
        int length = await this.ReadOneAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
        if (length >= 1)
        {
          totalLength += length;
          count -= length;
          if (count > 0)
            offset += length;
          else
            break;
        }
        else
          break;
      }
      while (this._reader.HasData);
      Debug.WriteLine("ChunkedStream.ReadAsync() {0}/{1}", (object) totalLength, (object) count0);
      return totalLength;
    }

    private async Task<int> ReadOneAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      int num1;
      if (this._eof)
      {
        num1 = 0;
      }
      else
      {
        ConfiguredTaskAwaitable<string> configuredTaskAwaitable;
        if (this._chunkSize.HasValue && this._chunkRead == this._chunkSize.Value)
        {
          configuredTaskAwaitable = this._reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
          string blankLine = await configuredTaskAwaitable;
          if (null == blankLine)
          {
            this._eof = true;
            num1 = 0;
            goto label_27;
          }
          else
          {
            if (!string.IsNullOrEmpty(blankLine))
            {
              this._eof = true;
              throw new WebException("Invalid chunked encoding");
            }
            this._chunkSize = new long?();
          }
        }
        if (!this._chunkSize.HasValue)
        {
          configuredTaskAwaitable = this._reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
          string chunkSizeLine = await configuredTaskAwaitable;
          if (string.IsNullOrEmpty(chunkSizeLine))
          {
            this._eof = true;
            num1 = 0;
            goto label_27;
          }
          else
          {
            int semicolon = chunkSizeLine.IndexOf(';');
            string chunkSizeString = semicolon > 0 ? chunkSizeLine.Substring(0, semicolon) : chunkSizeLine;
            long chunkSize;
            if (!long.TryParse(chunkSizeString, NumberStyles.HexNumber, (IFormatProvider) NumberFormatInfo.InvariantInfo, out chunkSize))
              throw new WebException("invalid chunk size: " + chunkSizeLine);
            if (chunkSize < 0L)
              throw new WebException("invalid chunk size: " + chunkSizeLine);
            if (0L == chunkSize)
            {
              this._eof = true;
              num1 = 0;
              goto label_27;
            }
            else
            {
              this._chunkSize = new long?(chunkSize);
              this._chunkRead = 0L;
            }
          }
        }
        int remaining = (int) (this._chunkSize.Value - this._chunkRead);
        if (count > remaining)
          count = remaining;
        if (count < 1)
        {
          num1 = 0;
        }
        else
        {
          int length = await this._reader.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
          Debug.WriteLine("ChunkedStream.ReadOneAsync() {0}/{1}", (object) length, (object) count);
          if (length < 1)
          {
            this._eof = true;
            num1 = 0;
          }
          else
          {
            long num2 = this._chunkRead + (long) length;
            long? nullable = this._chunkSize;
            Debug.Assert(num2 <= nullable.GetValueOrDefault() && nullable.HasValue);
            this._chunkRead += (long) length;
            this.IncrementPosition((long) length);
            num1 = length;
          }
        }
      }
label_27:
      return num1;
    }
  }
}
