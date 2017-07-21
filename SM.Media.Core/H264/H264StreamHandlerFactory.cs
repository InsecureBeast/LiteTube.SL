using System.Collections.Generic;
using SM.Media.Core.Pes;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.H264
{
  public class H264StreamHandlerFactory : IPesStreamFactoryInstance
  {
    private static readonly byte[] Types = new byte[1]
    {
      TsStreamType.H264StreamType
    };

    public ICollection<byte> SupportedStreamTypes
    {
      get
      {
        return (ICollection<byte>) H264StreamHandlerFactory.Types;
      }
    }

    public PesStreamHandler Create(PesStreamParameters parameters)
    {
      return (PesStreamHandler) new H264StreamHandler(parameters);
    }
  }
}
