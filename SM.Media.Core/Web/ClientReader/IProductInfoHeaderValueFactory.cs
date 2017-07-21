using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Web.ClientReader
{
    public interface IProductInfoHeaderValueFactory
    {
        ProductInfoHeaderValue Create();
    }
}
