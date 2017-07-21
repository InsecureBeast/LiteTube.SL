using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SM.Media.Core.Web.ClientReader
{
    public class ProductInfoHeaderValueFactory : IProductInfoHeaderValueFactory
    {
        private readonly IUserAgent _userAgent;

        public ProductInfoHeaderValueFactory(IUserAgent userAgent)
        {
            if (null == userAgent)
                throw new ArgumentNullException("userAgent");
            this._userAgent = userAgent;
        }

        public ProductInfoHeaderValue Create()
        {
            string name = this._userAgent.Name;
            string version = this._userAgent.Version;
            try
            {
                return new ProductInfoHeaderValue(name.Replace(' ', '-'), version);
            }
            catch (FormatException ex)
            {
                Debug.WriteLine("ProductInfoHeaderValueFactory.Create({0}, {1}) unable to construct ProductInfoHeaderValue: {2}", (object)name, (object)version, (object)ex.Message);
                return (ProductInfoHeaderValue)null;
            }
        }
    }
}
