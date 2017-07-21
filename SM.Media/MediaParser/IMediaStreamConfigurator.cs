﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaParser.MediaStreamSourceExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.MediaParser
{
  public static class MediaStreamSourceExtensions
  {
    public static Task PlayAsync(this IMediaStreamConfigurator mediaStreamConfigurator, IEnumerable<IMediaParserMediaStream> mediaParserMediaStreams, TimeSpan? duration, CancellationToken cancellationToken)
    {
      return mediaStreamConfigurator.PlayAsync(MediaParserMediaStreamExtensions.CreateMediaConfiguration(mediaParserMediaStreams, duration), cancellationToken);
    }
  }
}
