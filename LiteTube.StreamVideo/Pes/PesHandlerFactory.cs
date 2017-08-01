using System;
using System.Collections.Generic;
using System.Linq;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
  public sealed class PesHandlerFactory : IPesHandlerFactory
  {
    private static readonly IDictionary<byte, ContentType> TsStreamTypeContentTypes = (IDictionary<byte, ContentType>) new Dictionary<byte, ContentType>()
    {
      {
        TsStreamType.H262StreamType,
        ContentTypes.H262
      },
      {
        TsStreamType.Mp3Iso11172,
        ContentTypes.Mp3
      },
      {
        TsStreamType.Mp3Iso13818,
        ContentTypes.Mp3
      },
      {
        TsStreamType.H264StreamType,
        ContentTypes.H264
      },
      {
        TsStreamType.AacStreamType,
        ContentTypes.Aac
      },
      {
        TsStreamType.Ac3StreamType,
        ContentTypes.Ac3
      }
    };
    private readonly Dictionary<byte, IPesStreamFactoryInstance> _factories;

    public PesHandlerFactory(IEnumerable<IPesStreamFactoryInstance> factoryInstances)
    {
      if (factoryInstances == null)
        throw new ArgumentNullException("factoryInstances");
      this._factories = Enumerable.ToDictionary(Enumerable.SelectMany(factoryInstances, (Func<IPesStreamFactoryInstance, IEnumerable<byte>>) (fi => (IEnumerable<byte>) fi.SupportedStreamTypes), (fi, contentType) =>
      {
        var fAnonymousType0 = new
        {
          ContentType = contentType,
          Instance = fi
        };
        return fAnonymousType0;
      }), v => v.ContentType, v => v.Instance);
    }

    public PesStreamHandler CreateHandler(PesStreamParameters parameters)
    {
      IPesStreamFactoryInstance streamFactoryInstance;
      if (!this._factories.TryGetValue(parameters.StreamType.StreamType, out streamFactoryInstance))
        return (PesStreamHandler) new DefaultPesStreamHandler(parameters);
      return streamFactoryInstance.Create(parameters);
    }
  }
}
