using System;
using System.Collections.Generic;
using SM.Media.Core.Buffering;
using SM.Media.Core.Metadata;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.MediaParser
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
