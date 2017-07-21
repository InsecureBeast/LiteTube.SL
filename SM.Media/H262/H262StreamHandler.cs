// Decompiled with JetBrains decompiler
// Type: SM.Media.H262.H262StreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using SM.Media.Pes;
using SM.Media.TransportStream.TsParser;
using SM.Media.TransportStream.TsParser.Utility;
using System;

namespace SM.Media.H262
{
  public class H262StreamHandler : PesStreamHandler
  {
    private readonly H262Configurator _configurator;
    private readonly Action<TsPesPacket> _nextHandler;
    private readonly ITsPesPacketPool _pesPacketPool;
    private bool _foundframe;

    public override IConfigurationSource Configurator
    {
      get
      {
        return (IConfigurationSource) this._configurator;
      }
    }

    public H262StreamHandler(PesStreamParameters parameters)
      : base(parameters)
    {
      if (null == parameters)
        throw new ArgumentNullException("parameters");
      if (null == parameters.PesPacketPool)
        throw new ArgumentException("PesPacketPool cannot be null", "parameters");
      if (null == parameters.NextHandler)
        throw new ArgumentException("NextHandler cannot be null", "parameters");
      this._pesPacketPool = parameters.PesPacketPool;
      this._nextHandler = parameters.NextHandler;
      this._configurator = new H262Configurator(parameters.MediaStreamMetadata, parameters.StreamType.Description);
    }

    public override void PacketHandler(TsPesPacket packet)
    {
      base.PacketHandler(packet);
      if (null == packet)
        this._nextHandler((TsPesPacket) null);
      else if (packet.Length < 1)
      {
        this._pesPacketPool.FreePesPacket(packet);
      }
      else
      {
        if (!this._foundframe)
        {
          this._configurator.Configure();
          this._foundframe = true;
        }
        this._nextHandler(packet);
      }
    }
  }
}
