using System.Collections.Generic;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core.Pes
{
  public interface IPesStreamFactoryInstance
  {
    ICollection<byte> SupportedStreamTypes { get; }

    PesStreamHandler Create(PesStreamParameters parameters);
  }
}
