using System;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
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
