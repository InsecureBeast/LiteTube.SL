using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.MediaParser
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
