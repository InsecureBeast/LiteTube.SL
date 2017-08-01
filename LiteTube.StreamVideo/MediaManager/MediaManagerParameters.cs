using System;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.MediaManager
{
    public class MediaManagerParameters : IMediaManagerParameters
    {
        public Action<IProgramStreams> ProgramStreamsHandler { get; set; }
    }
}
