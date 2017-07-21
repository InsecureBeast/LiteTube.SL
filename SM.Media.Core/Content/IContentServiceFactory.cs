using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Core.Content
{
    public interface IContentServiceFactory<TService, TParameter>
    {
        Task<TService> CreateAsync(TParameter parameter, ContentType contentType, CancellationToken cancellationToken);
    }
}
