using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.Audio.Shoutcast
{
  public class ShoutcastMetadataFilter : IAudioParser, IDisposable
  {
    public static bool UseParseByPairs = true;
    private readonly MemoryStream _buffer = new MemoryStream();
    private ShoutcastMetadataFilter.State _state = ShoutcastMetadataFilter.State.Data;
    private readonly IAudioParser _audioParser;
    private readonly Encoding _encoding;
    private readonly int _interval;
    private readonly Action<ITrackMetadata> _reportMetadata;
    private int _metadataLength;
    private int _remainingData;

    public TimeSpan StartPosition
    {
      get
      {
        return this._audioParser.StartPosition;
      }
      set
      {
        this._audioParser.StartPosition = value;
      }
    }

    public TimeSpan? Position
    {
      get
      {
        return this._audioParser.Position;
      }
      set
      {
        this._audioParser.Position = value;
      }
    }

    public ShoutcastMetadataFilter(IAudioParser audioParser, Action<ITrackMetadata> reportMetadata, int interval, Encoding encoding)
    {
      if (null == audioParser)
        throw new ArgumentNullException("audioParser");
      if (null == reportMetadata)
        throw new ArgumentNullException("reportMetadata");
      if (interval < 1)
        throw new ArgumentOutOfRangeException("interval", "must be positive");
      this._encoding = encoding;
      this._audioParser = audioParser;
      this._reportMetadata = reportMetadata;
      this._interval = interval;
      this._remainingData = this._interval;
    }

    public void FlushBuffers()
    {
      if (this._buffer.Length > 0L)
        this.ProcessMetadata((Stream) this._buffer);
      this._audioParser.FlushBuffers();
      this._state = ShoutcastMetadataFilter.State.Data;
      this._buffer.SetLength(0L);
      this._remainingData = this._interval;
    }

    public void Dispose()
    {
      this._audioParser.Dispose();
    }

    public void ProcessData(byte[] buffer, int offset, int length)
    {
      while (length > 0)
      {
        switch (this._state)
        {
          case ShoutcastMetadataFilter.State.Data:
            if (this._remainingData > length)
            {
              this._audioParser.ProcessData(buffer, offset, length);
              this._remainingData -= length;
              return;
            }
            if (this._remainingData > 0)
            {
              this._audioParser.ProcessData(buffer, offset, this._remainingData);
              length -= this._remainingData;
              offset += this._remainingData;
              this._remainingData = this._interval;
            }
            this._state = ShoutcastMetadataFilter.State.MetadataLength;
            break;
          case ShoutcastMetadataFilter.State.MetadataLength:
            this._metadataLength = (int) buffer[offset] * 16;
            --length;
            ++offset;
            this._state = this._metadataLength > 0 ? ShoutcastMetadataFilter.State.Metadata : ShoutcastMetadataFilter.State.Data;
            break;
          case ShoutcastMetadataFilter.State.Metadata:
            if (this._metadataLength > length)
            {
              this._buffer.Write(buffer, offset, length);
              this._metadataLength -= length;
              return;
            }
            if (this._metadataLength > 0)
            {
              this._buffer.Write(buffer, offset, this._metadataLength);
              length -= this._metadataLength;
              offset += this._metadataLength;
              this._metadataLength = 0;
            }
            if (this._buffer.Length > 0L)
              this.ProcessMetadata((Stream) this._buffer);
            this._state = ShoutcastMetadataFilter.State.Data;
            break;
        }
      }
    }

    protected virtual void ProcessMetadata(Stream stream)
    {
      Debug.WriteLine("ShoutcastMetadataFilter.ProcessMetadata() length " + (object) stream.Position);
      if (stream.Length < 1L)
        return;
      try
      {
        stream.Seek(0L, SeekOrigin.Begin);
        string str = new StreamReader(stream, this._encoding).ReadToEnd();
        if (!string.IsNullOrWhiteSpace(str))
          this.ParseStringMetadata(str.TrimEnd(new char[1]));
      }
      catch (Exception ex)
      {
        Debug.WriteLine("ShoutcastMetadataFilter.ProcessMetadata() failed: " + ExceptionExtensions.ExtendedMessage(ex));
      }
      finally
      {
        stream.SetLength(0L);
      }
    }

    protected virtual void ParseStringMetadata(string metadata)
    {
      Debug.WriteLine("ShoutcastMetadataFilter.ParseStringMetadata(): " + metadata);
      TrackMetadata trackMetadata = new TrackMetadata()
      {
        TimeStamp = this._audioParser.Position
      };
      if (ShoutcastMetadataFilter.UseParseByPairs)
        this.ParseByPairs(metadata, trackMetadata);
      else
        this.ParseByQuotes(metadata, trackMetadata);
      this._reportMetadata((ITrackMetadata) trackMetadata);
    }

    protected virtual void ParseByQuotes(string metadata, TrackMetadata trackMetadata)
    {
      int startIndex1 = 0;
      do
      {
        int startIndex2 = metadata.IndexOf('=', startIndex1);
        if (startIndex2 > startIndex1)
        {
          string name = metadata.Substring(startIndex1, startIndex2 - startIndex1).Trim();
          int num1 = metadata.IndexOf('\'', startIndex2);
          if (num1 >= 0 && num1 + 1 < metadata.Length)
          {
            int startIndex3 = metadata.IndexOf('\'', num1 + 1);
            if (startIndex3 >= 0)
            {
              string str = metadata.Substring(num1 + 1, startIndex3 - num1 - 1);
              this.AddNameValueProperty(trackMetadata, name, str);
              int num2 = metadata.IndexOf(';', startIndex3);
              if (num2 >= 0)
                startIndex1 = num2 + 1;
              else
                goto label_3;
            }
            else
              goto label_5;
          }
          else
            goto label_7;
        }
        else
          goto label_9;
      }
      while (startIndex1 < metadata.Length);
      goto label_1;
label_9:
      return;
label_7:
      return;
label_5:
      return;
label_3:
      return;
label_1:;
    }

    protected virtual void ParseByPairs(string metadata, TrackMetadata trackMetadata)
    {
      int startIndex1 = 0;
      do
      {
        int startIndex2 = metadata.IndexOf("='", startIndex1, StringComparison.Ordinal);
        if (startIndex2 > startIndex1)
        {
          string name = metadata.Substring(startIndex1, startIndex2 - startIndex1).Trim();
          int num = metadata.IndexOf("';", startIndex2, StringComparison.Ordinal);
          bool flag = false;
          if (num < 0)
          {
            num = metadata.LastIndexOf('\'');
            if (num >= 0 && num > startIndex2 + 1)
              flag = true;
            else
              goto label_7;
          }
          int startIndex3 = startIndex2 + 2;
          string str = metadata.Substring(startIndex3, num - startIndex3);
          this.AddNameValueProperty(trackMetadata, name, str);
          if (!flag)
            startIndex1 = num + 2;
          else
            goto label_4;
        }
        else
          goto label_9;
      }
      while (startIndex1 < metadata.Length);
      goto label_1;
label_9:
      return;
label_7:
      return;
label_4:
      return;
label_1:;
    }

    protected virtual void AddNameValueProperty(TrackMetadata trackMetadata, string name, string value)
    {
      Debug.WriteLine("ShoutcastMetadataFilter.AddNameValueProperty(): " + name + "=" + value);
      value = string.IsNullOrWhiteSpace(value) ? (string) null : value.Trim();
      switch (name.ToLowerInvariant())
      {
        case "streamtitle":
          trackMetadata.Title = value;
          break;
        case "streamurl":
          Uri result;
          if (!Uri.TryCreate(value, UriKind.Absolute, out result))
            break;
          trackMetadata.Website = result;
          break;
      }
    }

    private enum State
    {
      Data,
      MetadataLength,
      Metadata,
    }
  }
}
