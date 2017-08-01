using System;

namespace LiteTube.StreamVideo.Content
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
