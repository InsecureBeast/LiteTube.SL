using System;
using System.Text;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Program
{
    public class MediaTrack
    {
        public Uri Url { get; set; }
        public string Title { get; set; }
        public bool UseNativePlayer { get; set; }
        public ContentType ContentType { get; set; }
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Title))
            {
                stringBuilder.Append('"');
                stringBuilder.Append(Title);
                stringBuilder.Append('"');
            }
            if (null != Url)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(' ');
                stringBuilder.Append('<' + Url.OriginalString + '>');
            }
            if (UseNativePlayer)
            {
                if (stringBuilder.Length > 0)
                    stringBuilder.Append(' ');
                stringBuilder.Append("[native]");
            }
            return stringBuilder.ToString();
        }
    }
}
