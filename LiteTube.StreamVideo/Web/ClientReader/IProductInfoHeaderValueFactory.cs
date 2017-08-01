using System.Net.Http.Headers;

namespace LiteTube.StreamVideo.Web.ClientReader
{
    public interface IProductInfoHeaderValueFactory
    {
        ProductInfoHeaderValue Create();
    }
}
