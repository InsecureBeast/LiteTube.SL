using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.ViewModels.Nodes;

namespace LiteTube.Controls
{
    public partial class AdMediatorControl : UserControl
    {
        public AdMediatorControl()
        {
            InitializeComponent();
            Loaded += AdMediatorControl_Loaded;
            AdMediator_Most_popular.AdMediatorError += AdMediator_Most_popular_AdMediatorError;
            AdMediator_Most_popular.AdSdkError += AdMediator_Most_popular_AdSdkError;
            AdMediator_F3EEB8.AdMediatorError += AdMediator_F3EEB8_AdMediatorError;
            adSomaViewer.Pub = 1100025508;
            adSomaViewer.Adspace = 130168472;
            adSomaViewer.Format = SOMAWP8.SomaAd.FormatRequested.img;
            adSomaViewer.AdError += AdSomaViewer_AdError;
            adSomaViewer.StartAds();
        }

        private void AdSomaViewer_AdError(object sender, string ErrorCode, string ErrorDescription)
        {
            System.Diagnostics.Debug.WriteLine(ErrorDescription);
        }

        private void AdMediator_F3EEB8_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Error.Message);
        }

        private void AdMediator_Most_popular_AdSdkError(object sender, Microsoft.AdMediator.Core.Events.AdFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ErrorDescription);
        }

        private void AdMediator_Most_popular_AdMediatorError(object sender, Microsoft.AdMediator.Core.Events.AdMediatorFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Error.Message);
        }

        private void AdMediatorControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdvMostPopularNodeViewModel)
            {
                AdMediator_F3EEB8.Visibility = Visibility.Collapsed;
                AdMediator_Most_popular.Visibility = Visibility.Collapsed;
                adMsControl.Visibility = Visibility.Collapsed;
                adSomaViewer.Visibility = Visibility.Visible;
                return;
            }

            if (DataContext is AdvNodeViewModel)
            {
                AdMediator_F3EEB8.Visibility = Visibility.Visible;
                AdMediator_Most_popular.Visibility = Visibility.Collapsed;
                adMsControl.Visibility = Visibility.Collapsed;
                adSomaViewer.Visibility = Visibility.Collapsed;
                adSomaViewer.StopAds();
            }
        }
    }
}
