using System.Collections.Generic;

namespace SM.Media.Core.TransportStream.TsParser
{
  public interface IProgramStreams
  {
    int ProgramNumber { get; }

    string Language { get; }

    ICollection<IProgramStream> Streams { get; }
  }
}
