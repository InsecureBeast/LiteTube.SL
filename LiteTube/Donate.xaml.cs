using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using LiteTube.Common.Helpers;
using LiteTube.ViewModels;

namespace LiteTube
{
    public partial class Donate : PhoneApplicationPage
    {
        public Donate()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);
            var model = DataContext as DonateViewModel;
            if (model == null)
                return;

            model.Init();
        }
    }
}