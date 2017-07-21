using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core
{
    public interface IStreamSource
    {
        bool HasSample { get; }

        float? BufferingProgress { get; }

        bool IsEof { get; }

        TimeSpan? PresentationTimestamp { get; }

        TsPesPacket GetNextSample();

        void FreeSample(TsPesPacket packet);

        bool DiscardPacketsBefore(TimeSpan value);
    }
}
