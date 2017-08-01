using System.Collections.Generic;
using LiteTube.StreamVideo.Pes;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.H264
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
