using System.Collections.Generic;
using LiteTube.StreamVideo.TransportStream.TsParser;

namespace LiteTube.StreamVideo.Pes
{
  public interface IPesStreamFactoryInstance
  {
    ICollection<byte> SupportedStreamTypes { get; }

    PesStreamHandler Create(PesStreamParameters parameters);
  }
}
