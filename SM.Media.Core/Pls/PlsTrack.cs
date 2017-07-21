using System;

namespace SM.Media.Core.Pls
{
  public class PlsTrack
  {
    public string File { get; set; }

    public string Title { get; set; }

    public TimeSpan? Length { get; set; }
  }
}
