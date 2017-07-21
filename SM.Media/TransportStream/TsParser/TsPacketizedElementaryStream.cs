// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.TsPacketizedElementaryStream
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.TransportStream.TsParser.Utility;
using System;
using System.Diagnostics;

namespace SM.Media.TransportStream.TsParser
{
  public class TsPacketizedElementaryStream
  {
    private const int DefaultPacketSize = 4096;
    private const int SystemClockHz = 90000;
    private const double PtsTo100ns = 111.111111111111;
    private readonly IBufferPool _bufferPool;
    private readonly Action<TsPesPacket> _handler;
    private readonly ITsPesPacketPool _pesPacketPool;
    private readonly TsStreamType _streamType;
    private BufferInstance _bufferEntry;
    private int _index;
    private uint _length;
    private uint _pid;
    private RegisterExtender _pts;
    private int _startIndex;
    private byte _streamId;

    public TsPacketizedElementaryStream(IBufferPool bufferPool, ITsPesPacketPool pesPacketPool, Action<TsPesPacket> packetHandler, TsStreamType streamType, uint pid)
    {
      if (null == bufferPool)
        throw new ArgumentNullException("bufferPool");
      if (null == pesPacketPool)
        throw new ArgumentNullException("pesPacketPool");
      this._bufferPool = bufferPool;
      this._pesPacketPool = pesPacketPool;
      this._streamType = streamType;
      this._pid = pid;
      this._handler = packetHandler;
    }

    public void Add(TsPacket packet)
    {
      if (null == this._handler)
        return;
      if (null != packet)
      {
        if (packet.IsStart)
        {
          if ((int) this._length == 0 && this._index > this._startIndex)
            this.Flush();
          this._index = this._startIndex;
          this.ParseHeader(packet);
        }
        if (null != this._bufferEntry)
        {
          if (this._index + packet.BufferLength > this._bufferEntry.Buffer.Length)
          {
            int minSize = Math.Max(this._index - this._startIndex + packet.BufferLength, 512);
            if (this._index < this._bufferEntry.Buffer.Length / 2)
              minSize *= 2;
            BufferInstance bufferInstance = this._bufferPool.Allocate(minSize);
            Array.Copy((Array) this._bufferEntry.Buffer, this._startIndex, (Array) bufferInstance.Buffer, 0, this._index - this._startIndex);
            this._bufferPool.Free(this._bufferEntry);
            this._bufferEntry = bufferInstance;
            this._index -= this._startIndex;
            this._startIndex = 0;
          }
          packet.CopyTo(this._bufferEntry.Buffer, this._index);
          this._index += packet.BufferLength;
        }
      }
      if (packet == null || this._length > 0U && (long) (this._index - this._startIndex) == (long) this._length)
        this.Flush();
      if (packet == null && null != this._bufferEntry)
      {
        this._bufferPool.Free(this._bufferEntry);
        this._bufferEntry = (BufferInstance) null;
      }
      if (packet != null || null == this._handler)
        return;
      this._handler((TsPesPacket) null);
    }

    private void ParseHeader(TsPacket packet)
    {
      if (packet.BufferLength < 6)
        return;
      int bufferOffset = packet.BufferOffset;
      int index1 = bufferOffset;
      byte[] buffer = packet.Buffer;
      uint num1 = (uint) ((int) buffer[index1] << 16 | (int) buffer[index1 + 1] << 8) | (uint) buffer[index1 + 2];
      int num2 = index1 + 3;
      if (1 != (int) num1)
        return;
      byte[] numArray = buffer;
      int index2 = num2;
      int num3 = 1;
      int index3 = index2 + num3;
      this._streamId = numArray[index2];
      uint num4 = (uint) buffer[index3] << 8 | (uint) buffer[index3 + 1];
      int num5 = index3 + 2;
      int minSize = 4096;
      if (num4 > 0U)
      {
        this._length = num4 + (uint) (num5 - bufferOffset);
        minSize = (int) this._length;
      }
      else
        this._length = 0U;
      if (this._bufferEntry != null && this._bufferEntry.Buffer.Length - this._startIndex < minSize)
      {
        this._bufferPool.Free(this._bufferEntry);
        this._bufferEntry = (BufferInstance) null;
      }
      if (null == this._bufferEntry)
      {
        this._bufferEntry = this._bufferPool.Allocate(minSize);
        this._startIndex = 0;
      }
      this._index = this._startIndex;
    }

    private void ParseNormalPesPacket()
    {
      if (null == this._handler)
        return;
      int num1 = 6 + this._startIndex;
      byte[] numArray1 = this._bufferEntry.Buffer;
      byte[] numArray2 = numArray1;
      int index1 = num1;
      int num2 = 1;
      int num3 = index1 + num2;
      byte num4 = numArray2[index1];
      if (128 != ((int) num4 & 192))
        return;
      byte num5 = (byte) ((int) num4 >> 4 & 3);
      bool flag1 = 0 != ((int) num4 & 8);
      bool flag2 = 0 != ((int) num4 & 4);
      bool flag3 = 0 != ((int) num4 & 2);
      bool flag4 = 0 != ((int) num4 & 1);
      byte[] numArray3 = numArray1;
      int index2 = num3;
      int num6 = 1;
      int num7 = index2 + num6;
      byte num8 = numArray3[index2];
      byte num9 = (byte) ((int) num8 >> 6 & 3);
      bool flag5 = 0 != ((int) num8 & 32);
      bool flag6 = 0 != ((int) num8 & 16);
      bool flag7 = 0 != ((int) num8 & 8);
      bool flag8 = 0 != ((int) num8 & 4);
      bool flag9 = 0 != ((int) num8 & 2);
      bool flag10 = 0 != ((int) num8 & 1);
      byte[] numArray4 = numArray1;
      int index3 = num7;
      int num10 = 1;
      int num11 = index3 + num10;
      byte num12 = numArray4[index3];
      int index4 = num11 + (int) num12;
      if (1 == (int) num9)
        return;
      ulong num13 = 0;
      ulong? dts = new ulong?();
      if (0 != (int) num9)
      {
        byte[] numArray5 = numArray1;
        int index5 = num11;
        int num14 = 1;
        int num15 = index5 + num14;
        byte num16 = numArray5[index5];
        if (2 == ((int) (byte) ((uint) num16 >> 4) & -2))
          ;
        if (0 == ((int) num16 & 1))
          return;
        long num17 = (long) ((ulong) ((int) num16 & 14) << 29);
        byte[] numArray6 = numArray1;
        int index6 = num15;
        int num18 = 1;
        int num19 = index6 + num18;
        long num20 = (long) ((uint) numArray6[index6] << 22);
        ulong num21 = (ulong) (num17 | num20);
        byte[] numArray7 = numArray1;
        int index7 = num19;
        int num22 = 1;
        int num23 = index7 + num22;
        byte num24 = numArray7[index7];
        if (0 == ((int) num24 & 1))
          return;
        long num25 = (long) (num21 | (ulong) (uint) (((int) num24 & 254) << 14));
        byte[] numArray8 = numArray1;
        int index8 = num23;
        int num26 = 1;
        int num27 = index8 + num26;
        long num28 = (long) ((uint) numArray8[index8] << 7);
        ulong num29 = (ulong) (num25 | num28);
        byte[] numArray9 = numArray1;
        int index9 = num27;
        int num30 = 1;
        num11 = index9 + num30;
        byte num31 = numArray9[index9];
        if (0 == ((int) num31 & 1))
          return;
        num13 = num29 | (ulong) ((uint) num31 >> 1);
        if (3 == (int) num9)
        {
          byte[] numArray10 = numArray1;
          int index10 = num11;
          int num32 = 1;
          int num33 = index10 + num32;
          byte num34 = numArray10[index10];
          if (1 == (int) (byte) ((uint) num34 >> 4))
            ;
          if (0 == ((int) num34 & 1))
            return;
          dts = new ulong?((ulong) ((int) num34 & 14) << 29);
          ulong? nullable = dts;
          byte[] numArray11 = numArray1;
          int index11 = num33;
          int num35 = 1;
          int num36 = index11 + num35;
          ulong num37 = (ulong) ((uint) numArray11[index11] << 22);
          dts = nullable.HasValue ? new ulong?(nullable.GetValueOrDefault() | num37) : new ulong?();
          byte[] numArray12 = numArray1;
          int index12 = num36;
          int num38 = 1;
          int num39 = index12 + num38;
          byte num40 = numArray12[index12];
          if (0 == ((int) num40 & 1))
            return;
          nullable = dts;
          ulong num41 = (ulong) (uint) (((int) num40 & 254) << 14);
          dts = nullable.HasValue ? new ulong?(nullable.GetValueOrDefault() | num41) : new ulong?();
          nullable = dts;
          byte[] numArray13 = numArray1;
          int index13 = num39;
          int num42 = 1;
          int num43 = index13 + num42;
          ulong num44 = (ulong) ((uint) numArray13[index13] << 7);
          dts = nullable.HasValue ? new ulong?(nullable.GetValueOrDefault() | num44) : new ulong?();
          byte[] numArray14 = numArray1;
          int index14 = num43;
          int num45 = 1;
          num11 = index14 + num45;
          byte num46 = numArray14[index14];
          if (0 == ((int) num46 & 1))
            return;
          nullable = dts;
          ulong num47 = (ulong) ((uint) num46 >> 1);
          dts = nullable.HasValue ? new ulong?(nullable.GetValueOrDefault() | num47) : new ulong?();
        }
      }
      if (flag5)
        num11 += 5;
      if (flag6)
        num11 += 3;
      if (flag7)
        ++num11;
      if (flag8)
        ++num11;
      if (flag9)
      {
        int num48 = num11 + 2;
      }
      if (!flag10)
        ;
      if (null == this._pts)
        this._pts = new RegisterExtender(num13, 33);
      else
        num13 = this._pts.Extend(num13);
      if (dts.HasValue)
        dts = new ulong?(this._pts.Extend(dts.Value));
      TsPesPacket packet = this.CreatePacket(index4, this._index - index4, num13, dts);
      this._startIndex = this._index;
      this._handler(packet);
    }

    private TimeSpan PtsToTimestamp(ulong pts)
    {
      return TimeSpan.FromTicks((long) Math.Round((double) pts * (1000.0 / 9.0)));
    }

    private TsPesPacket CreatePacket(int index, int length, ulong pts, ulong? dts)
    {
      Debug.Assert(length > 0);
      Debug.Assert(index >= 0);
      Debug.Assert(index + length <= this._bufferEntry.Buffer.Length);
      TsPesPacket tsPesPacket = this._pesPacketPool.AllocatePesPacket(this._bufferEntry);
      tsPesPacket.Index = index;
      tsPesPacket.Length = length;
      tsPesPacket.PresentationTimestamp = this.PtsToTimestamp(pts);
      tsPesPacket.DecodeTimestamp = dts.HasValue ? new TimeSpan?(this.PtsToTimestamp(dts.Value)) : new TimeSpan?();
      Debug.Assert(tsPesPacket.PresentationTimestamp >= TimeSpan.Zero);
      return tsPesPacket;
    }

    private void ParseDataPesPacket()
    {
      if (null == this._handler)
        return;
      TsPesPacket packet = this.CreatePacket(this._startIndex + 6, this._index - 6 - this._startIndex, 0UL, new ulong?());
      this._startIndex = this._index;
      this._handler(packet);
    }

    private void ParsePesPacket()
    {
      if (this._index - this._startIndex < 6)
        return;
      TsPacketizedElementaryStream.StreamId streamId = (TsPacketizedElementaryStream.StreamId) this._streamId;
      if ((uint) streamId <= 242U)
      {
        switch (streamId)
        {
          case TsPacketizedElementaryStream.StreamId.program_stream_map:
          case TsPacketizedElementaryStream.StreamId.private_stream_2:
          case TsPacketizedElementaryStream.StreamId.ECM_stream:
          case TsPacketizedElementaryStream.StreamId.EMM_stream:
          case TsPacketizedElementaryStream.StreamId.DSMCC_stream:
            break;
          case TsPacketizedElementaryStream.StreamId.padding_stream:
            return;
          default:
            goto label_7;
        }
      }
      else if (streamId != TsPacketizedElementaryStream.StreamId.itu_rec_h_222_1_E && streamId != TsPacketizedElementaryStream.StreamId.program_stream_directory)
        goto label_7;
      this.ParseDataPesPacket();
      return;
label_7:
      this.ParseNormalPesPacket();
    }

    private void Flush()
    {
      this.ParsePesPacket();
      this._startIndex = this._index;
    }

    public void Clear()
    {
      this._startIndex = 0;
      this._index = 0;
      this._length = 0U;
      if (null == this._bufferEntry)
        return;
      this._bufferPool.Free(this._bufferEntry);
      this._bufferEntry = (BufferInstance) null;
    }

    public void FlushBuffers()
    {
      this.Clear();
    }

    private enum StreamId : byte
    {
      program_stream_map = 188,
      private_stream_1 = 189,
      padding_stream = 190,
      private_stream_2 = 191,
      ECM_stream = 240,
      EMM_stream = 241,
      DSMCC_stream = 242,
      iso_13522_stream = 243,
      itu_rec_h_222_1_A = 244,
      itu_rec_h_222_1_B = 245,
      itu_rec_h_222_1_C = 246,
      itu_rec_h_222_1_D = 247,
      itu_rec_h_222_1_E = 248,
      ancillary_stream = 249,
      iso_14496_1_packetized_stream = 250,
      iso_14496_1_FlexMux_stream = 251,
      metadata_stream = 252,
      extended_stream_id = 253,
      reserved_data_stream = 254,
      program_stream_directory = 255,
    }
  }
}
