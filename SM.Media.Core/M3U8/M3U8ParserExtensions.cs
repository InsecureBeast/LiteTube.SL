using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM.Media.Core.Utility;
using SM.Media.Core.Utility.TextEncodings;
using SM.Media.Core.Web;

namespace SM.Media.Core.M3U8
{
  public static class M3U8ParserExtensions
  {
    public static Uri ResolveUrl(this M3U8Parser parser, string url)
    {
      return parser.ResolveUrl(new Uri(url, UriKind.RelativeOrAbsolute));
    }

    public static void Parse(this M3U8Parser parser, Uri baseUrl, Stream stream, Encoding encoding = null)
    {
      if (null == encoding)
        encoding = UriExtensions.HasExtension(baseUrl, ".m3u") ? SmEncodings.Latin1 : Encoding.UTF8;
      using (StreamReader streamReader = new StreamReader(stream, encoding))
        M3U8ParserExtensions.Parse(parser, baseUrl, (TextReader) streamReader);
    }

    public static Task<Uri> ParseAsync(this M3U8Parser parser, IWebReader webReader, IRetryManager retryManager, Uri playlist, CancellationToken cancellationToken)
    {
      IRetry retry = RetryManagerExtensions.CreateWebRetry(retryManager, 2, 250);
      return retry.CallAsync<Uri>((Func<Task<Uri>>) (() => WebReaderExtensions.ReadStreamAsync<Uri>(webReader, playlist, retry, (Func<Uri, Stream, Uri>) ((actualPlaylist, stream) =>
      {
        M3U8ParserExtensions.Parse(parser, actualPlaylist, stream, (Encoding) null);
        return actualPlaylist;
      }), cancellationToken)), cancellationToken);
    }

    public static void Parse(this M3U8Parser parser, Uri baseUrl, TextReader textReader)
    {
      parser.Parse(baseUrl, M3U8ParserExtensions.GetExtendedLines(textReader));
    }

    private static IEnumerable<string> GetExtendedLines(TextReader textReader)
    {
      bool eof = false;
      StringBuilder sb = new StringBuilder();
      while (!eof)
      {
        sb.Length = 0;
        string line = (string) null;
        while (true)
        {
          do
          {
            line = textReader.ReadLine();
            if (null != line)
            {
              line = line.Trim();
              if (!line.EndsWith("\\"))
                goto label_9;
            }
            else
              goto label_2;
          }
          while (line.Length < 1);
          if (sb.Length > 0)
            sb.Append(' ');
          sb.Append(line.Substring(0, line.Length - 1));
        }
label_2:
        eof = true;
label_9:
        if (sb.Length > 0)
        {
          sb.Append(' ');
          sb.Append(line);
          line = sb.ToString();
          sb.Length = 0;
        }
        if (null != line)
          yield return line;
      }
    }
  }
}
