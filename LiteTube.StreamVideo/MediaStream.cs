using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiteTube.StreamVideo.MediaParser;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo
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
