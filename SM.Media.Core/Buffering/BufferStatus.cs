using System;

namespace SM.Media.Core.Buffering
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
