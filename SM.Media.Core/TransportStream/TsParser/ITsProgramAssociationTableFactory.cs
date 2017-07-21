using System;

namespace SM.Media.Core.TransportStream.TsParser
{
  public interface ITsProgramAssociationTableFactory
  {
    TsProgramAssociationTable Create(ITsDecoder decoder, Func<int, bool> programFilter, Action<IProgramStreams> streamFilter);
  }
}
