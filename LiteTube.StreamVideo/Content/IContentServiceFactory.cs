using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Content
{
    public interface IContentServiceFactory<TService, TParameter>
    {
        Task<TService> CreateAsync(TParameter parameter, ContentType contentType, CancellationToken cancellationToken);
    }
}
