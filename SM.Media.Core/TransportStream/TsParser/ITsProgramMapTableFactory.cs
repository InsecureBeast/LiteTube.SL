using System;

namespace SM.Media.Core.TransportStream.TsParser
{
  public interface ITsProgramMapTableFactory
  {
    TsProgramMapTable Create(ITsDecoder decoder, int programNumber, uint pid, Action<IProgramStreams> streamFilter);
  }
}
