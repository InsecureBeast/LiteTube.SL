using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace LiteTube.Common
{
    internal class PurchaseMock : IPurchase
    {
        public Task Init()
        {
            return Task.Run(() => { });
        }

        public IProductListing GetProductInfo(string name)
        {
            var res = new ProductListingImpl
            {
                FormattedPrice = "3.99$",
                Name = name,
                ProductId = name + " ID"
            };

            return res;
        }

        public Task BuyProductAsync(string productId, string productName)
        {
            return Task.Run(() => { });
        }
    }
}