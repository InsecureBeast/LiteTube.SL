using System;
using System.Collections.Generic;
using SM.Media.Core.Content;
using SM.Media.Core.MediaParser;

namespace SM.Media.Core.TransportStream
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
