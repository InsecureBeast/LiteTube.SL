// Decompiled with JetBrains decompiler
// Type: SM.Media.Pes.PesHandlers
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using SM.Media.TransportStream.TsParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SM.Media.Pes
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
