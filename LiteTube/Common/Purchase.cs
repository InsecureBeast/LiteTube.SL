using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace LiteTube.Common
{
    class Purchase
    {
        private ListingInformation _listing;

        public async void Init()
        {
            // Initialize the license info for use in the app that is uploaded to the Store.
            // Uncomment the following line in the release version of your app.
            var licenseInformation = CurrentApp.LicenseInformation;

            // Initialize the license info for testing.
            // Comment the following line in the release version of your app.
            //var licenseInformation = CurrentAppSimulator.LicenseInformation;

            _listing = await CurrentApp.LoadListingInformationAsync();
            //_listing = await CurrentApp.LoadListingInformationByProductIdsAsync(new string[] { "donate1"});
            var product1 = _listing.ProductListings.Count;
        }

        public ProductListing GetProductInfo(string name)
        {
            try
            {
                return _listing.ProductListings[name];
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
                //rootPage.NotifyUser("Buying " + productName + "...", NotifyType.StatusMessage);
                try
                {
                    await CurrentApp.RequestProductPurchaseAsync(productId);
                    if (licenseInformation.ProductLicenses[productId].IsActive)
                    {
                        //rootPage.NotifyUser("You bought " + productName + ".", NotifyType.StatusMessage);
                    }
                    else
                    {
                        //rootPage.NotifyUser(productName + " was not purchased.", NotifyType.StatusMessage);
                    }
                }
                catch (Exception)
                {
                    //rootPage.NotifyUser("Unable to buy " + productName + ".", NotifyType.ErrorMessage);
                }
            }
            else
            {
                //rootPage.NotifyUser("You already own " + productName + ".", NotifyType.ErrorMessage);
            }
        }
    }
}
