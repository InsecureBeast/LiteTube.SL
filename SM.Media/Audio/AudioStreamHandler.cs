// Decompiled with JetBrains decompiler
// Type: SM.Media.Audio.AudioStreamHandler
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using SM.Media.Pes;
using SM.Media.TransportStream.TsParser;
using SM.Media.TransportStream.TsParser.Utility;
using System;
using System.Diagnostics;

namespace SM.Media.Audio
{
  public abstract class AudioStreamHandler : PesStreamHandler
  {
    protected readonly IAudioConfigurator AudioConfigurator;
    protected readonly Action<TsPesPacket> NextHandler;
    private readonly IAudioFrameHeader _frameHeader;
    private readonly int _minimumPacketSize;
    private readonly ITsPesPacketPool _pesPacketPool;
    protected AudioParserBase Parser;
    private bool _isConfigured;

    public override IConfigurationSource Configurator
    {
      get
      {
        return (IConfigurationSource) this.AudioConfigurator;
      }
    }

    protected AudioStreamHandler(PesStreamParameters parameters, IAudioFrameHeader frameHeader, IAudioConfigurator configurator, int minimumPacketSize)
      : base(parameters)
    {
      if (null == parameters)
        throw new ArgumentNullException("parameters");
      if (null == parameters.PesPacketPool)
        throw new ArgumentException("PesPacketPool cannot be null", "parameters");
      if (null == parameters.NextHandler)
        throw new ArgumentException("NextHandler cannot be null", "parameters");
      if (minimumPacketSize < 1)
        throw new ArgumentOutOfRangeException("minimumPacketSize", "minimumPacketSize must be positive: " + (object) minimumPacketSize);
      if (null == frameHeader)
        throw new ArgumentNullException("frameHeader");
      this._pesPacketPool = parameters.PesPacketPool;
      this.NextHandler = parameters.NextHandler;
      this._frameHeader = frameHeader;
      this.AudioConfigurator = configurator;
      this._minimumPacketSize = minimumPacketSize;
    }

    public override TimeSpan? GetDuration(TsPesPacket packet)
    {
      if (packet.Duration.HasValue)
        return packet.Duration;
      TimeSpan timeSpan = TimeSpan.Zero;
      int length = packet.Length;
      int num1 = packet.Index + length;
      int num2 = 0;
      int index = packet.Index;
      while (index < num1)
      {
        int num3;
        if (this._frameHeader.Parse(packet.Buffer, index, length, false))
        {
          timeSpan += this._frameHeader.Duration;
          if (this._frameHeader.HeaderOffset > 0)
            Debug.WriteLine("AudioStreamHandler.GetDuration() skipping {0} bytes before frame", (object) this._frameHeader.HeaderOffset);
          num3 = this._frameHeader.HeaderOffset + this._frameHeader.FrameLength;
          num2 = 0;
        }
        else if (length > this._frameHeader.HeaderOffset + this._minimumPacketSize)
        {
          num3 = this._frameHeader.HeaderOffset + 1;
          num2 += num3;
        }
        else
        {
          Debug.WriteLine("AudioStreamHandler.GetDuration() unable to find frame, skipping {0} bytes", (object) (length + num2));
          break;
        }
        index += num3;
        length -= num3;
      }
      packet.Duration = new TimeSpan?(timeSpan);
      return new TimeSpan?(timeSpan);
    }

    public override void PacketHandler(TsPesPacket packet)
    {
      base.PacketHandler(packet);
      if (null == packet)
      {
        if (null != this.Parser)
          this.Parser.FlushBuffers();
        if (null == this.NextHandler)
          return;
        this.NextHandler((TsPesPacket) null);
      }
      else if (null != this.Parser)
      {
        this.Parser.Position = new TimeSpan?(packet.PresentationTimestamp);
        this.Parser.ProcessData(packet.Buffer, packet.Index, packet.Length);
        this._pesPacketPool.FreePesPacket(packet);
      }
      else if (packet.Length < this._minimumPacketSize)
      {
        this._pesPacketPool.FreePesPacket(packet);
      }
      else
      {
        if (!this._isConfigured && this._frameHeader.Parse(packet.Buffer, packet.Index, packet.Length, true))
        {
          this._isConfigured = true;
          this.AudioConfigurator.Configure(this._frameHeader);
        }
        this.NextHandler(packet);
      }
    }
  }
}
