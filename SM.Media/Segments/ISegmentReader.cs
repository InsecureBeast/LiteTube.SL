// Decompiled with JetBrains decompiler
// Type: SM.Media.Segments.ISegmentReader
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Metadata;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Segments
{
  public interface ISegmentReader : IDisposable
  {
    Uri Url { get; }

    bool IsEof { get; }

    Task<int> ReadAsync(byte[] buffer, int offset, int length, Action<ISegmentMetadata> setMetadata, CancellationToken cancellationToken);

    void Close();
  }
}
