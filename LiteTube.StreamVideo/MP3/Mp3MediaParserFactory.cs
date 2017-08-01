using System;
using System.Collections.Generic;
using LiteTube.StreamVideo.Content;
using LiteTube.StreamVideo.MediaParser;

namespace LiteTube.StreamVideo.MP3
{
    public class Mp3MediaParserFactory : MediaParserFactoryBase<Mp3MediaParser>
    {
        private static readonly ContentType[] Types = { ContentTypes.Mp3 };

        public Mp3MediaParserFactory(Func<Mp3MediaParser> factory) : base(factory)
        {
        }

        public override ICollection<ContentType> KnownContentTypes => Types;
    }
}
