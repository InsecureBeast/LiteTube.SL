using System;
using SM.Media.Core.Pes;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.H262
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
        return _configurator;
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
