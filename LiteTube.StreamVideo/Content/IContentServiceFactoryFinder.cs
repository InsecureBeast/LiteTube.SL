namespace LiteTube.StreamVideo.Content
{
    public interface IContentServiceFactoryFinder<TService, TParameter>
    {
        IContentServiceFactoryInstance<TService, TParameter> GetFactory(ContentType contentType);

        void Register(ContentType contentType, IContentServiceFactoryInstance<TService, TParameter> factory);

        void Deregister(ContentType contentType);
    }
}
