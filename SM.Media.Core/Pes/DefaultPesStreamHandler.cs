using System;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.Pes
{
  public class DefaultPesStreamHandler : PesStreamHandler
  {
    private readonly Action<TsPesPacket> _nextHandler;

    public override IConfigurationSource Configurator
    {
      get
      {
        return (IConfigurationSource) null;
      }
    }

    public DefaultPesStreamHandler(PesStreamParameters parameters)
      : base(parameters)
    {
      if (null == parameters)
        throw new ArgumentNullException("parameters");
      if (null == parameters.NextHandler)
        throw new ArgumentException("NextHandler cannot be null", "parameters");
      this._nextHandler = parameters.NextHandler;
    }

    public override void PacketHandler(TsPesPacket packet)
    {
      base.PacketHandler(packet);
      this._nextHandler(packet);
    }
  }
}
