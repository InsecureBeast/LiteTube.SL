using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SM.Media.Core.Platform
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
