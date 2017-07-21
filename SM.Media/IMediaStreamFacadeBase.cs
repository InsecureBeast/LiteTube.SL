// Decompiled with JetBrains decompiler
// Type: SM.Media.IMediaStreamFacadeBase
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Builder;
using SM.Media.Content;
using SM.Media.MediaManager;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media
{
  public interface IMediaStreamFacadeBase : IDisposable
  {
    ContentType ContentType { get; set; }

    TimeSpan? SeekTarget { get; set; }

    MediaManagerState State { get; }

    IBuilder<IMediaManager> Builder { get; }

    bool IsDisposed { get; }

    Task PlayingTask { get; }

    event EventHandler<MediaManagerStateEventArgs> StateChange;

    Task StopAsync(CancellationToken cancellationToken);

    Task CloseAsync();
  }
}
