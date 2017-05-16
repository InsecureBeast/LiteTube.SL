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

        public DonateViewModel()
        {
            _donate1Command = new RelayCommand<FrameworkElement>(Donate1);
            _donate2Command = new RelayCommand<FrameworkElement>(Donate2);
            _donate3Command = new RelayCommand<FrameworkElement>(Donate3);
        }

        public async void Init()
        {
            var purchase = new Purchase();
            await purchase.Init();
            Small = GetProductDisplayName(purchase, "donate1");
            NotifyOfPropertyChanged(() => Small);
            Medium = GetProductDisplayName(purchase, "donateMedium");
            NotifyOfPropertyChanged(() => Medium);
            Large = GetProductDisplayName(purchase, "donateLarge");
            NotifyOfPropertyChanged(() => Large);
        }

        private string GetProductDisplayName(Purchase purchase, string productId)
        {
            var product = purchase.GetProductInfo(productId);
            if (product == null)
                return string.Empty;

            return $"{product.Name} ({product.FormattedPrice})";
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
            Donate(60);
        }

        private void Donate2(FrameworkElement obj)
        {
            Donate(120);
        }

        private void Donate3(FrameworkElement obj)
        {
            Donate(180);
        }

        private void Donate(int sum)
        {

        }
    }
}
