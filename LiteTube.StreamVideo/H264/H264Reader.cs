using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.H264
{
  internal sealed class H264Reader : IH264Reader
  {
    private static readonly byte[] NumClockTsLookup = new byte[9]
    {
      (byte) 1,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 2,
      (byte) 3
    };
    private static readonly uint[] ProfileIdcHasChromaFormat = Enumerable.ToArray<uint>((IEnumerable<uint>) Enumerable.OrderBy<uint, uint>((IEnumerable<uint>) new uint[9]
    {
      100U,
      110U,
      122U,
      244U,
      44U,
      83U,
      86U,
      118U,
      128U
    }, (Func<uint, uint>) (k => k)));
    private static readonly byte[] DivisorFromPicStruct = new byte[9]
    {
      (byte) 2,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 4,
      (byte) 6
    };
    private uint? _bottomFieldPicOrderInFramePresentFlag;
    private uint? _chromaFormatIdc;
    private uint? _cpbRemovalDelayLengthMinus1;
    private uint? _deltaPicOrderAlwaysZeroFlag;
    private uint? _dpbOutputDelayLengthMinus1;
    private bool? _fieldPicFlag;
    private bool? _fixedFrameRateFlag;
    private uint? _frameMbsOnlyFlag;
    private uint? _log2MaxFrameNumMinus4;
    private bool? _nalHrdParametersPresentFlag;
    private uint? _numUnitsInTick;
    private uint? _picOrderCntType;
    private uint? _picParameterSetId;
    private uint? _picStruct;
    private bool? _picStructPresentFlag;
    private uint? _ppsSeqParameterSetId;
    private uint? _redundantPicCntPresentFlag;
    private uint? _separateColourPlaneFlag;
    private uint? _seqParameterSetId;
    private uint? _slicePicParameterSetId;
    private uint? _timeOffsetLength;
    private uint? _timeScale;
    private ICollection<byte> _timingBytes;
    private bool? _vclHrdParametersPresentFlag;

    public int? FrameRateDenominator { get; private set; }

    public int? FrameRateNumerator { get; private set; }

    public string Name { get; private set; }

    public int? Width { get; private set; }

    public int? Height { get; private set; }

    public void ReadSliceHeader(H264Bitstream r, bool IdrPicFlag)
    {
      H264BitstreamExtensions.ReadUe(r);
      H264BitstreamExtensions.ReadUe(r);
      this._slicePicParameterSetId = new uint?(H264BitstreamExtensions.ReadUe(r));
      uint? nullable = this._separateColourPlaneFlag;
      if (((int) nullable.GetValueOrDefault() != 1 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
        r.ReadBits(2);
      H264Bitstream h264Bitstream1 = r;
      nullable = this._log2MaxFrameNumMinus4;
      int count1 = (nullable.HasValue ? (int) nullable.GetValueOrDefault() : 0) + 4;
      h264Bitstream1.ReadBits(count1);
      uint num1 = 0;
      nullable = this._frameMbsOnlyFlag;
      if (((int) nullable.GetValueOrDefault() != 0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
      {
        num1 = r.ReadBits(1);
        if (0 != (int) num1)
          r.ReadBits(1);
      }
      this._fieldPicFlag = new bool?(0 != (int) num1);
      if (!IdrPicFlag)
        return;
      H264BitstreamExtensions.ReadUe(r);
      nullable = this._picOrderCntType;
      if (((int) nullable.GetValueOrDefault() != 0 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
      {
        H264Bitstream h264Bitstream2 = r;
        nullable = this._log2MaxFrameNumMinus4;
        int count2 = (nullable.HasValue ? (int) nullable.GetValueOrDefault() : 0) + 4;
        h264Bitstream2.ReadBits(count2);
        nullable = this._bottomFieldPicOrderInFramePresentFlag;
        if (((int) nullable.GetValueOrDefault() != 0 ? 1 : (!nullable.HasValue ? 1 : 0)) != 0 && 0 == (int) num1)
          H264BitstreamExtensions.ReadSe(r);
      }
      nullable = this._picOrderCntType;
      int num2;
      if (((int) nullable.GetValueOrDefault() != 1 ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
      {
        nullable = this._deltaPicOrderAlwaysZeroFlag;
        num2 = ((int) nullable.GetValueOrDefault() != 0 ? 0 : (nullable.HasValue ? 1 : 0)) == 0 ? 1 : 0;
      }
      else
        num2 = 1;
      if (num2 == 0)
      {
        H264BitstreamExtensions.ReadSe(r);
        nullable = this._bottomFieldPicOrderInFramePresentFlag;
        if (((int) nullable.GetValueOrDefault() != 0 ? 1 : (!nullable.HasValue ? 1 : 0)) != 0 && 0 == (int) num1)
          H264BitstreamExtensions.ReadSe(r);
      }
      nullable = this._redundantPicCntPresentFlag;
      if (((int) nullable.GetValueOrDefault() != 0 ? 1 : (!nullable.HasValue ? 1 : 0)) != 0)
        H264BitstreamExtensions.ReadUe(r);
    }

    public void ReadSei(H264Bitstream r, ICollection<byte> buffer)
    {
      uint num = H264BitstreamExtensions.ReadFfSum(r);
      uint length = H264BitstreamExtensions.ReadFfSum(r);
      if ((int) num != 1)
        return;
      if (this._slicePicParameterSetId.HasValue)
      {
        this.ReadPicTiming(r, length);
      }
      else
      {
        if (EnumerableExtensions.SequencesAreEquivalent<byte>((IEnumerable<byte>) buffer, (IEnumerable<byte>) this._timingBytes))
          return;
        this._timingBytes = (ICollection<byte>) Enumerable.ToArray<byte>((IEnumerable<byte>) buffer);
      }
    }

    public bool ReaderCheckConfigure(H264Configurator h264Configurator)
    {
      if (!this._slicePicParameterSetId.HasValue || !this._ppsSeqParameterSetId.HasValue || !this._seqParameterSetId.HasValue)
        return false;
      uint? nullable1 = this._slicePicParameterSetId;
      uint? nullable2 = this._picParameterSetId;
      int num;
      if (((int) nullable1.GetValueOrDefault() != (int) nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue == nullable2.HasValue ? 1 : 0)) != 0)
      {
        nullable1 = this._ppsSeqParameterSetId;
        nullable2 = this._seqParameterSetId;
        num = ((int) nullable1.GetValueOrDefault() != (int) nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue == nullable2.HasValue ? 1 : 0)) == 0 ? 1 : 0;
      }
      else
        num = 1;
      if (num != 0)
        return false;
      if (!this.ComputeFrameRate())
        Debug.WriteLine("H264Reader.ReaderCheckConfigure() unable to get frame rate");
      return true;
    }

    public void ReadSps(H264Bitstream r)
    {
      this._ppsSeqParameterSetId = new uint?();
      r.ReadBits(1);
      r.ReadBits(2);
      r.ReadBits(5);
      uint profile_idc = r.ReadBits(8);
      uint constraint_sets = r.ReadBits(8);
      uint num1 = r.ReadBits(8);
      this._seqParameterSetId = new uint?(H264BitstreamExtensions.ReadUe(r));
      if (Array.BinarySearch<uint>(H264Reader.ProfileIdcHasChromaFormat, profile_idc) >= 0)
      {
        uint num2 = H264BitstreamExtensions.ReadUe(r);
        this._chromaFormatIdc = new uint?(num2);
        if (3 == (int) num2)
          this._separateColourPlaneFlag = new uint?(r.ReadBits(1));
        H264BitstreamExtensions.ReadUe(r);
        H264BitstreamExtensions.ReadUe(r);
        r.ReadBits(1);
        if (0 != (int) r.ReadBits(1))
        {
          for (int index = 0; index < (3 != (int) num2 ? 8 : 12); ++index)
          {
            if (0 != (int) r.ReadBits(1))
              this.ParseScalingList(r, index < 6 ? 16 : 64);
          }
        }
      }
      this._log2MaxFrameNumMinus4 = new uint?(H264BitstreamExtensions.ReadUe(r));
      uint num3 = H264BitstreamExtensions.ReadUe(r);
      this._picOrderCntType = new uint?(num3);
      if (0 == (int) num3)
        H264BitstreamExtensions.ReadUe(r);
      else if (1 == (int) num3)
      {
        this._deltaPicOrderAlwaysZeroFlag = new uint?(r.ReadBits(1));
        H264BitstreamExtensions.ReadSe(r);
        H264BitstreamExtensions.ReadSe(r);
        uint num2 = H264BitstreamExtensions.ReadUe(r);
        for (int index = 0; (long) index < (long) num2; ++index)
          H264BitstreamExtensions.ReadSe(r);
      }
      H264BitstreamExtensions.ReadUe(r);
      r.ReadBits(1);
      uint num4 = H264BitstreamExtensions.ReadUe(r);
      uint num5 = H264BitstreamExtensions.ReadUe(r);
      uint num6 = r.ReadBits(1);
      this._frameMbsOnlyFlag = new uint?(num6);
      if (0 == (int) num6)
        r.ReadBits(1);
      r.ReadBits(1);
      uint num7 = r.ReadBits(1);
      uint num8 = (uint) (((int) num4 + 1) * 16);
      uint num9 = (uint) ((2 - (int) num6) * ((int) num5 + 1) * 16);
      if (0 != (int) num7)
      {
        uint num2 = H264BitstreamExtensions.ReadUe(r);
        uint num10 = H264BitstreamExtensions.ReadUe(r);
        uint num11 = H264BitstreamExtensions.ReadUe(r);
        uint num12 = H264BitstreamExtensions.ReadUe(r);
        num8 = (uint) ((int) num8 - (int) num2 * 2 - (int) num10 * 2);
        num9 = (uint) ((int) num9 - (int) num11 * 2 - (int) num12 * 2);
      }
      if (0 != (int) r.ReadBits(1))
        this.ReadVuiParameters(r);
      this.Height = new int?((int) num9);
      this.Width = new int?((int) num8);
      this.Name = string.Format("H.264 \"{0}\" profile, level {1} {2}x{3}", (object) H264Reader.ProfileName(profile_idc, constraint_sets), (object) ((double) num1 / 10.0), (object) num8, (object) num9);
    }

    public void ReadPps(H264Bitstream r)
    {
      this._slicePicParameterSetId = new uint?();
      r.ReadBits(1);
      r.ReadBits(2);
      r.ReadBits(5);
      this._picParameterSetId = new uint?(H264BitstreamExtensions.ReadUe(r));
      this._ppsSeqParameterSetId = new uint?(H264BitstreamExtensions.ReadUe(r));
      r.ReadBits(1);
      this._bottomFieldPicOrderInFramePresentFlag = new uint?(r.ReadBits(1));
      uint num1 = H264BitstreamExtensions.ReadUe(r);
      if (num1 > 0U)
      {
        uint num2 = H264BitstreamExtensions.ReadUe(r);
        if (0 == (int) num2)
        {
          for (int index = 0; (long) index <= (long) num1; ++index)
            H264BitstreamExtensions.ReadUe(r);
        }
        else if (2 == (int) num2)
        {
          for (int index = 0; (long) index < (long) num1; ++index)
          {
            H264BitstreamExtensions.ReadUe(r);
            H264BitstreamExtensions.ReadUe(r);
          }
        }
        else if (3 == (int) num2 || 4 == (int) num2 || 5 == (int) num2)
        {
          r.ReadBits(1);
          H264BitstreamExtensions.ReadUe(r);
        }
        else if (6 == (int) num2)
        {
          int bitSize = H264Reader.GetBitSize(num1);
          uint num3 = H264BitstreamExtensions.ReadUe(r);
          for (int index = 0; (long) index <= (long) num3; ++index)
            r.ReadBits(bitSize);
        }
      }
      H264BitstreamExtensions.ReadUe(r);
      H264BitstreamExtensions.ReadUe(r);
      r.ReadBits(1);
      r.ReadBits(2);
      H264BitstreamExtensions.ReadSe(r);
      H264BitstreamExtensions.ReadSe(r);
      H264BitstreamExtensions.ReadSe(r);
      r.ReadBits(1);
      r.ReadBits(1);
      this._redundantPicCntPresentFlag = new uint?(r.ReadBits(1));
      if (!this.more_rbsp_data(r))
        return;
      uint num4 = r.ReadBits(1);
      if (0 != (int) r.ReadBits(1))
      {
        int num2 = 0;
        while (true)
        {
          long num3 = (long) num2;
          long num5 = 6;
          uint? nullable = this._chromaFormatIdc;
          long num6 = (((int) nullable.GetValueOrDefault() != 3 ? 1 : (!nullable.HasValue ? 1 : 0)) != 0 ? 2L : 6L) * (long) num4;
          long num7 = num5 + num6;
          if (num3 < num7)
          {
            if (0 != (int) r.ReadBits(1))
            {
              if (num2 < 6)
                this.ReadScalingList(r, 16);
              else
                this.ReadScalingList(r, 64);
            }
            ++num2;
          }
          else
            break;
        }
      }
      H264BitstreamExtensions.ReadSe(r);
    }

    public void TryReparseTimingSei(H264Configurator h264Configurator)
    {
      if (this._timingBytes == null || !this._seqParameterSetId.HasValue)
        return;
      h264Configurator.ParseSei(this._timingBytes);
    }

    private void ReadVuiParameters(H264Bitstream r)
    {
      if (0 != (int) r.ReadBits(1) && (int) byte.MaxValue == (int) r.ReadBits(8))
      {
        r.ReadBits(16);
        r.ReadBits(16);
      }
      if (0 != (int) r.ReadBits(1))
        r.ReadBits(1);
      if (0 != (int) r.ReadBits(1))
      {
        r.ReadBits(3);
        r.ReadBits(1);
        if (0 != (int) r.ReadBits(1))
        {
          r.ReadBits(8);
          r.ReadBits(8);
          r.ReadBits(8);
        }
      }
      if (0 != (int) r.ReadBits(1))
      {
        H264BitstreamExtensions.ReadUe(r);
        H264BitstreamExtensions.ReadUe(r);
      }
      if (0 != (int) r.ReadBits(1))
      {
        this._numUnitsInTick = new uint?(r.ReadBits(32));
        this._timeScale = new uint?(r.ReadBits(32));
        this._fixedFrameRateFlag = new bool?(0 != (int) r.ReadBits(1));
      }
      uint num1 = r.ReadBits(1);
      this._nalHrdParametersPresentFlag = new bool?(0 != (int) num1);
      if (0 != (int) num1)
        this.ReadHrdParameters(r);
      uint num2 = r.ReadBits(1);
      this._vclHrdParametersPresentFlag = new bool?(0 != (int) num2);
      if (0 != (int) num2)
        this.ReadHrdParameters(r);
      if ((int) num1 != 0 || 0 != (int) num2)
        r.ReadBits(1);
      this._picStructPresentFlag = new bool?(0 != (int) r.ReadBits(1));
      if (0 == (int) r.ReadBits(1))
        return;
      r.ReadBits(1);
      H264BitstreamExtensions.ReadUe(r);
      H264BitstreamExtensions.ReadUe(r);
      H264BitstreamExtensions.ReadUe(r);
      H264BitstreamExtensions.ReadUe(r);
      H264BitstreamExtensions.ReadUe(r);
      H264BitstreamExtensions.ReadUe(r);
    }

    private void ReadHrdParameters(H264Bitstream r)
    {
      uint num = H264BitstreamExtensions.ReadUe(r);
      r.ReadBits(4);
      r.ReadBits(4);
      for (int index = 0; (long) index <= (long) num; ++index)
      {
        H264BitstreamExtensions.ReadUe(r);
        H264BitstreamExtensions.ReadUe(r);
        r.ReadBits(1);
      }
      r.ReadBits(5);
      this._cpbRemovalDelayLengthMinus1 = new uint?(r.ReadBits(5));
      this._dpbOutputDelayLengthMinus1 = new uint?(r.ReadBits(5));
      this._timeOffsetLength = new uint?(r.ReadBits(5));
    }

    private void ReadScalingList(H264Bitstream r, int sizeOfScalingList)
    {
      int num1 = 8;
      int num2 = 8;
      for (int index = 0; index < sizeOfScalingList; ++index)
      {
        if (num2 != 0)
        {
          int num3 = H264BitstreamExtensions.ReadSe(r);
          num2 = (num1 + num3 + 256) % 256;
          bool flag = index == 0 && num2 == 0;
        }
        num1 = num2 == 0 ? num1 : num2;
      }
    }

    private void ReadUserDataUnregistered(H264Bitstream r, uint length)
    {
      byte[] b = new byte[16];
      for (int index = 0; index < b.Length; ++index)
        b[index] = (byte) r.ReadBits(8);
      Guid guid = new Guid(b);
    }

    private int? GetNumClockTs()
    {
      if (!this._picStruct.HasValue)
        return new int?();
      uint num = this._picStruct.Value;
      if ((long) num >= (long) H264Reader.NumClockTsLookup.Length)
        return new int?();
      return new int?((int) H264Reader.NumClockTsLookup[num]);
    }

    private void ReadPicTiming(H264Bitstream r, uint length)
    {
      if (this._nalHrdParametersPresentFlag.HasValue && this._nalHrdParametersPresentFlag.Value || this._vclHrdParametersPresentFlag.HasValue && this._vclHrdParametersPresentFlag.Value)
      {
        if (!this._cpbRemovalDelayLengthMinus1.HasValue)
          return;
        r.ReadBits((int) this._cpbRemovalDelayLengthMinus1.Value + 1);
        if (!this._dpbOutputDelayLengthMinus1.HasValue)
          return;
        r.ReadBits((int) this._dpbOutputDelayLengthMinus1.Value + 1);
      }
      if (!this._picStructPresentFlag.HasValue || !this._picStructPresentFlag.Value)
        return;
      this._picStruct = new uint?(r.ReadBits(4));
      int? numClockTs = this.GetNumClockTs();
      if (!numClockTs.HasValue)
        return;
      int num1 = numClockTs.Value;
      for (int index = 0; index < num1; ++index)
      {
        if (0 != (int) r.ReadBits(1))
        {
          r.ReadBits(2);
          r.ReadBits(1);
          r.ReadBits(5);
          uint num2 = r.ReadBits(1);
          r.ReadBits(1);
          r.ReadBits(1);
          r.ReadBits(8);
          uint num3;
          uint num4;
          uint num5;
          if (0 != (int) num2)
          {
            num3 = r.ReadBits(6);
            num4 = r.ReadBits(6);
            num5 = r.ReadBits(5);
          }
          else if (0 != (int) r.ReadBits(1))
          {
            num3 = r.ReadBits(6);
            if (0 != (int) r.ReadBits(1))
            {
              num4 = r.ReadBits(6);
              if (0 != (int) r.ReadBits(1))
                num5 = r.ReadBits(5);
            }
          }
          uint? nullable = this._timeOffsetLength;
          uint num6 = nullable.HasValue ? nullable.GetValueOrDefault() : 24U;
          if (num6 > 0U)
            H264BitstreamExtensions.ReadSignedBits(r, (int) num6);
        }
      }
    }

    private bool ComputeFrameRate()
    {
      if (!this._numUnitsInTick.HasValue || !this._timeScale.HasValue)
        return true;
      uint num1 = this._numUnitsInTick.Value;
      uint num2 = this._timeScale.Value;
      if (!this._picStructPresentFlag.HasValue)
        return true;
      uint num3;
      if (this._picStructPresentFlag.Value)
      {
        if (!this._picStruct.HasValue)
          return false;
        if ((long) this._picStruct.Value >= (long) H264Reader.DivisorFromPicStruct.Length)
          return true;
        num3 = (uint) H264Reader.DivisorFromPicStruct[_picStruct.Value];
      }
      else
      {
        if (!this._fieldPicFlag.HasValue)
          return true;
        num3 = this._fieldPicFlag.Value ? 1U : 2U;
      }
      if (1 != (int) num3)
      {
        if (0 == (int) (num2 % num3))
          num2 /= num3;
        else
          num1 *= num3;
      }
      this.FrameRateDenominator = new int?((int) num1);
      this.FrameRateNumerator = new int?((int) num2);
      return true;
    }

    private void ParseScalingList(H264Bitstream r, int sizeOfScalingList)
    {
      int num1 = 8;
      int num2 = 8;
      for (int index = 0; index < sizeOfScalingList; ++index)
      {
        if (0 != num2)
        {
          int num3 = H264BitstreamExtensions.ReadSe(r);
          num2 = num1 + num3 + 256 & (int) byte.MaxValue;
          bool flag = index == 0 && 0 == num2;
        }
        num1 = num2 == 0 ? num1 : num2;
      }
    }

    private static string ProfileName(uint profile_idc, uint constraint_sets)
    {
      bool flag1 = 0 != ((int) constraint_sets & 2);
      bool flag2 = 0 != ((int) constraint_sets & 8);
      bool flag3 = 0 != ((int) constraint_sets & 16);
      bool flag4 = 0 != ((int) constraint_sets & 32);
      uint num = profile_idc;
      if (num <= 88U)
      {
        if (num <= 66U)
        {
          if ((int) num == 44)
            return "CAVLC 4:4:4 Intra";
          if ((int) num == 66)
            return flag1 ? "Constrained Baseline" : "Baseline";
        }
        else
        {
          if ((int) num == 77)
            return "Main";
          if ((int) num == 88)
            return "Extended";
        }
      }
      else if (num <= 110U)
      {
        if ((int) num != 100)
        {
          if ((int) num == 110)
            return flag2 ? "High 10 Intra" : "High 10";
        }
        else
        {
          if (flag3 && flag4)
            return "Constrained High";
          return flag3 ? "Progressive High" : "High";
        }
      }
      else
      {
        if ((int) num == 122)
          return flag2 ? "High 4:2:2 Intra" : "High 4:2:2";
        if ((int) num == 244)
          return flag2 ? "High 4:4:4 Intra" : "High 4:4:4 Predictive";
      }
      if (flag1)
        return "Constrained Baseline";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("P{0}-CS[", (object) profile_idc);
      for (int index = 0; index < 8; ++index)
      {
        if (0L != ((long) constraint_sets & (long) (1 << index)))
          stringBuilder.Append(index);
        else
          stringBuilder.Append('.');
      }
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }

    private static int GetBitSize(uint value)
    {
      if (1U < value)
        return 0;
      int num = 1;
      for (uint index = 1; index < value; index = (uint) ((int) index << 1 | 1))
        ++num;
      return num;
    }

    private bool more_rbsp_data(H264Bitstream r)
    {
      return r.HasData;
    }
  }
}
