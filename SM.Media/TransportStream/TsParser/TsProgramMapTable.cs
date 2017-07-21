// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.TsProgramMapTable
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using SM.Media.TransportStream.TsParser.Descriptor;
using SM.Media.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SM.Media.TransportStream.TsParser
{
  public class TsProgramMapTable : TsProgramSpecificInformation
  {
    private readonly Dictionary<uint, TsProgramMapTable.ProgramMap> _newProgramStreams = new Dictionary<uint, TsProgramMapTable.ProgramMap>();
    private readonly Dictionary<uint, TsProgramMapTable.ProgramMap> _programStreamMap = new Dictionary<uint, TsProgramMapTable.ProgramMap>();
    private readonly Dictionary<Tuple<uint, TsStreamType>, TsProgramMapTable.ProgramMap> _retiredProgramStreams = new Dictionary<Tuple<uint, TsStreamType>, TsProgramMapTable.ProgramMap>();
    private readonly List<TsProgramMapTable.ProgramMap> _streamList = new List<TsProgramMapTable.ProgramMap>();
    private const int MinimumProgramMapSize = 9;
    private readonly ITsDecoder _decoder;
    private readonly ITsDescriptorFactory _descriptorFactory;
    private readonly uint _pid;
    private readonly int _programNumber;
    private readonly Action<IProgramStreams> _streamFilter;
    private bool _foundPcrPid;
    private TsDescriptor[] _newProgramDescriptors;
    private ulong? _pcr;
    private int? _pcrIndex;
    private uint _pcrPid;

    public TsProgramMapTable(ITsDecoder decoder, ITsDescriptorFactory descriptorFactory, int programNumber, uint pid, Action<IProgramStreams> streamFilter)
      : base(TsProgramSpecificInformation.TsTableId.TS_program_map_section)
    {
      if (null == decoder)
        throw new ArgumentNullException("decoder");
      if (null == descriptorFactory)
        throw new ArgumentNullException("descriptorFactory");
      this._decoder = decoder;
      this._descriptorFactory = descriptorFactory;
      this._programNumber = programNumber;
      this._pid = pid;
      this._streamFilter = streamFilter;
    }

    protected override void ParseSection(TsPacket packet, int offset, int length)
    {
      if (length < 9)
        return;
      int index1 = offset;
      byte[] buffer = packet.Buffer;
      int num1 = (int) buffer[index1] << 8 | (int) buffer[index1 + 1];
      int num2 = index1 + 2;
      byte[] numArray1 = buffer;
      int index2 = num2;
      int num3 = 1;
      int num4 = index2 + num3;
      byte num5 = numArray1[index2];
      bool flag = 0 != ((int) num5 & 1);
      byte num6 = (byte) ((uint) (byte) ((uint) num5 >> 1) & 31U);
      byte[] numArray2 = buffer;
      int index3 = num4;
      int num7 = 1;
      int num8 = index3 + num7;
      byte num9 = numArray2[index3];
      byte[] numArray3 = buffer;
      int index4 = num8;
      int num10 = 1;
      int index5 = index4 + num10;
      byte num11 = numArray3[index4];
      if ((int) num11 < (int) num9)
        return;
      uint num12 = (uint) buffer[index5] << 8 | (uint) buffer[index5 + 1];
      int index6 = index5 + 2;
      this._pcrPid = num12 & 8191U;
      int num13 = (int) buffer[index6] << 8 | (int) buffer[index6 + 1];
      int offset1 = index6 + 2;
      int length1 = num13 & 4095;
      if (offset1 - offset + length1 >= length)
        return;
      TsDescriptor[] tsDescriptorArray1 = (TsDescriptor[]) null;
      if (length1 > 0)
        tsDescriptorArray1 = Enumerable.ToArray<TsDescriptor>(TsDescriptors.Parse(this._descriptorFactory, buffer, offset1, length1));
      this._newProgramDescriptors = tsDescriptorArray1;
      int num14 = offset1 + length1;
      int num15 = offset + length;
      while (num14 + 5 <= num15)
      {
        byte[] numArray4 = buffer;
        int index7 = num14;
        int num16 = 1;
        int index8 = index7 + num16;
        byte streamType1 = numArray4[index7];
        uint num17 = (uint) buffer[index8] << 8 | (uint) buffer[index8 + 1];
        int index9 = index8 + 2;
        uint index10 = num17 & 8191U;
        int num18 = (int) buffer[index9] << 8 | (int) buffer[index9 + 1];
        int offset2 = index9 + 2;
        int length2 = num18 & 4095;
        if (offset2 + length2 > num15)
          return;
        TsDescriptor[] tsDescriptorArray2 = (TsDescriptor[]) null;
        if (length2 > 0)
          tsDescriptorArray2 = Enumerable.ToArray<TsDescriptor>(TsDescriptors.Parse(this._descriptorFactory, buffer, offset2, length2));
        num14 = offset2 + length2;
        TsStreamType streamType2 = TsStreamType.FindStreamType(streamType1);
        TsProgramMapTable.ProgramMap programMap = new TsProgramMapTable.ProgramMap()
        {
          Pid = index10,
          StreamType = streamType2,
          StreamDescriptors = tsDescriptorArray2
        };
        this._newProgramStreams[index10] = programMap;
      }
      if ((int) num9 != (int) num11)
        return;
      this.MapProgramStreams();
    }

    private void AddPcr(TsPacket packet)
    {
      if (packet == null || !packet.Pcr.HasValue)
        return;
      this._pcrIndex = new int?(packet.TsIndex);
      this._pcr = packet.Pcr;
    }

    private void ClearProgram(TsProgramMapTable.ProgramMap program)
    {
      this._decoder.UnregisterHandler(program.Pid);
      TsPacketizedElementaryStream elementaryStream = program.Stream;
      if (null != elementaryStream)
        elementaryStream.Clear();
      Debug.Assert(this._programStreamMap.Remove(program.Pid));
    }

    public void Clear()
    {
      foreach (TsProgramMapTable.ProgramMap program in Enumerable.ToArray<TsProgramMapTable.ProgramMap>((IEnumerable<TsProgramMapTable.ProgramMap>) this._programStreamMap.Values))
        this.ClearProgram(program);
      Debug.Assert(0 == this._programStreamMap.Count);
      this._newProgramStreams.Clear();
      this._newProgramDescriptors = (TsDescriptor[]) null;
      this._retiredProgramStreams.Clear();
    }

    public void FlushBuffers()
    {
      foreach (TsProgramMapTable.ProgramMap programMap in this._programStreamMap.Values)
        programMap.Stream.FlushBuffers();
      this._newProgramStreams.Clear();
    }

    private void MapProgramStreams()
    {
      this._streamList.Clear();
      foreach (TsProgramMapTable.ProgramMap programMap1 in this._programStreamMap.Values)
      {
        TsProgramMapTable.ProgramMap programMap2;
        if (this._newProgramStreams.TryGetValue(programMap1.Pid, out programMap2))
        {
          if (!object.Equals((object) programMap2.StreamType, (object) programMap1.StreamType))
            this._streamList.Add(programMap1);
        }
        else
          this._streamList.Add(programMap1);
      }
      if (this._streamList.Count > 0)
      {
        foreach (TsProgramMapTable.ProgramMap program in this._streamList)
        {
          Debug.WriteLine("*** TsProgramMapTable.MapProgramStreams(): retiring " + (object) program);
          this.ClearProgram(program);
          this._retiredProgramStreams[Tuple.Create<uint, TsStreamType>(program.Pid, program.StreamType)] = program;
        }
        this._streamList.Clear();
      }
      string programLanguage = Iso639_2Normalization.Normalize(TsDescriptors.GetDefaultLanguage((IEnumerable<TsDescriptor>) this._newProgramDescriptors));
      ProgramStreams programStreams = new ProgramStreams()
      {
        ProgramNumber = this._programNumber,
        Language = programLanguage,
        Streams = (ICollection<IProgramStream>) Enumerable.ToArray<ProgramStreams.ProgramStream>(Enumerable.Select<TsProgramMapTable.ProgramMap, ProgramStreams.ProgramStream>((IEnumerable<TsProgramMapTable.ProgramMap>) this._newProgramStreams.Values, (Func<TsProgramMapTable.ProgramMap, ProgramStreams.ProgramStream>) (s => new ProgramStreams.ProgramStream()
        {
          Pid = s.Pid,
          StreamType = s.StreamType,
          Language = Iso639_2Normalization.Normalize(TsDescriptors.GetDefaultLanguage((IEnumerable<TsDescriptor>) s.StreamDescriptors)) ?? programLanguage
        })))
      };
      if (null != this._streamFilter)
        this._streamFilter((IProgramStreams) programStreams);
      foreach (var fAnonymousType1 in Enumerable.Join((IEnumerable<IProgramStream>) programStreams.Streams, (IEnumerable<TsProgramMapTable.ProgramMap>) this._newProgramStreams.Values, (Func<IProgramStream, uint>) (ps => ps.Pid), (Func<TsProgramMapTable.ProgramMap, uint>) (pm => pm.Pid), (ps, pm) =>
      {
        var fAnonymousType1 = new
        {
          BlockStream = ps.BlockStream,
          ProgramStream = pm,
          Language = ps.Language
        };
        return fAnonymousType1;
      }))
      {
        bool flag = !fAnonymousType1.BlockStream;
        uint index = fAnonymousType1.ProgramStream.Pid;
        TsStreamType streamType = fAnonymousType1.ProgramStream.StreamType;
        string language = fAnonymousType1.Language;
        TsProgramMapTable.ProgramMap program;
        if (this._programStreamMap.TryGetValue(index, out program))
        {
          if (!object.Equals((object) program.StreamType, (object) streamType) || !flag)
            this.ClearProgram(program);
          else
            continue;
        }
        if (flag)
        {
          Tuple<uint, TsStreamType> key = Tuple.Create<uint, TsStreamType>(index, streamType);
          TsProgramMapTable.ProgramMap programMap;
          TsPacketizedElementaryStream pes;
          if (this._retiredProgramStreams.TryGetValue(key, out programMap))
          {
            Debug.WriteLine("*** TsProgramMapTable.MapProgramStreams(): remapping retired program " + (object) programMap);
            Debug.Assert(this._retiredProgramStreams.Remove(key), "Unable to remove program from retired");
            pes = programMap.Stream;
            this._programStreamMap[index] = programMap;
          }
          else
          {
            IMediaStreamMetadata mediaStreamMetadata = (IMediaStreamMetadata) null;
            if (null != language)
              mediaStreamMetadata = (IMediaStreamMetadata) new MediaStreamMetadata()
              {
                Language = language
              };
            pes = this._decoder.CreateStream(streamType, index, mediaStreamMetadata);
            fAnonymousType1.ProgramStream.Stream = pes;
            this._programStreamMap[index] = fAnonymousType1.ProgramStream;
          }
          if ((int) index == (int) this._pcrPid)
          {
            this._foundPcrPid = true;
            this._decoder.RegisterHandler(index, (Action<TsPacket>) (p =>
            {
              this.AddPcr(p);
              pes.Add(p);
            }));
          }
          else
            this._decoder.RegisterHandler(index, new Action<TsPacket>(pes.Add));
        }
        else if ((int) index == (int) this._pcrPid)
        {
          this._foundPcrPid = true;
          this._decoder.RegisterHandler(index, new Action<TsPacket>(this.AddPcr));
        }
      }
      this._newProgramStreams.Clear();
      if (this._foundPcrPid)
        return;
      this._foundPcrPid = true;
      this._decoder.RegisterHandler(this._pcrPid, new Action<TsPacket>(this.AddPcr));
    }

    private class ProgramMap
    {
      public uint Pid;
      public TsPacketizedElementaryStream Stream;
      public TsDescriptor[] StreamDescriptors;
      public TsStreamType StreamType;

      public override string ToString()
      {
        return string.Format("{0}/{1}", new object[2]
        {
          (object) this.Pid,
          this.StreamType == null ? (object) "<unknown type>" : (object) this.StreamType.Description
        });
      }
    }
  }
}
