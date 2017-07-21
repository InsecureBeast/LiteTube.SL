// Decompiled with JetBrains decompiler
// Type: SM.Media.Pes.DefaultPesStreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using SM.Media.TransportStream.TsParser;
using System;

namespace SM.Media.Pes
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
