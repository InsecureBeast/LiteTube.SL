// Decompiled with JetBrains decompiler
// Type: SM.Media.Buffering.IBufferingPolicy
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Buffering
{
  public interface IBufferingPolicy
  {
    bool ShouldBlockReads(bool isReadBlocked, TimeSpan durationBuffered, int bytesBuffered, bool isExhausted, bool isAllExhausted);

    bool IsDoneBuffering(TimeSpan bufferDuration, int bytesBuffered, int bytesBufferedWhenExhausted, bool isStarting);

    float GetProgress(TimeSpan bufferDuration, int bytesBuffered, int bytesBufferedWhenExhausted, bool isStarting);
  }
}
