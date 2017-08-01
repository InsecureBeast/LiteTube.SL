using System.Collections.Generic;

namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  public interface IProgramStreams
  {
    int ProgramNumber { get; }

    string Language { get; }

    ICollection<IProgramStream> Streams { get; }
  }
}
