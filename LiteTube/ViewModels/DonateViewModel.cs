using LiteTube.Common;
using MyToolkit.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LiteTube.ViewModels
{
    class DonateViewModel : PropertyChangedBase
    {
        private readonly RelayCommand<FrameworkElement> _donate1Command;
        private readonly RelayCommand<FrameworkElement> _donate2Command;
        private readonly RelayCommand<FrameworkElement> _donate3Command;
        private readonly Purchase _purchase;

        private const string PRODUCT_ID_SMALL = "donate1";
        private const string PRODUCT_ID_MEDIUM = "donateMedium";
        private const string PRODUCT_ID_LARGE = "donateLarge";

        public DonateViewModel()
        {
            _donate1Command = new RelayCommand<FrameworkElement>(Donate1);
            _donate2Command = new RelayCommand<FrameworkElement>(Donate2);
            _donate3Command = new RelayCommand<FrameworkElement>(Donate3);

            //TODO Вынести как зависимость
            _purchase = new Purchase();
        }

        public async void Init()
        {
            await _purchase.Init();
            Small = GetProductDisplayName(_purchase, PRODUCT_ID_SMALL);
            NotifyOfPropertyChanged(() => Small);
            Medium = GetProductDisplayName(_purchase, PRODUCT_ID_MEDIUM);
            NotifyOfPropertyChanged(() => Medium);
            Large = GetProductDisplayName(_purchase, PRODUCT_ID_LARGE);
            NotifyOfPropertyChanged(() => Large);
        }

        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }

        public ICommand Donate1Command
        {
            get { return _donate1Command; }
        }

        public ICommand Donate2Command
        {
            get { return _donate2Command; }
        }

        public ICommand Donate3Command
        {
            get { return _donate3Command; }
        }

        private void Donate1(FrameworkElement obj)
        {
            Donate(PRODUCT_ID_SMALL, Small);
        }

        private void Donate2(FrameworkElement obj)
        {
            Donate(PRODUCT_ID_MEDIUM, Medium);
        }

        private void Donate3(FrameworkElement obj)
        {
            Donate(PRODUCT_ID_LARGE, Large);
        }

        private async void Donate(string productId, string productName)
        {
            await _purchase.BuyProductAsync(productId, productName);
        }

        private string GetProductDisplayName(Purchase purchase, string productId)
        {
            var product = purchase.GetProductInfo(productId);
            if (product == null)
                return string.Empty;

            return $"{product.Name} ({product.FormattedPrice})";
        }
    }
}
