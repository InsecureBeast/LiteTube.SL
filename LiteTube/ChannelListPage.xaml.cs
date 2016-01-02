using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTube
{
    public partial class ChannelListPage : PhoneApplicationPage
    {
        public ChannelListPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext != null)
                return;

            var model = PhoneApplicationService.Current.State["model"];
            DataContext = model;
            PhoneApplicationService.Current.State["model"] = null;
        }
    }
}