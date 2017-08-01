using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Web;

namespace LiteTube.StreamVideo.Segments
{
  public interface ISegmentManagerParameters
  {
    ICollection<Uri> Source { get; set; }

    IWebReader WebReader { get; set; }
  }
}
