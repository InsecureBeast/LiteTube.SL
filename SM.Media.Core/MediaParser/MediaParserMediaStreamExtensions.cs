using System;
using System.Collections.Generic;
using SM.Media.Core.Configuration;

namespace SM.Media.Core.MediaParser
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
