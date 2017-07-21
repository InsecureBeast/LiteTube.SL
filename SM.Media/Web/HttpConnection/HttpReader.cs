// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.HttpConnection.HttpReader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Web.HttpConnection
{
  public sealed class HttpReader : IHttpReader, IDisposable
  {
    private byte[] _buffer = new byte[16384];
    private const int InitialCapacity = 16384;
    private const int MaximumCapacity = 65536;
    private const int ResizeRead = 1024;
    private const int MinimumRead = 1024;
    private const int MaximumRead = 1024;
    private const int HasDataThreshold = 256;
    private readonly Encoding _encoding;
    private readonly HttpReader.ReadAsyncDelegate _readAsync;
    private bool _badLine;
    private int _begin;
    private int _end;
    private bool _eof;

    public bool HasData
    {
      get
      {
        return this._end - this._begin > 256 || this._eof && this._end > this._begin;
      }
    }

    public HttpReader(HttpReader.ReadAsyncDelegate readAsync, Encoding encoding)
    {
      if (null == readAsync)
        throw new ArgumentNullException("readAsync");
      this._readAsync = readAsync;
      this._encoding = encoding;
    }

    public void Dispose()
    {
      this._buffer = (byte[]) null;
    }

    public void Clear()
    {
      this.ThrowIfDisposed();
      this._begin = 0;
      this._end = 0;
    }

    public async Task<string> ReadLineAsync(CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      this._badLine = false;
      int eolIndex;
      int length;
      int begin;
      do
      {
        eolIndex = this.FindLine();
        if (eolIndex >= 0)
        {
          begin = this._begin;
          this._begin = eolIndex;
          if (this._badLine)
            this._badLine = false;
          else
            goto label_3;
        }
        if (this._end - this._begin > this._buffer.Length - 1024 && !this.GrowBuffer())
        {
          this.Clear();
          this._badLine = true;
        }
        length = await this.FillBufferAsync(cancellationToken).ConfigureAwait(false);
      }
      while (length >= 1);
      goto label_8;
label_3:
      string str = this.CreateString(begin, eolIndex);
      goto label_12;
label_8:
      if (this._badLine)
      {
        this.Clear();
        str = (string) null;
      }
      else
        str = this.CreateString(this._begin, this._end);
label_12:
      return str;
    }

    public async Task<int> ReadAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      if (length < 1)
        throw new ArgumentException("argument must be positive", "length");
      int size = this._end - this._begin;
      int num;
      if (size < 1)
      {
        if (length >= this._buffer.Length)
        {
          num = await this.ReadBufferAsync(buffer, offset, length, cancellationToken).ConfigureAwait(false);
          goto label_11;
        }
        else
        {
          int bytesRead = await this.FillBufferAsync(cancellationToken).ConfigureAwait(false);
          if (bytesRead <= 0)
          {
            num = 0;
            goto label_11;
          }
          else
            size = this._end - this._begin;
        }
      }
      if (length >= size)
      {
        Array.Copy((Array) this._buffer, this._begin, (Array) buffer, offset, size);
        this.Clear();
        num = size;
      }
      else
      {
        Array.Copy((Array) this._buffer, this._begin, (Array) buffer, offset, length);
        this._begin += length;
        num = length;
      }
label_11:
      return num;
    }

    private async Task<int> FillBufferAsync(CancellationToken cancellationToken)
    {
      int remaining = this._buffer.Length - this._end;
      if (this._begin > 0 && remaining < 128)
      {
        int length1 = this._end - this._begin;
        Array.Copy((Array) this._buffer, this._begin, (Array) this._buffer, 0, length1);
        this._begin = 0;
        this._end = length1;
        remaining = this._buffer.Length - this._end;
      }
      int readLength = remaining;
      int length = await this.ReadBufferAsync(this._buffer, this._end, readLength, cancellationToken).ConfigureAwait(false);
      if (length > 0)
        this._end += length;
      return length;
    }

    private bool GrowBuffer()
    {
      if (this._buffer.Length >= 65536)
        return false;
      byte[] numArray = new byte[Math.Min(65536, 2 * this._buffer.Length)];
      if (this._end > this._begin)
      {
        int length = this._end - this._begin;
        Array.Copy((Array) this._buffer, this._begin, (Array) numArray, 0, length);
        this._begin = 0;
        this._end = length;
      }
      else
        this._begin = this._end = 0;
      this._buffer = numArray;
      return true;
    }

    private int FindLine()
    {
      for (int index = this._begin; index < this._end; ++index)
      {
        byte num = this._buffer[index];
        if (10 == (int) num)
          return index + 1;
        if (13 == (int) num && index + 1 < this._end)
        {
          if (10 == (int) this._buffer[index + 1])
            return index + 2;
          return index + 1;
        }
      }
      return -1;
    }

    private async Task<int> ReadBufferAsync(byte[] buffer, int offset, int length, CancellationToken cancellationToken)
    {
      int num;
      if (this._eof)
      {
        num = 0;
      }
      else
      {
        int bytesRead = await this._readAsync(buffer, offset, length, cancellationToken).ConfigureAwait(false);
        if (bytesRead <= 0)
        {
          this._eof = true;
          num = 0;
        }
        else
          num = bytesRead;
      }
      return num;
    }

    private string CreateString(int begin, int end)
    {
      if (end - begin < 1)
        return (string) null;
      switch ((char) this._buffer[end - 1])
      {
        case '\n':
          --end;
          if (end > begin && 13 == (int) this._buffer[end - 1])
          {
            --end;
            break;
          }
          break;
        case '\r':
          --end;
          break;
        default:
          return (string) null;
      }
      return end > begin ? this._encoding.GetString(this._buffer, begin, end - begin) : string.Empty;
    }

    private void ThrowIfDisposed()
    {
      if (null == this._buffer)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public delegate Task<int> ReadAsyncDelegate(byte[] buffer, int offset, int length, CancellationToken cancellationToken);
  }
}
