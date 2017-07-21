// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.WorkBuffer
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using System;
using System.Threading;

namespace SM.Media.Utility
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
