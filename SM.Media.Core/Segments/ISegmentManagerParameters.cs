using System;
using System.Collections.Generic;
using SM.Media.Core.Web;

namespace SM.Media.Core.Segments
{
  public interface ISegmentManagerParameters
  {
    ICollection<Uri> Source { get; set; }

    IWebReader WebReader { get; set; }
  }
}
