using System;
using SM.Media.Core.Content;

namespace SM.Media.Core.Metadata
{
  public interface ISegmentMetadata
  {
    Uri Url { get; }

    ContentType ContentType { get; }

    long? Length { get; }
  }
}
