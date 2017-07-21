// Decompiled with JetBrains decompiler
// Type: SM.Media.Buffering.BufferStatus
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Buffering
{
  public class BufferStatus
  {
    public int Size { get; set; }

    public TimeSpan? Newest { get; set; }

    public TimeSpan? Oldest { get; set; }

    public int PacketCount { get; set; }

    public bool IsDone { get; set; }

    public bool IsValid { get; set; }

    public bool IsMedia { get; set; }
  }
}
