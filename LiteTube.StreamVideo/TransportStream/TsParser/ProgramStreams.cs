using System.Collections.Generic;

namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  internal class ProgramStreams : IProgramStreams
  {
    public string Language { get; set; }

    public int ProgramNumber { get; internal set; }

    public ICollection<IProgramStream> Streams { get; internal set; }

    public class ProgramStream : IProgramStream
    {
      public string Language { get; set; }

      public uint Pid { get; internal set; }

      public TsStreamType StreamType { get; internal set; }

      public bool BlockStream { get; set; }
    }
  }
}
