// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacAudioSpecificConfig
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Diagnostics;

namespace SM.Media.AAC
{
  public static class AacAudioSpecificConfig
  {
    public static int? RemapObjectType1 { get; set; }

    public static byte[] DefaultAudioSpecificConfigFactory(AacFrameHeader aacFrameHeader)
    {
      int num = aacFrameHeader.Profile + 1;
      if (1 == num && AacAudioSpecificConfig.RemapObjectType1.HasValue)
      {
        num = AacAudioSpecificConfig.RemapObjectType1.Value;
        Debug.WriteLine("AacConfigurator.AudioSpecificConfig: Changing AAC object type from 1 to {0}.", (object) num);
      }
      return new byte[2]
      {
        (byte) (num << 3 | aacFrameHeader.FrequencyIndex >> 1 & 7),
        (byte) (aacFrameHeader.FrequencyIndex << 7 | (int) aacFrameHeader.ChannelConfig << 3)
      };
    }
  }
}
