using System;
using LiteTube.StreamVideo.Content;

namespace LiteTube.StreamVideo.Web
{
  public static class WebReaderManagerExtensions
  {
    public static IWebReader CreateRootReader(this IWebReaderManager webReaderManager, ContentKind contentKind, ContentType contentType = null)
    {
      return webReaderManager.CreateReader((Uri) null, contentKind, (IWebReader) null, contentType);
    }

    public static IWebReader CreateRootReader(this IWebReaderManager webReaderManager, ContentType contentType = null)
    {
      return WebReaderManagerExtensions.CreateRootReader(webReaderManager, ContentKind.Unknown, contentType);
    }
  }
}
