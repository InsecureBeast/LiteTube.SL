using System.Collections.Generic;
using SM.Media.Core.Pes;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.Ac3
{
  public class Ac3StreamHandlerFactory : IPesStreamFactoryInstance
  {
    private static readonly byte[] Types = new byte[1]
    {
      TsStreamType.Ac3StreamType
    };

    public ICollection<byte> SupportedStreamTypes
    {
      get
      {
        return (ICollection<byte>) Ac3StreamHandlerFactory.Types;
      }
    }

    public PesStreamHandler Create(PesStreamParameters parameters)
    {
      return (PesStreamHandler) new Ac3StreamHandler(parameters);
    }
  }
}
