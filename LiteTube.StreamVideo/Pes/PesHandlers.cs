using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
    public sealed class PesHandlers : IPesHandlers
    {
        private readonly Dictionary<uint, PesStreamHandler> _handlers = new Dictionary<uint, PesStreamHandler>();
        private readonly Dictionary<byte, Func<uint, TsStreamType, Action<TsPesPacket>>> _pesStreamHandlerFactory = new Dictionary<byte, Func<uint, TsStreamType, Action<TsPesPacket>>>();
        private readonly IPesHandlerFactory _handlerFactory;
        private readonly Func<PesStreamParameters> _parameterFactory;

        public PesHandlers(IPesHandlerFactory handlerFactory, Func<PesStreamParameters> parameterFactory)
        {
            if (null == handlerFactory)
                throw new ArgumentNullException(nameof(handlerFactory));
            if (null == parameterFactory)
                throw new ArgumentNullException(nameof(parameterFactory));
            _handlerFactory = handlerFactory;
            _parameterFactory = parameterFactory;
        }

        public void Dispose()
        {
            CleanupHandlers();
        }

        public PesStreamHandler GetPesHandler(TsStreamType streamType, uint pid, IMediaStreamMetadata mediaStreamMetadata, Action<TsPesPacket> nextHandler)
        {
            PesStreamHandler handler;
            if (_handlers.TryGetValue(pid, out handler))
            {
                Debug.WriteLine("Found PES {0} stream ({1}) with PID {2}", streamType.Contents, streamType.Description, pid);
            }
            else
            {
                Debug.WriteLine("Create PES {0} stream ({1}) with PID {2}", streamType.Contents, streamType.Description, pid);
                var parameters = _parameterFactory();
                parameters.Pid = pid;
                parameters.StreamType = streamType;
                parameters.NextHandler = nextHandler;
                parameters.MediaStreamMetadata = mediaStreamMetadata;
                handler = _handlerFactory.CreateHandler(parameters);
                _handlers[pid] = handler;
            }
            return handler;
        }

        public void Initialize()
        {
            CleanupHandlers();
        }

        private void CleanupHandlers()
        {
            _handlers?.Clear();
        }

        public void RegisterHandlerFactory(byte streamType, Func<uint, TsStreamType, Action<TsPesPacket>> handlerFactory)
        {
            _pesStreamHandlerFactory[streamType] = handlerFactory;
        }
    }
}
