// Decompiled with JetBrains decompiler
// Type: SM.Media.MediaParser.MediaParserFactoryBase`1
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using SM.Media.Content;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.MediaParser
{
  public abstract class MediaParserFactoryBase<TMediaParser> : IMediaParserFactoryInstance, IContentServiceFactoryInstance<IMediaParser, IMediaParserParameters> where TMediaParser : IMediaParser
  {
    private readonly Func<TMediaParser> _parserFactory;

    public abstract ICollection<ContentType> KnownContentTypes { get; }

    protected MediaParserFactoryBase(Func<TMediaParser> parserFactory)
    {
      if (null == parserFactory)
        throw new ArgumentNullException("parserFactory");
      this._parserFactory = parserFactory;
    }

    public Task<IMediaParser> CreateAsync(IMediaParserParameters parameter, ContentType contentType, CancellationToken cancellationToken)
    {
      return TaskEx.FromResult<IMediaParser>((IMediaParser) this._parserFactory());
    }
  }
}
