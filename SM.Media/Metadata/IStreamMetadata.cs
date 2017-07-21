// Decompiled with JetBrains decompiler
// Type: SM.Media.Metadata.IStreamMetadata
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;

namespace SM.Media.Metadata
{
  public interface IStreamMetadata
  {
    Uri Url { get; }

    ContentType ContentType { get; }

    int? Bitrate { get; }

    TimeSpan? Duration { get; }

    string Name { get; }

    string Description { get; }

    string Genre { get; }

    Uri Website { get; }
  }
}
