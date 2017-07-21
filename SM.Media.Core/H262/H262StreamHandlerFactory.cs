using System.Collections.Generic;
using SM.Media.Core.Pes;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.H262
{
  public class H262StreamHandlerFactory : IPesStreamFactoryInstance
  {
    private static readonly byte[] Types = new byte[1]
    {
      TsStreamType.H262StreamType
    };

    public ICollection<byte> SupportedStreamTypes
    {
      get
      {
        return (ICollection<byte>) H262StreamHandlerFactory.Types;
      }
    }

    public PesStreamHandler Create(PesStreamParameters parameters)
    {
      return (PesStreamHandler) new H262StreamHandler(parameters);
    }
  }
}
