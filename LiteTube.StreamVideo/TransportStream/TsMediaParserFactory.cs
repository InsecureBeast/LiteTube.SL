using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaParser;

namespace LiteTube.StreamVideo.TransportStream
{
  public class TsMediaParserFactory : MediaParserFactoryBase<TsMediaParser>
  {
    private static readonly ContentType[] Types = new ContentType[1]
    {
      ContentTypes.TransportStream
    };

    public override ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return (ICollection<ContentType>) TsMediaParserFactory.Types;
      }
    }

    public TsMediaParserFactory(Func<TsMediaParser> factory)
      : base(factory)
    {
    }
  }
}
