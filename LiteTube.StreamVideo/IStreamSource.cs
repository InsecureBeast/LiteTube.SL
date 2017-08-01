using System;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo
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
