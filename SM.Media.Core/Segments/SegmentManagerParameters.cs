using System;
using System.Collections.Generic;
using SM.Media.Core.Web;

namespace SM.Media.Core.Segments
{
  public class SegmentManagerParameters : ISegmentManagerParameters
  {
    public ICollection<Uri> Source { get; set; }

    public IWebReader WebReader { get; set; }
  }
}
