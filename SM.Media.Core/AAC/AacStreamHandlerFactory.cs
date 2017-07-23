using System.Collections.Generic;
using SM.Media.Core.Pes;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.AAC
{
    public class AacStreamHandlerFactory : IPesStreamFactoryInstance
    {
        private static readonly byte[] Types = new byte[1]
        {
            TsStreamType.AacStreamType
        };

        public ICollection<byte> SupportedStreamTypes
        {
            get { return Types; }
        }

        public PesStreamHandler Create(PesStreamParameters parameters)
        {
            return new AacStreamHandler(parameters);
        }
    }
}
