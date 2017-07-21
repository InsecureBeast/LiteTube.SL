// Decompiled with JetBrains decompiler
// Type: SM.Media.AAC.AacDecoderSettings
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Utility;

namespace SM.Media.AAC
{
  public static class AacDecoderSettings
  {
    private static readonly ResettableParameters<SM.Media.AAC.AacDecoderParameters> AacDecoderParameters = new ResettableParameters<SM.Media.AAC.AacDecoderParameters>();

    public static SM.Media.AAC.AacDecoderParameters Parameters
    {
      get
      {
        return AacDecoderSettings.AacDecoderParameters.Parameters;
      }
      set
      {
        AacDecoderSettings.AacDecoderParameters.Parameters = value;
      }
    }
  }
}
