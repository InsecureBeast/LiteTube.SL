using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.Content;
using SM.Media.Core.Metadata;

namespace SM.Media.Core
{
    public interface IConfigurationSource
    {
        string CodecPrivateData { get; }

        string Name { get; }

        string StreamDescription { get; }

        int? Bitrate { get; }

        ContentType ContentType { get; }

        IMediaStreamMetadata MediaStreamMetadata { get; }

        bool IsConfigured { get; }

        event EventHandler ConfigurationComplete;
    }
}
