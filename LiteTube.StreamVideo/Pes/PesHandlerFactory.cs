using System;
using System.Collections.Generic;
using System.Linq;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
    public sealed class PesHandlerFactory : IPesHandlerFactory
    {
        private readonly Dictionary<byte, IPesStreamFactoryInstance> _factories;

        public PesHandlerFactory(IEnumerable<IPesStreamFactoryInstance> factoryInstances)
        {
            if (factoryInstances == null)
                throw new ArgumentNullException(nameof(factoryInstances));

            _factories = factoryInstances.SelectMany(fi => fi.SupportedStreamTypes, (fi, contentType) =>
            {
                var t = new
                {
                    ContentType = contentType,
                    Instance = fi
                };
                return t;
            }).ToDictionary(v => v.ContentType, v => v.Instance);
        }

        public PesStreamHandler CreateHandler(PesStreamParameters parameters)
        {
            IPesStreamFactoryInstance streamFactoryInstance;
            return !_factories.TryGetValue(parameters.StreamType.StreamType, out streamFactoryInstance) 
                ? new DefaultPesStreamHandler(parameters) 
                : streamFactoryInstance.Create(parameters);
        }
    }
}
