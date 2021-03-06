﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Segments
{
  public class SegmentManagerFactoryFinder : ContentServiceFactoryFinder<ISegmentManager, ISegmentManagerParameters>, ISegmentManagerFactoryFinder, IContentServiceFactoryFinder<ISegmentManager, ISegmentManagerParameters>
  {
    public SegmentManagerFactoryFinder(IEnumerable<ISegmentManagerFactoryInstance> factoryInstances)
      : base(Enumerable.OfType<IContentServiceFactoryInstance<ISegmentManager, ISegmentManagerParameters>>((IEnumerable) factoryInstances))
    {
    }

    public void Register(ContentType contentType, ISegmentManagerFactoryInstance factory)
    {
      this.Register(contentType, (IContentServiceFactoryInstance<ISegmentManager, ISegmentManagerParameters>) factory);
    }
  }
}
