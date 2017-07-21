using System;
using System.Threading;
using SM.Media.Core.Metadata;

namespace SM.Media.Core.Utility
{
  public class WorkBuffer
  {
    public readonly int Sequence = Interlocked.Increment(ref WorkBuffer._sequenceCounter);
    private const int DefaultBufferSize = 32712;
    public readonly byte[] Buffer;
    public int Length;
    public ISegmentMetadata Metadata;
    private static int _sequenceCounter;
    public int ReadCount;

    public WorkBuffer()
      : this(32712)
    {
    }

    public WorkBuffer(int bufferSize)
    {
      if (bufferSize < 1)
        throw new ArgumentException("The buffer size must be positive", "bufferSize");
      this.Buffer = new byte[bufferSize];
    }

    public override string ToString()
    {
      return string.Format("WorkBuffer({0}) count {1} length {2}/{3}", (object) this.Sequence, (object) this.ReadCount, (object) this.Length, (object) this.Buffer.Length);
    }
  }
}
