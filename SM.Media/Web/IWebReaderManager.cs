﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Web.WebReaderManagerExtensions
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media.Content;
using System;

namespace SM.Media.Web
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
