using System;

namespace LiteTube.StreamVideo.TransportStream.TsParser.Descriptor
{
  public static class TsDescriptorTypes
  {
    private static readonly TsDescriptorType[] DescriptorTypes = new TsDescriptorType[65]
    {
      new TsDescriptorType((byte) 0, "Reserved"),
      new TsDescriptorType((byte) 1, "Reserved"),
      new TsDescriptorType((byte) 2, "Video stream"),
      new TsDescriptorType((byte) 3, "Audio stream"),
      new TsDescriptorType((byte) 4, "Hierarchy"),
      new TsDescriptorType((byte) 5, "Registration"),
      new TsDescriptorType((byte) 6, "Data stream alignment"),
      new TsDescriptorType((byte) 7, "Target background grid"),
      new TsDescriptorType((byte) 8, "Video window"),
      new TsDescriptorType((byte) 9, "CA"),
      TsIso639LanguageDescriptor.DescriptorType,
      new TsDescriptorType((byte) 11, "System clock"),
      new TsDescriptorType((byte) 12, "Multiplex buffer utilization"),
      new TsDescriptorType((byte) 13, "Copyright"),
      new TsDescriptorType((byte) 14, "Maximum bitrate"),
      new TsDescriptorType((byte) 15, "Private data indicator"),
      new TsDescriptorType((byte) 16, "Smoothing buffer"),
      new TsDescriptorType((byte) 17, "STD"),
      new TsDescriptorType((byte) 18, "IBP"),
      new TsDescriptorType((byte) 19, "ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 20, "Defined in ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 21, "Defined in ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 22, "Defined in ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 23, "Defined in ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 24, "Defined in ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 25, "Defined in ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 26, "Defined in ISO/IEC 13818-6"),
      new TsDescriptorType((byte) 27, "MPEG-4 video"),
      new TsDescriptorType((byte) 28, "MPEG-4 audio"),
      new TsDescriptorType((byte) 29, "IOD"),
      new TsDescriptorType((byte) 30, "SL"),
      new TsDescriptorType((byte) 31, "FMC"),
      new TsDescriptorType((byte) 32, "External ES ID"),
      new TsDescriptorType((byte) 33, "MuxCode"),
      new TsDescriptorType((byte) 34, "FmxBufferSize"),
      new TsDescriptorType((byte) 35, "Multiplexbuffer"),
      new TsDescriptorType((byte) 36, "Content labeling"),
      new TsDescriptorType((byte) 37, "Metadata pointer"),
      new TsDescriptorType((byte) 38, "Metadata"),
      new TsDescriptorType((byte) 39, "Metadata STD"),
      new TsDescriptorType((byte) 40, "AVC video"),
      new TsDescriptorType((byte) 41, "IPMP (defined in ISO/IEC 13818-11, MPEG-2 IPMP)"),
      new TsDescriptorType((byte) 42, "AVC timing and HRD"),
      new TsDescriptorType((byte) 43, "MPEG-2 AAC audio"),
      new TsDescriptorType((byte) 44, "FlexMuxTiming"),
      new TsDescriptorType((byte) 45, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 46, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 44, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 47, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 48, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 49, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 50, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 51, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 52, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 53, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 54, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 55, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 56, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 57, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 58, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 59, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 60, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 61, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 62, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved"),
      new TsDescriptorType((byte) 63, "ITU-T Rec. H.222.0 | ISO/IEC 13818-1 Reserved")
    };

    static TsDescriptorTypes()
    {
      TsDescriptorTypes.Validate();
    }

    public static void Validate()
    {
      for (int index = 0; index < TsDescriptorTypes.DescriptorTypes.Length; ++index)
      {
        TsDescriptorType tsDescriptorType = TsDescriptorTypes.DescriptorTypes[index];
        if ((int) tsDescriptorType.Code != index)
          throw new InvalidOperationException(string.Concat(new object[4]
          {
            (object) "Descriptor type mismatch ",
            (object) (int) tsDescriptorType.Code,
            (object) " != ",
            (object) index
          }));
      }
    }

    public static TsDescriptorType GetDescriptorType(byte code)
    {
      if ((int) code < TsDescriptorTypes.DescriptorTypes.Length)
        return TsDescriptorTypes.DescriptorTypes[(int) code];
      return (TsDescriptorType) null;
    }
  }
}
