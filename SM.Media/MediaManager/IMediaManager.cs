// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaManager.IMediaManager
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using SM.Media.MediaParser;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.MediaManager
{
  public interface IMediaManager : IDisposable
  {
    TimeSpan? SeekTarget { get; set; }

    MediaManagerState State { get; }

    ContentType ContentType { get; set; }

    Task PlayingTask { get; }

    event EventHandler<MediaManagerStateEventArgs> OnStateChange;

    Task<IMediaStreamConfigurator> OpenMediaAsync(ICollection<Uri> source, CancellationToken cancellationToken);

    Task StopMediaAsync(CancellationToken cancellationToken);

    Task CloseMediaAsync();

    Task<TimeSpan> SeekMediaAsync(TimeSpan position);
  }
}
