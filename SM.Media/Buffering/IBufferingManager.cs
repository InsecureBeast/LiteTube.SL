// Decompiled with JetBrains decompiler
// Type: SM.Media.Buffering.IBufferingManager
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.TransportStream.TsParser;
using System;

namespace SM.Media.Buffering
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
