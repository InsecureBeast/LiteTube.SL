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
        IProductListing GetProductInfo(string name);
        Task BuyProductAsync(string productId, string productName);
    }

    public interface IProductListing
    {
        string FormattedPrice { get; }
        string Name { get; }
        string ProductId { get; }
    }
}
