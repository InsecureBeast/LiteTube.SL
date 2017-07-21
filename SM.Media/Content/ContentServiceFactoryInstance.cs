// Decompiled with JetBrains decompiler
// Type: SM.Media.Content.ContentServiceFactoryInstance`3
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;

namespace SM.Media.Content
{
  public abstract class ContentServiceFactoryInstance<TServiceImplementation, TService, TParameter> : ContentServiceFactoryBase<TServiceImplementation, TService, TParameter> where TServiceImplementation : TService
  {
    private readonly Func<TServiceImplementation> _factory;

    protected ContentServiceFactoryInstance(Func<TServiceImplementation> factory)
    {
      if (null == factory)
        throw new ArgumentNullException("factory");
      this._factory = factory;
    }

    protected virtual TServiceImplementation Create()
    {
      return this._factory();
    }

    protected virtual void Initialize(TServiceImplementation instance, TParameter parameter)
    {
    }

    protected override TServiceImplementation Create(TParameter parameter, ContentType contentType)
    {
      TServiceImplementation instance = this.Create();
      this.Initialize(instance, parameter);
      return instance;
    }
  }
}
