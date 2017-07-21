// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.TsStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.TransportStream.TsParser
{
  public class TsStream
  {
    private readonly byte[] _buffer = new byte[32768];
    private readonly TsDecoder _decoder;
    private readonly Action<TsStream> _handler;
    private readonly uint _pid;
    private int _count;
    private int _index;

    public uint PID
    {
      get
      {
        return this._pid;
      }
    }

    public int Length
    {
      get
      {
        return this._index;
      }
    }

    public byte[] Buffer
    {
      get
      {
        return this._buffer;
      }
    }

    public TsStream(TsDecoder decoder, uint pid, Action<TsStream> handler)
    {
      this._decoder = decoder;
      this._pid = pid;
      this._handler = handler;
    }

    public void Add(TsPacket packet)
    {
      if (packet.IsStart)
      {
        this._index = 0;
      }
      else
      {
        int num = this._count + 1 & 15;
        if (packet.ContinuityCount != num)
          return;
      }
      this._count = packet.ContinuityCount;
      int payloadLength = packet.PayloadLength;
      if (this._index + payloadLength > this._buffer.Length)
        return;
      packet.CopyTo(this._buffer, this._index);
      this._index += payloadLength;
      if (null != this._handler)
        this._handler(this);
    }
  }
}
