using System.Diagnostics;

namespace LiteTube.StreamVideo.AAC
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
