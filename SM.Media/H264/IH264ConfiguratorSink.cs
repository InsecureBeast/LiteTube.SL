// Decompiled with JetBrains decompiler
// Type: SM.Media.H264.IH264ConfiguratorSink
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.H264
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
