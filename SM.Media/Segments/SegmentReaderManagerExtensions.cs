﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.SegmentReaderManagerExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
{
  internal static class SegmentReaderManagerExtensions
  {
    public static async Task<TimeSpan> StartAsync(this ISegmentReaderManager segmentManager, CancellationToken cancellationToken)
    {
      await segmentManager.StartAsync().ConfigureAwait(false);
      return await segmentManager.SeekAsync(TimeSpan.Zero, cancellationToken).ConfigureAwait(false);
    }
  }
}
