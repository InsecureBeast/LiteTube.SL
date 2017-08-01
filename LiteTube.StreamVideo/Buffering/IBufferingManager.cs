using System;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Buffering
{
  public interface IBufferingManager : IDisposable
  {
    float? BufferingProgress { get; }

    bool IsBuffering { get; }

    IStreamBuffer CreateStreamBuffer(TsStreamType streamType);

    void Flush();

    bool IsSeekAlreadyBuffered(TimeSpan position);

    void Initialize(IQueueThrottling queueThrottling, Action reportBufferingChange);

    void Shutdown(IQueueThrottling queueThrottling);

    void Refresh();

    void ReportExhaustion();

    void ReportEndOfData();
  }
}
