using System;
using System.Collections.Generic;
using System.Diagnostics;
using SM.Media.Core.Metadata;

namespace SM.Media.Core.TransportStream.TsParser
{
  public sealed class TsDecoder : ITsDecoder, IDisposable
  {
    private readonly Dictionary<uint, Action<TsPacket>> _packetHandlers = new Dictionary<uint, Action<TsPacket>>();
    private readonly TsPacket _tsPacket = new TsPacket();
    private volatile bool _enableProcessing = true;
    private readonly byte[] _destinationArray;
    private readonly int _packetSize;
    private readonly ITsProgramAssociationTableFactory _programAssociationTableFactory;
    private int _destinationLength;
    private Func<TsStreamType, uint, IMediaStreamMetadata, TsPacketizedElementaryStream> _pesStreamFactory;
    private TsProgramAssociationTable _programAssociationTable;
    private int _tsIndex;

    public Action<TsPacket> PacketMonitor { get; set; }

    public bool EnableProcessing
    {
      get
      {
        return this._enableProcessing;
      }
      set
      {
        this._enableProcessing = value;
      }
    }

    public TsDecoder(ITsProgramAssociationTableFactory programAssociationTableFactory)
    {
      if (null == programAssociationTableFactory)
        throw new ArgumentNullException("programAssociationTableFactory");
      this._programAssociationTableFactory = programAssociationTableFactory;
      this._packetSize = 188;
      this._destinationArray = new byte[this._packetSize * 174];
    }

    public void Dispose()
    {
      this.Clear();
    }

    public void Initialize(Func<TsStreamType, uint, IMediaStreamMetadata, TsPacketizedElementaryStream> pesStreamFactory, Action<IProgramStreams> programStreamsHandler = null)
    {
      if (pesStreamFactory == null)
        throw new ArgumentNullException("pesStreamFactory");
      this._pesStreamFactory = pesStreamFactory;
      this.Clear();
      this._programAssociationTable = this._programAssociationTableFactory.Create((ITsDecoder) this, (Func<int, bool>) (program => true), programStreamsHandler);
      this._packetHandlers[0U] = new Action<TsPacket>(((TsProgramSpecificInformation) this._programAssociationTable).Add);
      this._tsIndex = 0;
    }

    public void FlushBuffers()
    {
      if (null != this._programAssociationTable)
        this._programAssociationTable.FlushBuffers();
      this._destinationLength = 0;
    }

    public void ParseEnd()
    {
      this.Parse((byte[]) null, 0, 0);
      foreach (Action<TsPacket> action in this._packetHandlers.Values)
        action((TsPacket) null);
    }

    public void Parse(byte[] buffer, int offset, int length)
    {
      if (!this.EnableProcessing)
        return;
      if (this._destinationLength > 0)
      {
        int num = this._destinationLength % this._packetSize;
        if (num > 0)
        {
          int length1 = Math.Min(this._packetSize - num, length);
          if (length1 > 0)
          {
            Array.Copy((Array) buffer, offset, (Array) this._destinationArray, this._destinationLength, length1);
            offset += length1;
            length -= length1;
            this._destinationLength += length1;
          }
        }
        if (!this.ParseBuffer())
        {
          Debug.Assert(0 == length);
          return;
        }
      }
      int index = offset;
      while (this.EnableProcessing && index <= offset + length - this._packetSize)
      {
        if (71 != (int) buffer[index] || !this.ParsePacket(buffer, index))
          ++index;
        else
          index += this._packetSize;
      }
      while (index < offset + length && 71 != (int) buffer[index])
        ++index;
      this._destinationLength = length - (index - offset);
      if (this._destinationLength <= 0)
        return;
      Array.Copy((Array) buffer, index, (Array) this._destinationArray, 0, this._destinationLength);
    }

    public void RegisterHandler(uint pid, Action<TsPacket> handler)
    {
      this._packetHandlers[pid] = handler;
    }

    public void UnregisterHandler(uint pid)
    {
      this._packetHandlers.Remove(pid);
    }

    public TsPacketizedElementaryStream CreateStream(TsStreamType streamType, uint pid, IMediaStreamMetadata mediaStreamMetadata)
    {
      return this._pesStreamFactory(streamType, pid, mediaStreamMetadata);
    }

    private void Clear()
    {
      if (null != this._programAssociationTable)
      {
        this._programAssociationTable.Clear();
        this._programAssociationTable = (TsProgramAssociationTable) null;
      }
      this._packetHandlers.Clear();
      this._destinationLength = 0;
    }

    private bool ParseBuffer()
    {
      int num = 0;
      while (this._destinationLength >= this._packetSize)
      {
        this.ParsePacket(this._destinationArray, num);
        num += this._packetSize;
        this._destinationLength -= this._packetSize;
      }
      if (this._destinationLength > 0)
        Array.Copy((Array) this._destinationArray, num, (Array) this._destinationArray, 0, this._destinationLength);
      return 0 == this._destinationLength;
    }

    private bool ParsePacket(byte[] buffer, int offset)
    {
      if (!this._tsPacket.Parse(this._tsIndex, buffer, offset))
        return false;
      this._tsIndex += this._packetSize;
      if (this._tsPacket.IsSkip)
        return true;
      Action<TsPacket> action;
      if (this._packetHandlers.TryGetValue(this._tsPacket.Pid, out action))
        action(this._tsPacket);
      Action<TsPacket> packetMonitor = this.PacketMonitor;
      if (null != packetMonitor)
        packetMonitor(this._tsPacket);
      return true;
    }
  }
}
