﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.Utility;

namespace LiteTube.StreamVideo.MediaParser
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
