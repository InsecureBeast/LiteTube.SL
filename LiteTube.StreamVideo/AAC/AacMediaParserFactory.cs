using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaParser;

namespace LiteTube.StreamVideo.AAC
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
