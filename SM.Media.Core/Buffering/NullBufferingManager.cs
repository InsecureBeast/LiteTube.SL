using System;
using SM.Media.Core.TransportStream.TsParser;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.Buffering
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
