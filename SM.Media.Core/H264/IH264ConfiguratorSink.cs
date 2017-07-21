using System.Collections.Generic;

namespace SM.Media.Core.H264
{
  public interface IH264ConfiguratorSink
  {
    IEnumerable<byte> SpsBytes { get; }

    IEnumerable<byte> PpsBytes { get; }

    bool IsConfigured { get; }

    void ParseSpsBytes(ICollection<byte> value);

    void ParsePpsBytes(ICollection<byte> value);
  }
}
