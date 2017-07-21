// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.PoolBufferInstance
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.TransportStream.TsParser.Utility;
using System.Diagnostics;
using System.Threading;

namespace SM.Media.Utility
{
  internal sealed class PoolBufferInstance : BufferInstance
  {
    private readonly int _bufferEntryId = Interlocked.Increment(ref PoolBufferInstance._bufferEntryCount);
    private static int _bufferEntryCount;
    private int _allocationCount;

    public PoolBufferInstance(int size)
      : base(new byte[size])
    {
    }

    public override void Reference()
    {
      Debug.Assert(this._allocationCount >= 0);
      Interlocked.Increment(ref this._allocationCount);
    }

    public override bool Dereference()
    {
      Debug.Assert(this._allocationCount > 0);
      return 0 == Interlocked.Decrement(ref this._allocationCount);
    }

    public override string ToString()
    {
      return string.Format("Buffer({0}) {1} bytes {2} refs", (object) this._bufferEntryId, (object) this.Buffer.Length, (object) this._allocationCount);
    }
  }
}
