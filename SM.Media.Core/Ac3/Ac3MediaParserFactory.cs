using System;
using System.Collections.Generic;
using SM.Media.Core.Content;
using SM.Media.Core.MediaParser;

namespace SM.Media.Core.Ac3
{
  public class Ac3MediaParserFactory : MediaParserFactoryBase<Ac3MediaParser>
  {
    private static readonly ContentType[] Types = new ContentType[1]
    {
      ContentTypes.Ac3
    };

    public override ICollection<ContentType> KnownContentTypes
    {
      get
      {
        return (ICollection<ContentType>) Ac3MediaParserFactory.Types;
      }
    }

    public Ac3MediaParserFactory(Func<Ac3MediaParser> factory)
      : base(factory)
    {
    }
  }
}
