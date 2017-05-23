using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.Common
{
    class ProductListingImpl : IProductListing
    {
        public string FormattedPrice { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }
    }
}
