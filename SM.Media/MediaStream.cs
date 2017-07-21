// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using SM.Media.MediaParser;
using SM.Media.TransportStream.TsParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SM.Media
{
  public sealed class MediaStream : IMediaParserMediaStream, IDisposable
  {
    private readonly List<TsPesPacket> _packets = new List<TsPesPacket>();
    private readonly IConfigurationSource _configurator;
    private readonly Action<TsPesPacket> _freePacket;
    private readonly IStreamBuffer _streamBuffer;

    public ICollection<TsPesPacket> Packets
    {
      get
      {
        return (ICollection<TsPesPacket>) this._packets;
      }
    }

    public IConfigurationSource ConfigurationSource
    {
      get
      {
        return this._configurator;
      }
    }

    public IStreamSource StreamSource
    {
      get
      {
        return (IStreamSource) this._streamBuffer;
      }
    }

    public MediaStream(IConfigurationSource configurator, IStreamBuffer streamBuffer, Action<TsPesPacket> freePacket)
    {
      if (null == streamBuffer)
        throw new ArgumentNullException("streamBuffer");
      if (null == freePacket)
        throw new ArgumentNullException("freePacket");
      this._configurator = configurator;
      this._streamBuffer = streamBuffer;
      this._freePacket = freePacket;
    }

    public void Dispose()
    {
      this.Flush();
    }

    public void Flush()
    {
      if (this._packets.Count <= 0)
        return;
      foreach (TsPesPacket tsPesPacket in this._packets)
      {
        if (null != tsPesPacket)
          this._freePacket(tsPesPacket);
      }
      this._packets.Clear();
    }

    public void EnqueuePacket(TsPesPacket packet)
    {
      this._packets.Add(packet);
    }

    public bool PushPackets()
    {
      if (this._packets.Count <= 0)
        return false;
      if (!this._streamBuffer.TryEnqueue((ICollection<TsPesPacket>) this._packets))
      {
        Debug.WriteLine("MediaStream.PushPackets() the stream buffer was not ready to accept the packets: " + (object) this._streamBuffer);
        return false;
      }
      this._packets.Clear();
      return true;
    }
  }
}
