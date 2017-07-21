// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.H264StreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using SM.Media.Pes;
using SM.Media.TransportStream.TsParser;
using SM.Media.TransportStream.TsParser.Utility;
using System;
using System.Collections.Generic;

namespace SM.Media.H264
{
  public sealed class H264StreamHandler : PesStreamHandler
  {
    private readonly RbspDecoder _rbspDecoder = new RbspDecoder();
    private readonly H264Configurator _configurator;
    private readonly Action<TsPesPacket> _nextHandler;
    private readonly NalUnitParser _parser;
    private readonly ITsPesPacketPool _pesPacketPool;
    private INalParser _currentParser;
    private bool _isConfigured;

    public override IConfigurationSource Configurator
    {
      get
      {
        return (IConfigurationSource) this._configurator;
      }
    }

    public H264StreamHandler(PesStreamParameters parameters)
      : base(parameters)
    {
      if (null == parameters)
        throw new ArgumentNullException("parameters");
      if (null == parameters.PesPacketPool)
        throw new ArgumentException("PesPacketPool cannot be null", "parameters");
      if (null == parameters.NextHandler)
        throw new ArgumentException("NextHandler cannot be null", "parameters");
      this._pesPacketPool = parameters.PesPacketPool;
      this._nextHandler = parameters.NextHandler;
      this._configurator = new H264Configurator(parameters.MediaStreamMetadata, parameters.StreamType.Description);
      this._parser = new NalUnitParser(new Func<byte, NalUnitParser.ParserStateHandler>(this.ResolveHandler));
    }

    private NalUnitParser.ParserStateHandler ResolveHandler(byte arg)
    {
      switch ((int) arg & 31)
      {
        case 1:
        case 2:
        case 5:
          this._rbspDecoder.CompletionHandler = new Action<IList<byte>>(this._configurator.ParseSliceHeader);
          this._currentParser = (INalParser) this._rbspDecoder;
          break;
        case 6:
          this._rbspDecoder.CompletionHandler = new Action<IList<byte>>(this._configurator.ParseSei);
          this._currentParser = (INalParser) this._rbspDecoder;
          break;
        case 7:
          this._rbspDecoder.CompletionHandler = new Action<IList<byte>>(this._configurator.ParseSpsBytes);
          this._currentParser = (INalParser) this._rbspDecoder;
          break;
        case 8:
          this._rbspDecoder.CompletionHandler = new Action<IList<byte>>(this._configurator.ParsePpsBytes);
          this._currentParser = (INalParser) this._rbspDecoder;
          break;
        default:
          this._currentParser = (INalParser) null;
          return (NalUnitParser.ParserStateHandler) null;
      }
      if (null == this._currentParser)
        return (NalUnitParser.ParserStateHandler) null;
      return new NalUnitParser.ParserStateHandler(this._currentParser.Parse);
    }

    public override void PacketHandler(TsPesPacket packet)
    {
      base.PacketHandler(packet);
      if (null == packet)
        this._nextHandler((TsPesPacket) null);
      else if (packet.Length < 1)
      {
        this._pesPacketPool.FreePesPacket(packet);
      }
      else
      {
        if (!this._isConfigured)
        {
          this._parser.Reset();
          this._parser.Parse(packet.Buffer, packet.Index, packet.Length, true);
          this._isConfigured = this._configurator.IsConfigured;
        }
        this._nextHandler(packet);
      }
    }
  }
}
