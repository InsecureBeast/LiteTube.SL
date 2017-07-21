using System;
using SM.Media.Core.TransportStream.TsParser.Descriptor;

namespace SM.Media.Core.TransportStream.TsParser
{
  public class TsProgramMapTableFactory : ITsProgramMapTableFactory
  {
    private readonly ITsDescriptorFactory _descriptorFactory;

    public TsProgramMapTableFactory(ITsDescriptorFactory descriptorFactory)
    {
      if (null == descriptorFactory)
        throw new ArgumentNullException("descriptorFactory");
      this._descriptorFactory = descriptorFactory;
    }

    public TsProgramMapTable Create(ITsDecoder decoder, int programNumber, uint pid, Action<IProgramStreams> streamFilter)
    {
      return new TsProgramMapTable(decoder, this._descriptorFactory, programNumber, pid, streamFilter);
    }
  }
}
