using LiteTube.Common;
using MyToolkit.Command;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly RelayCommand<FrameworkElement> _donate4Command;

        public DonateViewModel()
        {
            _donate1Command = new RelayCommand<FrameworkElement>(Donate1);
            _donate2Command = new RelayCommand<FrameworkElement>(Donate2);
            _donate3Command = new RelayCommand<FrameworkElement>(Donate3);
            _donate4Command = new RelayCommand<FrameworkElement>(Donate4);
        }

        public async void Init()
        {
            var purchase = new Purchase();
            await purchase.Init();
            var test = purchase.GetProductInfo("donate1");
            if (test == null)
                return;

            Test = test.Name + test.FormattedPrice;
            NotifyOfPropertyChanged(() => Test);
        }

        public string Test
        {
            get;set;
        }

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

        public ICommand Donate4Command
        {
            get { return _donate4Command; }
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

        private void Donate4(FrameworkElement obj)
        {
            Donate(240);
        }

        private void Donate(int sum)
        {

        }
    }
}
