﻿using System;
using System.Collections.Generic;
using SM.Media.Core.Content;
using SM.Media.Core.MediaParser;

namespace SM.Media.Core.AAC
{
    public class AacMediaParserFactory : MediaParserFactoryBase<AacMediaParser>
    {
        public AacMediaParserFactory(Func<AacMediaParser> factory) : base(factory)
        {
        }

        private static readonly ContentType[] Types = new ContentType[1]
        {
            ContentTypes.Aac
        };

        public override ICollection<ContentType> KnownContentTypes
        {
            get { return Types; }
        }
    }
}
