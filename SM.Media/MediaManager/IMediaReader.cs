// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaManager.IMediaReader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.MediaParser;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.MediaManager
{
  internal interface IMediaReader : IDisposable
  {
    bool IsConfigured { get; }

    bool IsEnabled { get; set; }

    ICollection<IMediaParserMediaStream> MediaStreams { get; }

    Task<long> ReadAsync(CancellationToken cancellationToken);

    Task CloseAsync();

    Task StopAsync();

    bool IsBuffered(TimeSpan position);
  }
}
