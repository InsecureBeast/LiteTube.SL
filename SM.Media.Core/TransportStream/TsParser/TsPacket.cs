using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SM.Media.Core.TransportStream.TsParser
{
  public class TsPacket
  {
    private static readonly Dictionary<uint, string> PacketTypes = new Dictionary<uint, string>()
    {
      {
        0U,
        "Program Association Table"
      },
      {
        1U,
        "Conditional Access Table"
      },
      {
        2U,
        "Transport Stream Description Table"
      },
      {
        3U,
        "IPMP Control Information Table"
      },
      {
        8191U,
        "Null packet"
      }
    };
    private static readonly string[] AdaptationValues = new string[4]
    {
      "Reserved",
      "Payload Only",
      "Adaptation Only",
      "Adaptation and Payload"
    };
    private static readonly string[] ScramblingControlValues = new string[4]
    {
      "Not scrambled",
      "User defined 1",
      "User defined 2",
      "User defined 3"
    };
    private const double Pcr27MHzTicksScale = 0.37037037037037;
    public const int PacketSize = 188;
    public const int SyncByte = 71;
    private int _adaptationFieldControl;
    private byte _adaptationFlags;
    private int _adaptationLength;
    private byte[] _buffer;
    private int _payloadIndex;
    private int _payloadLength;
    private bool _transportErrorIndicator;
    private bool _transportPriority;
    private int _transportScramblingControl;

    public uint Pid { get; private set; }

    public int PayloadLength
    {
      get
      {
        return this._payloadLength;
      }
    }

    public bool IsStart { get; private set; }

    public bool IsSkip { get; private set; }

    public int ContinuityCount { get; private set; }

    public bool IsDiscontinuos { get; private set; }

    public ulong? Pcr { get; private set; }

    public int TsIndex { get; private set; }

    internal byte[] Buffer
    {
      get
      {
        return this._buffer;
      }
    }

    internal int BufferOffset
    {
      get
      {
        return this._payloadIndex;
      }
    }

    internal int BufferLength
    {
      get
      {
        return this._payloadLength;
      }
    }

    public void CopyTo(byte[] buffer, int index)
    {
      Array.Copy((Array) this._buffer, this._payloadIndex, (Array) buffer, index, this._payloadLength);
    }

    private ulong ReadTime(byte[] buffer, int index)
    {
      ulong num1 = (ulong) ((uint) ((int) buffer[index] << 24 | (int) buffer[index + 1] << 16 | (int) buffer[index + 2] << 8) | (uint) buffer[index + 3]) << 1;
      uint num2 = (uint) buffer[index + 4] << 8 | (uint) buffer[index + 5];
      if (0 != ((int) num2 & 32768))
        num1 |= 1UL;
      return num1 * 300UL + (ulong) (num2 & 511U);
    }

    public bool Parse(int tsIndex, byte[] buffer, int index)
    {
      this.TsIndex = tsIndex;
      this._buffer = buffer;
      int num1 = index;
      this.IsSkip = false;
      int num2 = 71;
      byte[] numArray1 = buffer;
      int index1 = num1;
      int num3 = 1;
      int num4 = index1 + num3;
      int num5 = (int) numArray1[index1];
      if (num2 != num5)
      {
        this.IsSkip = true;
        return false;
      }
      byte[] numArray2 = buffer;
      int index2 = num4;
      int num6 = 1;
      int num7 = index2 + num6;
      this.Pid = (uint) numArray2[index2] << 8;
      TsPacket tsPacket = this;
      int num8 = (int) tsPacket.Pid;
      byte[] numArray3 = buffer;
      int index3 = num7;
      int num9 = 1;
      int num10 = index3 + num9;
      int num11 = (int) numArray3[index3];
      int num12 = num8 | num11;
      tsPacket.Pid = (uint) num12;
      this._transportErrorIndicator = 0 != ((int) this.Pid & 32768);
      this.IsStart = 0 != ((int) this.Pid & 16384);
      this._transportPriority = 0 != ((int) this.Pid & 8192);
      this.Pid &= 8191U;
      if (8191 == (int) this.Pid)
      {
        this.IsSkip = true;
        return true;
      }
      if (this._transportErrorIndicator)
      {
        this.IsSkip = true;
        return true;
      }
      byte[] numArray4 = buffer;
      int index4 = num10;
      int num13 = 1;
      int num14 = index4 + num13;
      byte num15 = numArray4[index4];
      this._transportScramblingControl = (int) num15 >> 6 & 3;
      this._adaptationFieldControl = (int) num15 >> 4 & 3;
      if (0 == this._adaptationFieldControl)
        this.IsSkip = true;
      this.ContinuityCount = (int) num15 & 15;
      this._payloadIndex = num14;
      this._payloadLength = 188 - (num14 - index);
      this.IsDiscontinuos = false;
      this.Pcr = new ulong?();
      if (0 != (this._adaptationFieldControl & 2))
      {
        byte[] numArray5 = buffer;
        int index5 = num14;
        int num16 = 1;
        int num17 = index5 + num16;
        this._adaptationLength = (int) numArray5[index5];
        if (2 == this._adaptationFieldControl)
        {
          if (this._adaptationLength != 183)
          {
            Debug.WriteLine("Invalid adaptation_field_length (without payload): " + (object) this._adaptationLength);
            this._adaptationLength = 183;
          }
        }
        else if (3 == this._adaptationFieldControl && this._adaptationLength > 182 && 183 != this._adaptationLength)
        {
          Debug.WriteLine("Invalid adaptation_field_length (with payload): " + (object) this._adaptationLength);
          this._adaptationLength = 183;
        }
        ++this._payloadIndex;
        --this._payloadLength;
        if (this._adaptationLength > 0)
        {
          int num18 = this._adaptationLength;
          if (this._payloadLength < this._adaptationLength)
            return false;
          this._payloadIndex += this._adaptationLength;
          this._payloadLength -= this._adaptationLength;
          byte[] numArray6 = buffer;
          int index6 = num17;
          int num19 = 1;
          int index7 = index6 + num19;
          this._adaptationFlags = numArray6[index6];
          int num20 = num18 - 1;
          this.IsDiscontinuos = 0 != ((int) this._adaptationFlags & 128);
          if (0 != ((int) this._adaptationFlags & 16))
          {
            if (num20 < 6)
              return false;
            this.Pcr = new ulong?(this.ReadTime(buffer, index7));
            int num21 = index7 + 6;
            int num22 = num20 - 6;
          }
        }
      }
      else
        this._adaptationLength = 0;
      return true;
    }

    public override string ToString()
    {
      string str;
      if (!TsPacket.PacketTypes.TryGetValue(this.Pid, out str))
      {
        if (this.Pid >= 3U && this.Pid <= 15U)
          str = string.Format("Reserved{0:X4}", (object) this.Pid);
        else
          str = string.Format("PID{0:X4}", (object) this.Pid);
      }
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendFormat("'{0}'{1}{2} Count={3} ({4}, {5})", (object) str, this.IsStart ? (object) " Start" : (object) (string) null, this._transportPriority ? (object) " Priority" : (object) (string) null, (object) this.ContinuityCount, (object) TsPacket.AdaptationValues[this._adaptationFieldControl], (object) TsPacket.ScramblingControlValues[this._transportScramblingControl]);
      if ((this._adaptationFieldControl & 2) != 0 && this._adaptationLength > 0)
      {
        stringBuilder1.AppendLine();
        stringBuilder1.AppendFormat("   Adaptation Length={0} Flags: ", (object) this._adaptationLength);
        if (0 != ((int) this._adaptationFlags & 128))
          stringBuilder1.Append(" Discontinuity");
        if (0 != ((int) this._adaptationFlags & 64))
          stringBuilder1.Append(" RandomAccess");
        if (0 != ((int) this._adaptationFlags & 32))
          stringBuilder1.Append(" ElementaryStreamPriority");
        if (0 != ((int) this._adaptationFlags & 16))
          stringBuilder1.Append(" PCR");
        if (0 != ((int) this._adaptationFlags & 4))
          stringBuilder1.Append(" SplicingPoint");
        if (0 != ((int) this._adaptationFlags & 2))
          stringBuilder1.Append(" Private");
        if (0 != ((int) this._adaptationFlags & 1))
          stringBuilder1.Append(" Ext");
        if (0 != ((int) this._adaptationFlags & 16))
        {
          stringBuilder1.AppendLine();
          StringBuilder stringBuilder2 = stringBuilder1;
          string format = "   PCR {0} ({1})";
          object[] objArray1 = new object[2]
          {
            (object) this.Pcr,
            null
          };
          object[] objArray2 = objArray1;
          int index = 1;
          ulong? pcr = this.Pcr;
          // ISSUE: variable of a boxed type
          var local = (ValueType) TimeSpan.FromTicks((long) (pcr.HasValue ? new double?((double) pcr.GetValueOrDefault() * (10.0 / 27.0)) : new double?()).Value);
          objArray2[index] = (object) local;
          object[] objArray3 = objArray1;
          stringBuilder2.AppendFormat(format, objArray3);
        }
      }
      return stringBuilder1.ToString();
    }
  }
}
