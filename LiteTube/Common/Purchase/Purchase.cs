using LiteTube.Common.Exceptions;
using LiteTube.Common.Helpers;
using LiteTube.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Store;

namespace LiteTube.Common
{
    class Purchase : IPurchase
    {
        private ListingInformation _listing;

        public async Task Init()
        {
            // Initialize the license info for use in the app that is uploaded to the Store.
            // Uncomment the following line in the release version of your app.
            //var licenseInformation = CurrentApp.LicenseInformation;

            // Initialize the license info for testing.
            // Comment the following line in the release version of your app.
            //var licenseInformation = CurrentAppSimulator.LicenseInformation;

            _listing = await CurrentApp.LoadListingInformationAsync();
            //_listing = await CurrentApp.LoadListingInformationByProductIdsAsync(new string[] { "donate1"});
            //var product1 = _listing.ProductListings.Count;
        }

        public IProductListing GetProductInfo(string name)
        {
            try
            {
                var listing = _listing.ProductListings[name];
                var res = new ProductListingImpl
                {
                    ProductId = listing.ProductId,
                    Name = listing.Name,
                    FormattedPrice = listing.FormattedPrice
                };

                return res;
            }
            catch (Exception)
            {
                return null;
                //rootPage.NotifyUser("LoadListingInformationAsync API call failed", NotifyType.ErrorMessage);
            }
        }

        public async Task BuyProductAsync(string productId, string productName)
        {
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            if (!licenseInformation.ProductLicenses[productId].IsActive)
            {
                try
                {
                    await CurrentApp.RequestProductPurchaseAsync(productId);
                    if (licenseInformation.ProductLicenses[productId].IsActive)
                    {
                        LayoutHelper.InvokeFromUiThread(() => 
                        {
                            MessageBox.Show($"{AppResources.YouBought} {productName}.");
                        });
                    }
                    else
                    {
                        throw new PurchaseException($"{AppResources.UnableToBuy} {productName}.");
                        //rootPage.NotifyUser(productName + " was not purchased.", NotifyType.StatusMessage);
                    }
                }
                catch (Exception)
                {
                    throw new PurchaseException($"{AppResources.UnableToBuy} {productName}.");
                }
            }
            else
            {
                throw new PurchaseException($"{AppResources.YouAlreadyOwn} {productName}.");
            }
        }
    }
}
