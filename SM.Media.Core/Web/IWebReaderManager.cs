using System;
using SM.Media.Core.Content;

namespace SM.Media.Core.Web
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
