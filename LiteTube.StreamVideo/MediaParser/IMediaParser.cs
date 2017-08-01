using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Buffering;
using LiteTube.StreamVideo.Metadata;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.MediaParser
{
  public interface IMediaParser : IDisposable
  {
    ICollection<IMediaParserMediaStream> MediaStreams { get; }

    bool EnableProcessing { get; set; }

    TimeSpan StartPosition { get; set; }

    event EventHandler ConfigurationComplete;

    void ProcessEndOfData();

    void ProcessData(byte[] buffer, int offset, int length);

    void FlushBuffers();

    void Initialize(IBufferingManager bufferingManager, Action<IProgramStreams> programStreamsHandler = null);

    void InitializeStream(IStreamMetadata streamMetadata);

    void StartSegment(ISegmentMetadata segmentMetadata);

    void SetTrackMetadata(ITrackMetadata trackMetadata);
  }
}
