﻿// Decompiled with JetBrains decompiler
// Type: SM.Media.Content.IContentServiceFactoryFinder`2
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

namespace SM.Media.Content
{
  public interface IContentServiceFactoryFinder<TService, TParameter>
  {
    IContentServiceFactoryInstance<TService, TParameter> GetFactory(ContentType contentType);

    void Register(ContentType contentType, IContentServiceFactoryInstance<TService, TParameter> factory);

    void Deregister(ContentType contentType);
  }
}
