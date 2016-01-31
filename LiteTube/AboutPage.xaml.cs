using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.Resources;
using System.Reflection;

namespace LiteTube
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var varsion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            VrsionTblk.Text = string.Format("{0} {1}", AppResources.Version, varsion);
        }
    }
}