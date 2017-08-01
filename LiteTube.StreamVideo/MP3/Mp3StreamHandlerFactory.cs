using System.Collections.Generic;
using LiteTube.StreamVideo.Pes;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.MP3
{
    public class Mp3StreamHandlerFactory : IPesStreamFactoryInstance
    {
        private static readonly byte[] Types =  { TsStreamType.Mp3Iso11172, TsStreamType.Mp3Iso13818 };

        public ICollection<byte> SupportedStreamTypes => Types;

        public PesStreamHandler Create(PesStreamParameters parameters)
        {
            return new Mp3StreamHandler(parameters);
        }
    }
}
