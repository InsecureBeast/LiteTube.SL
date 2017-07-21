using System.Collections.Generic;
using SM.Media.Core.Pes;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.MP3
{
  public class Mp3StreamHandlerFactory : IPesStreamFactoryInstance
  {
    private static readonly byte[] Types = new byte[2]
    {
      TsStreamType.Mp3Iso11172,
      TsStreamType.Mp3Iso13818
    };

    public ICollection<byte> SupportedStreamTypes
    {
      get
      {
        return (ICollection<byte>) Mp3StreamHandlerFactory.Types;
      }
    }

    public PesStreamHandler Create(PesStreamParameters parameters)
    {
      return (PesStreamHandler) new Mp3StreamHandler(parameters);
    }
  }
}
