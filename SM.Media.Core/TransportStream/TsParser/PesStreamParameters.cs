﻿using System;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.TransportStream.TsParser
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
