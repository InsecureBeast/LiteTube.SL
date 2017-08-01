using System;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.MediaManager
{
    public interface IMediaManagerParameters
    {
        Action<IProgramStreams> ProgramStreamsHandler { get; set; }
    }
}
