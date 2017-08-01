using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace LiteTube.StreamVideo.Platform
{
    public interface IMediaStreamConfiguration
    {
        IStreamSource VideoStreamSource { get; }

        IStreamSource AudioStreamSource { get; }

        ICollection<MediaStreamDescription> Descriptions { get; }

        IDictionary<MediaSourceAttributesKeys, string> Attributes { get; }

        TimeSpan? Duration { get; }
    }
}
