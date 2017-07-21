using System.Diagnostics;
using System.Threading;
using SM.Media.Core.TransportStream.TsParser.Utility;

namespace SM.Media.Core.Utility
{
  internal sealed class PoolBufferInstance : BufferInstance
  {
    private readonly int _bufferEntryId = Interlocked.Increment(ref _bufferEntryCount);
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
