using System;

namespace SM.Media.Core.Audio
{
  public interface IAudioParser : IDisposable
  {
    TimeSpan StartPosition { get; set; }

    TimeSpan? Position { get; set; }

    void FlushBuffers();

    void ProcessData(byte[] buffer, int offset, int length);
  }
}
