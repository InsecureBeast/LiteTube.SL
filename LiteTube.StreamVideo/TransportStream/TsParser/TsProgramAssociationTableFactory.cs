using System;

namespace LiteTube.StreamVideo.TransportStream.TsParser
{
  public class TsProgramAssociationTableFactory : ITsProgramAssociationTableFactory
  {
    private readonly ITsProgramMapTableFactory _programMapTableFactory;

    public TsProgramAssociationTableFactory(ITsProgramMapTableFactory programMapTableFactory)
    {
      if (null == programMapTableFactory)
        throw new ArgumentNullException("programMapTableFactory");
      this._programMapTableFactory = programMapTableFactory;
    }

    public TsProgramAssociationTable Create(ITsDecoder decoder, Func<int, bool> programFilter, Action<IProgramStreams> streamFilter)
    {
      return new TsProgramAssociationTable(decoder, this._programMapTableFactory, programFilter, streamFilter);
    }
  }
}
