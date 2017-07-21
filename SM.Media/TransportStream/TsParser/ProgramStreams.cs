// Decompiled with JetBrains decompiler
// Type: SM.Media.TransportStream.TsParser.ProgramStreams
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System.Collections.Generic;

namespace SM.Media.TransportStream.TsParser
{
  internal class ProgramStreams : IProgramStreams
  {
    public string Language { get; set; }

    public int ProgramNumber { get; internal set; }

    public ICollection<IProgramStream> Streams { get; internal set; }

    public class ProgramStream : IProgramStream
    {
      public string Language { get; set; }

      public uint Pid { get; internal set; }

      public TsStreamType StreamType { get; internal set; }

      public bool BlockStream { get; set; }
    }
  }
}
