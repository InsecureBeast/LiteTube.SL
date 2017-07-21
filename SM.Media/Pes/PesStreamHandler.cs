// Decompiled with JetBrains decompiler
// Type: SM.Media.Pes.PesStreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using SM.Media.TransportStream.TsParser;
using System;

namespace SM.Media.Pes
{
  public abstract class PesStreamHandler : IPesStreamHandler
  {
    protected readonly uint Pid;
    protected readonly TsStreamType StreamType;

    public abstract IConfigurationSource Configurator { get; }

    protected PesStreamHandler(PesStreamParameters parameters)
    {
      if (null == parameters)
        throw new ArgumentNullException("parameters");
      this.StreamType = parameters.StreamType;
      this.Pid = parameters.Pid;
    }

    public virtual void PacketHandler(TsPesPacket packet)
    {
    }

    public virtual TimeSpan? GetDuration(TsPesPacket packet)
    {
      return packet.Duration;
    }
  }
}
