using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  public class TsProgramAssociationTable : TsProgramSpecificInformation
  {
    private readonly List<TsProgramAssociationTable.ProgramAssociation> _newPrograms = new List<TsProgramAssociationTable.ProgramAssociation>();
    private readonly List<TsProgramAssociationTable.ProgramAssociation> _oldPrograms = new List<TsProgramAssociationTable.ProgramAssociation>();
    private readonly List<TsProgramAssociationTable.ProgramAssociation> _programs = new List<TsProgramAssociationTable.ProgramAssociation>();
    private const int MinimumProgramAssociationSize = 5;
    private readonly ITsDecoder _decoder;
    private readonly Func<int, bool> _programFilter;
    private readonly ITsProgramMapTableFactory _programMapTableFactory;
    private readonly Action<IProgramStreams> _streamFilter;
    private bool _currentNextIndicator;
    private bool _hasData;
    private byte _lastSectionNumber;
    private byte _sectionNumber;
    private int _transportStreamId;
    private uint _versionNumber;

    public TsProgramAssociationTable(ITsDecoder decoder, ITsProgramMapTableFactory programMapTableFactory, Func<int, bool> programFilter, Action<IProgramStreams> streamFilter)
      : base(TsProgramSpecificInformation.TsTableId.program_association_section)
    {
      if (null == decoder)
        throw new ArgumentNullException("decoder");
      if (null == programMapTableFactory)
        throw new ArgumentNullException("programMapTableFactory");
      this._decoder = decoder;
      this._programMapTableFactory = programMapTableFactory;
      this._programFilter = programFilter;
      this._streamFilter = streamFilter;
    }

    protected override void ParseSection(TsPacket packet, int offset, int length)
    {
      if (length < 5)
        return;
      int index1 = offset;
      byte[] buffer = packet.Buffer;
      int num1 = index1 + length;
      this._transportStreamId = (int) buffer[index1] << 8 | (int) buffer[index1 + 1];
      int num2 = index1 + 2;
      byte[] numArray1 = buffer;
      int index2 = num2;
      int num3 = 1;
      int num4 = index2 + num3;
      byte num5 = numArray1[index2];
      this._currentNextIndicator = 0 != ((int) num5 & 1);
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
      if ((int) this._lastSectionNumber > 0 && (int) num11 != (int) this._lastSectionNumber || (int) num11 < (int) num9)
        return;
      if (this._hasData)
      {
        if ((int) this._versionNumber != (int) num6)
          return;
        ++this._sectionNumber;
        if ((int) this._sectionNumber != (int) num9 || (int) this._lastSectionNumber != (int) num11)
          return;
      }
      else
      {
        if (0 != (int) num9)
          return;
        this._sectionNumber = (byte) 0;
        this._lastSectionNumber = num11;
        this._versionNumber = (uint) num6;
        this._hasData = true;
      }
      while (index5 + 4 <= num1)
      {
        int program_number = (int) buffer[index5] << 8 | (int) buffer[index5 + 1];
        int index6 = index5 + 2;
        uint pid = (uint) buffer[index6] << 8 | (uint) buffer[index6 + 1];
        pid &= 8191U;
        index5 = index6 + 2;
        if (!Enumerable.Any<TsProgramAssociationTable.ProgramAssociation>((IEnumerable<TsProgramAssociationTable.ProgramAssociation>) this._newPrograms, (Func<TsProgramAssociationTable.ProgramAssociation, bool>) (p => (int) p.Pid == (int) pid && p.ProgramNumber == program_number)) && this._programFilter(program_number))
          this._newPrograms.Add(new TsProgramAssociationTable.ProgramAssociation()
          {
            ProgramNumber = program_number,
            Pid = pid
          });
      }
      if ((int) this._sectionNumber != (int) this._lastSectionNumber || !this._currentNextIndicator)
        return;
      this.Activate();
    }

    private void Activate()
    {
      foreach (TsProgramAssociationTable.ProgramAssociation programAssociation in this._programs)
      {
        if (!this._newPrograms.Contains(programAssociation))
        {
          this._decoder.UnregisterHandler(programAssociation.Pid);
          this._oldPrograms.Add(programAssociation);
        }
      }
      foreach (TsProgramAssociationTable.ProgramAssociation program in this._oldPrograms)
        this.CloseProgram(program);
      this._oldPrograms.Clear();
      foreach (TsProgramAssociationTable.ProgramAssociation programAssociation in this._newPrograms)
      {
        if (0 != programAssociation.ProgramNumber && !this._programs.Contains(programAssociation))
        {
          TsProgramMapTable tsProgramMapTable = this._programMapTableFactory.Create(this._decoder, programAssociation.ProgramNumber, programAssociation.Pid, this._streamFilter);
          programAssociation.MapTable = tsProgramMapTable;
          this._decoder.RegisterHandler(programAssociation.Pid, new Action<TsPacket>(((TsProgramSpecificInformation) tsProgramMapTable).Add));
          this._programs.Add(programAssociation);
        }
      }
      this._newPrograms.Clear();
    }

    private void CloseProgram(TsProgramAssociationTable.ProgramAssociation program)
    {
      Debug.Assert(this._programs.Remove(program));
      TsProgramMapTable tsProgramMapTable = program.MapTable;
      if (null == tsProgramMapTable)
        return;
      tsProgramMapTable.Clear();
    }

    public void Clear()
    {
      foreach (TsProgramAssociationTable.ProgramAssociation program in this._programs.ToArray())
        this.CloseProgram(program);
      Debug.Assert(0 == this._programs.Count);
      this._newPrograms.Clear();
      this._oldPrograms.Clear();
    }

    public void FlushBuffers()
    {
      foreach (TsProgramAssociationTable.ProgramAssociation programAssociation in this._programs)
        programAssociation.FlushBuffers();
      this._newPrograms.Clear();
    }

    private class ProgramAssociation : IEquatable<TsProgramAssociationTable.ProgramAssociation>
    {
      public TsProgramMapTable MapTable;
      public uint Pid;
      public int ProgramNumber;

      public bool Equals(TsProgramAssociationTable.ProgramAssociation other)
      {
        if (object.ReferenceEquals((object) this, (object) other))
          return true;
        return (int) this.Pid == (int) other.Pid && this.ProgramNumber == other.ProgramNumber;
      }

      public override bool Equals(object obj)
      {
        return this.Equals(obj as TsProgramAssociationTable.ProgramAssociation);
      }

      public override int GetHashCode()
      {
        return 5 * this.Pid.GetHashCode() + 65537 * this.ProgramNumber.GetHashCode();
      }

      public void FlushBuffers()
      {
        this.MapTable.FlushBuffers();
      }
    }
  }
}
