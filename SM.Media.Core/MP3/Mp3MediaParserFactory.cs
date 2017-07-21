using System;
using System.Collections.Generic;
using SM.Media.Core.Content;
using SM.Media.Core.MediaParser;

namespace SM.Media.Core.MP3
{
  public class Mp3MediaParserFactory : MediaParserFactoryBase<Mp3MediaParser>
  {
    private static readonly ContentType[] Types = new ContentType[1]
    {
      ContentTypes.Mp3
    };

    public override ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return (ICollection<ContentType>) Mp3MediaParserFactory.Types;
      }
    }

    public Mp3MediaParserFactory(Func<Mp3MediaParser> factory)
      : base(factory)
    {
    }
  }
}
