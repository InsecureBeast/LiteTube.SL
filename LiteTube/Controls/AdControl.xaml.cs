using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AdMediator.Core.Events;

namespace LiteTube.Controls
{
    public partial class AdControl : UserControl
    {
        public AdControl()
        {
            InitializeComponent();
            //AdMediator_F3EEB8.AdSdkOptionalParameters["Smaato"]["Margin"] = new Thickness(0, -20, 0, 0);
            //AdMediator_F3EEB8.AdSdkOptionalParameters["Smaato"]["Width"] = 320d;
            //AdMediator_F3EEB8.AdSdkOptionalParameters["Smaato"]["Height"] = 50d;

            //var p = AdMediator_F3EEB8.AdSdkOptionalParameters;
            AdMediator_F3EEB8.AdMediatorError += AdMediator_F3EEB8_AdMediatorError;
            AdMediator_F3EEB8.AdMediatorFilled += AdMediator_F3EEB8_AdMediatorFilled;
            AdMediator_F3EEB8.AdSdkError += AdMediator_F3EEB8_AdSdkError;
        }

        private void AdMediator_F3EEB8_AdSdkError(object sender, AdFailedEventArgs e)
        {
        }

        private void AdMediator_F3EEB8_AdMediatorFilled(object sender, AdSdkEventArgs e)
        {
        }

        private void AdMediator_F3EEB8_AdMediatorError(object sender, AdMediatorFailedEventArgs e)
        {
        }
    }
}
