// Decompiled with JetBrains decompiler
// Type: SM.Media.Configuration.ConfigurationSourceExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;

namespace SM.Media.Configuration
{
  public static class ConfigurationSourceExtensions
  {
    public static string GetLanguage(this IConfigurationSource configurationSource)
    {
      IMediaStreamMetadata mediaStreamMetadata = configurationSource.MediaStreamMetadata;
      if (null == mediaStreamMetadata || string.IsNullOrWhiteSpace(mediaStreamMetadata.Language))
        return (string) null;
      return mediaStreamMetadata.Language;
    }
  }
}
