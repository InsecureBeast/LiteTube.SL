using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace LiteTube.Common
{
    public interface IPurchase
    {
        Task Init();
        ProductListing GetProductInfo(string name);
        Task BuyProductAsync(string productId, string productName);
    }
}
