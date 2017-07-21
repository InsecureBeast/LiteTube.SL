using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SM.Media.Core.Content;

namespace SM.Media.Core.MediaParser
{
  public class MediaParserFactoryFinder : ContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>, IMediaParserFactoryFinder, IContentServiceFactoryFinder<IMediaParser, IMediaParserParameters>
  {
    public MediaParserFactoryFinder(IEnumerable<IMediaParserFactoryInstance> factoryInstances)
      : base(Enumerable.OfType<IContentServiceFactoryInstance<IMediaParser, IMediaParserParameters>>((IEnumerable) factoryInstances))
    {
    }

    public void Register(ContentType contentType, IMediaParserFactoryInstance factory)
    {
      this.Register(contentType, (IContentServiceFactoryInstance<IMediaParser, IMediaParserParameters>) factory);
    }
  }
}
