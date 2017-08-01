using System;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
    public class DefaultPesStreamHandler : PesStreamHandler
    {
        private readonly Action<TsPesPacket> _nextHandler;

        public override IConfigurationSource Configurator => null;

        public DefaultPesStreamHandler(PesStreamParameters parameters)
          : base(parameters)
        {
            if (null == parameters)
                throw new ArgumentNullException(nameof(parameters));
            if (null == parameters.NextHandler)
                throw new ArgumentException("NextHandler cannot be null", nameof(parameters));
            _nextHandler = parameters.NextHandler;
        }

        public override void PacketHandler(TsPesPacket packet)
        {
            base.PacketHandler(packet);
            _nextHandler(packet);
        }
    }
}
