using System.Collections.Generic;
using LiteTube.StreamVideo.Pes;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.AAC
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
