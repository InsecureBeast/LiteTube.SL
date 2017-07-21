// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.PesStreamParameters
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using SM.Media.TransportStream.TsParser.Utility;
using System;

namespace SM.Media.TransportStream.TsParser
{
  public class PesStreamParameters
  {
    private readonly ITsPesPacketPool _pesPacketPool;

    public uint Pid { get; set; }

    public TsStreamType StreamType { get; set; }

    public Action<TsPesPacket> NextHandler { get; set; }

    public ITsPesPacketPool PesPacketPool
    {
      get
      {
        return this._pesPacketPool;
      }
    }

    public IMediaStreamMetadata MediaStreamMetadata { get; set; }

    public PesStreamParameters(ITsPesPacketPool pesPacketPool)
    {
      if (null == pesPacketPool)
        throw new ArgumentNullException("pesPacketPool");
      this._pesPacketPool = pesPacketPool;
    }
  }
}
