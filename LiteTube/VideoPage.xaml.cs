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
    public partial class VideoPage : PhoneApplicationPage
    {
        public VideoPage()
        {
            InitializeComponent();
        }

        private void OnChannelButtonClick(object sender, RoutedEventArgs e)
        {
            //var viewModel = DataContext as VideoPageViewModel;
            //if (viewModel == null)
            //    return;

            //Frame.Navigate(typeof(ChannelPage), new ChannelPageViewModel(viewModel.Channel, viewModel.DataSource));
        }
    }
}