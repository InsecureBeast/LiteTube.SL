using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SM.Media.Core.Platform
{
    public class MediaStreamConfiguration : IMediaStreamConfiguration
    {
        public IStreamSource VideoStreamSource { get; set; }

        public IStreamSource AudioStreamSource { get; set; }

        public ICollection<MediaStreamDescription> Descriptions { get; set; }

        public IDictionary<MediaSourceAttributesKeys, string> Attributes { get; set; }

        public TimeSpan? Duration { get; set; }
    }
}
