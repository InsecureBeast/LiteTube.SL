// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaParser.IMediaParser
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Buffering;
using SM.Media.Metadata;
using SM.Media.TransportStream.TsParser;
using System;
using System.Collections.Generic;

namespace SM.Media.MediaParser
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
