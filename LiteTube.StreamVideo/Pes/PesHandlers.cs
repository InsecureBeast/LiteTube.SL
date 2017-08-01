using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
  public sealed class PesHandlers : IPesHandlers, IDisposable
  {
    private readonly Dictionary<uint, PesStreamHandler> _handlers = new Dictionary<uint, PesStreamHandler>();
    private readonly Dictionary<byte, Func<uint, TsStreamType, Action<TsPesPacket>>> _pesStreamHandlerFactory = new Dictionary<byte, Func<uint, TsStreamType, Action<TsPesPacket>>>();
    private readonly IPesHandlerFactory _handlerFactory;
    private readonly Func<PesStreamParameters> _parameterFactory;

    public PesHandlers(IPesHandlerFactory handlerFactory, Func<PesStreamParameters> parameterFactory)
    {
      if (null == handlerFactory)
        throw new ArgumentNullException("handlerFactory");
      if (null == parameterFactory)
        throw new ArgumentNullException("parameterFactory");
      this._handlerFactory = handlerFactory;
      this._parameterFactory = parameterFactory;
    }

    public void Dispose()
    {
      this.CleanupHandlers();
    }

    public PesStreamHandler GetPesHandler(TsStreamType streamType, uint pid, IMediaStreamMetadata mediaStreamMetadata, Action<TsPesPacket> nextHandler)
    {
      PesStreamHandler handler;
      if (this._handlers.TryGetValue(pid, out handler))
      {
        Debug.WriteLine("Found PES {0} stream ({1}) with PID {2}", (object) streamType.Contents, (object) streamType.Description, (object) pid);
      }
      else
      {
        Debug.WriteLine("Create PES {0} stream ({1}) with PID {2}", (object) streamType.Contents, (object) streamType.Description, (object) pid);
        PesStreamParameters parameters = this._parameterFactory();
        parameters.Pid = pid;
        parameters.StreamType = streamType;
        parameters.NextHandler = nextHandler;
        parameters.MediaStreamMetadata = mediaStreamMetadata;
        handler = this._handlerFactory.CreateHandler(parameters);
        this._handlers[pid] = handler;
      }
      return handler;
    }

    public void Initialize()
    {
      this.CleanupHandlers();
    }

    private void CleanupHandlers()
    {
      if (null == this._handlers)
        return;
      this._handlers.Clear();
    }

    public void RegisterHandlerFactory(byte streamType, Func<uint, TsStreamType, Action<TsPesPacket>> handlerFactory)
    {
      this._pesStreamHandlerFactory[streamType] = handlerFactory;
    }
  }
}
