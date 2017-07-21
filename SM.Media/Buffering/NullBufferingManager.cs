// Decompiled with JetBrains decompiler
// Type: SM.Media.Buffering.NullBufferingManager
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.TransportStream.TsParser;
using SM.Media.TransportStream.TsParser.Utility;
using System;

namespace SM.Media.Buffering
{
  public sealed class NullBufferingManager : IBufferingManager, IDisposable
  {
    private readonly ITsPesPacketPool _packetPool;

    public float? BufferingProgress
    {
      get
      {
        return new float?();
      }
    }

    public bool IsBuffering
    {
      get
      {
        return false;
      }
    }

    public NullBufferingManager(ITsPesPacketPool packetPool)
    {
      if (null == packetPool)
        throw new ArgumentNullException("packetPool");
      this._packetPool = packetPool;
    }

    public IStreamBuffer CreateStreamBuffer(TsStreamType streamType)
    {
      return (IStreamBuffer) new StreamBuffer(streamType, new Action<TsPesPacket>(this._packetPool.FreePesPacket), (IBufferingManager) this);
    }

    public void Flush()
    {
    }

    public bool IsSeekAlreadyBuffered(TimeSpan position)
    {
      return true;
    }

    public void Initialize(IQueueThrottling queueThrottling, Action reportBufferingChange)
    {
    }

    public void Shutdown(IQueueThrottling queueThrottling)
    {
    }

    public void Refresh()
    {
    }

    public void ReportExhaustion()
    {
    }

    public void ReportEndOfData()
    {
    }

    public void Dispose()
    {
    }
  }
}
