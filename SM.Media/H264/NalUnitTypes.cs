// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.NalUnitTypes
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Diagnostics;

namespace SM.Media.H264
{
  public static class NalUnitTypes
  {
    private static readonly NalUnitTypeDescriptor[] Types = new NalUnitTypeDescriptor[25]
    {
      new NalUnitTypeDescriptor(NalUnitType.Uns, "UNS", "Unspecified"),
      new NalUnitTypeDescriptor(NalUnitType.Slice, "SLICE", "Coded slice of a non-IDR picture"),
      new NalUnitTypeDescriptor(NalUnitType.Dpa, "DPA", "Coded slice data partition A"),
      new NalUnitTypeDescriptor(NalUnitType.Dpb, "DPB", "Coded slice data partition B"),
      new NalUnitTypeDescriptor(NalUnitType.Dpc, "DPC", "Coded slice data partition C"),
      new NalUnitTypeDescriptor(NalUnitType.Idr, "IDR", "Coded slice of an IDR picture"),
      new NalUnitTypeDescriptor(NalUnitType.Sei, "SEI", "Supplemental enhancement information"),
      new NalUnitTypeDescriptor(NalUnitType.Sps, "SPS", "Sequence parameter set"),
      new NalUnitTypeDescriptor(NalUnitType.Pps, "PPS", "Picture parameter set"),
      new NalUnitTypeDescriptor(NalUnitType.Aud, "AUD", "Access unit delimiter"),
      new NalUnitTypeDescriptor(NalUnitType.EoSeq, "EOSEQ", "End of sequence"),
      new NalUnitTypeDescriptor(NalUnitType.EoStream, "EOSTREAM", "End of stream"),
      new NalUnitTypeDescriptor(NalUnitType.Fill, "FILL", "Filler data"),
      new NalUnitTypeDescriptor(NalUnitType.SpsExt, "SPS_EXT", "Sequence parameter set extension"),
      new NalUnitTypeDescriptor(NalUnitType.Prefix, "PREFIX", "Prefix NAL unit"),
      new NalUnitTypeDescriptor(NalUnitType.SubSps, "SUB_SPS", "Subset sequence parameter set"),
      new NalUnitTypeDescriptor(NalUnitType.Rsv16, "RSV16", "Reserved"),
      new NalUnitTypeDescriptor(NalUnitType.Rsv17, "RSV17", "Reserved"),
      new NalUnitTypeDescriptor(NalUnitType.Rsv18, "RSV18", "Reserved"),
      new NalUnitTypeDescriptor(NalUnitType.SlcAux, "SLC_AUX", "Coded slice of an auxiliary coded picture without partitioning"),
      new NalUnitTypeDescriptor(NalUnitType.SlcExt, "SLC_EXT", "Coded slice extension"),
      new NalUnitTypeDescriptor(NalUnitType.SlcDv, "SLC_DV", "Coded slice extension for depth view components"),
      new NalUnitTypeDescriptor(NalUnitType.Rsv22, "RSV22", "Reserved"),
      new NalUnitTypeDescriptor(NalUnitType.Rsv23, "RSV23", "Reserved"),
      new NalUnitTypeDescriptor(NalUnitType.Vdrd, "VDRD", "View and Dependency Representation Delimiter")
    };

    static NalUnitTypes()
    {
      NalUnitTypes.Validate();
    }

    [Conditional("DEBUG")]
    private static void Validate()
    {
      for (int index = 0; index < NalUnitTypes.Types.Length; ++index)
      {
        int num = (int) NalUnitTypes.Types[index].Type;
        if (num != index)
          throw new Exception(string.Format("Invalid table {0} != {1}", new object[2]
          {
            (object) num,
            (object) index
          }));
      }
    }

    public static NalUnitTypeDescriptor GetNalUnitType(NalUnitType nalUnitType)
    {
      int index = (int) nalUnitType;
      if (index < NalUnitTypes.Types.Length)
        return NalUnitTypes.Types[index];
      return (NalUnitTypeDescriptor) null;
    }
  }
}
