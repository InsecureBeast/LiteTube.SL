using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Common.Helpers;
using LiteTube.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTube
{
    public partial class PlaylistPage : PhoneApplicationPage
    {
        public PlaylistPage()
        {
            InitializeComponent();
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private void Play_Click(object sender, EventArgs e)
        {
            var model = DataContext as PlaylistPageViewModel;
            if (model == null)
                return;

            model.PlayAll();
        }
    }
}