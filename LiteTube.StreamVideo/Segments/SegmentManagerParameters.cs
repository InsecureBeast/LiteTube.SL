using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Segments
{
  public class SegmentManagerParameters : ISegmentManagerParameters
  {
    public ICollection<Uri> Source { get; set; }

    public IWebReader WebReader { get; set; }
  }
}
