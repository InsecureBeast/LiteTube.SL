// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaParser.MediaParserMediaStreamExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Configuration;
using System;
using System.Collections.Generic;

namespace SM.Media.MediaParser
{
  public static class MediaParserMediaStreamExtensions
  {
    public static IMediaConfiguration CreateMediaConfiguration(this IEnumerable<IMediaParserMediaStream> mediaParserMediaStreams, TimeSpan? duration)
    {
      MediaConfiguration mediaConfiguration = new MediaConfiguration()
      {
        Duration = duration
      };
      List<IMediaParserMediaStream> list = (List<IMediaParserMediaStream>) null;
      foreach (IMediaParserMediaStream parserMediaStream in mediaParserMediaStreams)
      {
        IConfigurationSource configurationSource = parserMediaStream.ConfigurationSource;
        if (configurationSource is IVideoConfigurationSource && null == mediaConfiguration.Video)
          mediaConfiguration.Video = parserMediaStream;
        else if (configurationSource is IAudioConfigurationSource && null == mediaConfiguration.Audio)
        {
          mediaConfiguration.Audio = parserMediaStream;
        }
        else
        {
          if (null == list)
            list = new List<IMediaParserMediaStream>();
          list.Add(parserMediaStream);
        }
      }
      mediaConfiguration.AlternateStreams = (IReadOnlyCollection<IMediaParserMediaStream>) list;
      return (IMediaConfiguration) mediaConfiguration;
    }
  }
}
