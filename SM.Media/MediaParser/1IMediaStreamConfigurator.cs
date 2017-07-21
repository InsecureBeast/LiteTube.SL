// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaParser.IMediaStreamConfigurator
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.MediaManager;
using SM.Media.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.MediaParser
{
  public interface IMediaStreamConfigurator : IDisposable
  {
    TimeSpan? SeekTarget { get; set; }

    IMediaManager MediaManager { get; set; }

    void Initialize();

    Task<TMediaStreamSource> CreateMediaStreamSourceAsync<TMediaStreamSource>(CancellationToken cancellationToken) where TMediaStreamSource : class;

    Task PlayAsync(IMediaConfiguration configuration, CancellationToken cancellationToken);

    Task CloseAsync();

    void ReportError(string message);

    void CheckForSamples();

    void ValidateEvent(MediaStreamFsm.MediaEvent mediaEvent);
  }
}
