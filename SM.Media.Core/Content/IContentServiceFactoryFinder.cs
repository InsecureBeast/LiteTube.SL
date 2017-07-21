using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Content
{
    public interface IContentServiceFactoryFinder<TService, TParameter>
    {
        IContentServiceFactoryInstance<TService, TParameter> GetFactory(ContentType contentType);

        void Register(ContentType contentType, IContentServiceFactoryInstance<TService, TParameter> factory);

        void Deregister(ContentType contentType);
    }
}
