using System;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Metadata
{
  public interface ISegmentMetadata
  {
    Uri Url { get; }

    ContentType ContentType { get; }

    long? Length { get; }
  }
}
