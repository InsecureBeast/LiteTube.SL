using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SM.Media.Core.Configuration;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;
using SM.Media.Core.Utility;

namespace SM.Media.Core.H264
{
  internal sealed class H264Configurator : VideoConfigurator, IConfigurationSource
    {
    private readonly StringBuilder _codecPrivateData = new StringBuilder();
    private readonly IH264Reader _h264Reader = (IH264Reader) new H264Reader();
    private IEnumerable<byte> _ppsBytes;
    private IEnumerable<byte> _spsBytes;

    public override string CodecPrivateData
    {
      get
      {
        if (this._codecPrivateData.Length > 0)
          return this._codecPrivateData.ToString();
        this._codecPrivateData.Append("00000001");
        foreach (byte num in this.RbspEscape(this._spsBytes))
          this._codecPrivateData.Append(num.ToString("X2"));
        this._codecPrivateData.Append("00000001");
        foreach (byte num in this.RbspEscape(this._ppsBytes))
          this._codecPrivateData.Append(num.ToString("X2"));
        return this._codecPrivateData.ToString();
      }
    }

    public H264Configurator(IMediaStreamMetadata mediaStreamMetadata, string streamDescription = null)
      : base("H264", ContentTypes.H264, mediaStreamMetadata)
    {
      this.StreamDescription = streamDescription;
    }

    public void ParseSpsBytes(ICollection<byte> value)
    {
      if (EnumerableExtensions.SequencesAreEquivalent<byte>((IEnumerable<byte>) value, this._spsBytes))
        return;
      this._spsBytes = (IEnumerable<byte>) Enumerable.ToArray<byte>((IEnumerable<byte>) value);
      using (H264Bitstream r = new H264Bitstream(this._spsBytes))
        this._h264Reader.ReadSps(r);
      this.CheckConfigure();
    }

    public void ParsePpsBytes(ICollection<byte> value)
    {
      if (EnumerableExtensions.SequencesAreEquivalent<byte>((IEnumerable<byte>) value, this._ppsBytes))
        return;
      this._ppsBytes = (IEnumerable<byte>) Enumerable.ToArray<byte>((IEnumerable<byte>) value);
      using (H264Bitstream r = new H264Bitstream(this._ppsBytes))
        this._h264Reader.ReadPps(r);
      this.CheckConfigure();
    }

    private IEnumerable<byte> RbspEscape(IEnumerable<byte> sequence)
    {
      int zeroCount = 0;
      foreach (byte num in sequence)
      {
        int previousZeroCount = zeroCount;
        if (0 == (int) num)
          ++zeroCount;
        else
          zeroCount = 0;
        if (((int) num & -4) == 0 && previousZeroCount == 2)
        {
          zeroCount = 0;
          yield return 3;
        }
        yield return num;
      }
    }

    private void CheckConfigure()
    {
      if (this.IsConfigured)
        return;
      this._codecPrivateData.Length = 0;
      if (this._spsBytes == null || null == this._ppsBytes || !this._h264Reader.ReaderCheckConfigure(this))
        return;
      this.Name = this._h264Reader.Name;
      this.Height = this._h264Reader.Height;
      this.Width = this._h264Reader.Width;
      this.FrameRateNumerator = this._h264Reader.FrameRateNumerator;
      this.FrameRateDenominator = this._h264Reader.FrameRateDenominator;
      if (this.FrameRateDenominator.HasValue && this.FrameRateNumerator.HasValue)
      {
        string format = "H264Configurator.ComputeFrameRate() {0}/{1} -> {2:F4} fps";
        object[] objArray1 = new object[3]
        {
          (object) this.FrameRateNumerator,
          (object) this.FrameRateDenominator,
          null
        };
        object[] objArray2 = objArray1;
        int index = 2;
        int? frameRateNumerator = this.FrameRateNumerator;
        double num = (double) this.FrameRateDenominator.Value;
        // ISSUE: variable of a boxed type
        var local = (ValueType) (frameRateNumerator.HasValue ? new double?((double) frameRateNumerator.GetValueOrDefault() / num) : new double?());
        objArray2[index] = (object) local;
        object[] objArray3 = objArray1;
        Debug.WriteLine(format, objArray3);
      }
      Debug.WriteLine("Configuration " + this.Name);
      this.SetConfigured();
    }

    internal void ParseSliceHeader(IList<byte> buffer)
    {
      using (H264Bitstream r = new H264Bitstream((IEnumerable<byte>) buffer))
      {
        r.ReadBits(1);
        r.ReadBits(2);
        uint num = r.ReadBits(5);
        this._h264Reader.ReadSliceHeader(r, 5 == (int) num);
      }
      this._h264Reader.TryReparseTimingSei(this);
      this.CheckConfigure();
    }

    internal void ParseSei(ICollection<byte> buffer)
    {
      using (H264Bitstream r = new H264Bitstream((IEnumerable<byte>) buffer))
      {
        r.ReadBits(1);
        r.ReadBits(2);
        r.ReadBits(5);
        this._h264Reader.ReadSei(r, buffer);
      }
      this.CheckConfigure();
    }

    internal void ParseAud(IList<byte> buffer)
    {
      using (H264Bitstream h264Bitstream = new H264Bitstream((IEnumerable<byte>) buffer))
      {
        h264Bitstream.ReadBits(1);
        h264Bitstream.ReadBits(2);
        h264Bitstream.ReadBits(5);
        h264Bitstream.ReadBits(3);
      }
    }
  }
}
