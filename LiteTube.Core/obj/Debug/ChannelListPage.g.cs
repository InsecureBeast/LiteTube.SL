﻿#pragma checksum "D:\Programming\LiteTube\LiteTube.Core\ChannelListPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "86EEA163CDF6D5A28BCC0AC299013843"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using LiteTube.Core.Controls;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace LiteTube.Core {
    
    
    public partial class ChannelListPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal LiteTube.Core.Controls.NavigationPanel NavControl;
        
        internal System.Windows.Controls.ContentPresenter ChannelList;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/LiteTube.Core;component/ChannelListPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.NavControl = ((LiteTube.Core.Controls.NavigationPanel)(this.FindName("NavControl")));
            this.ChannelList = ((System.Windows.Controls.ContentPresenter)(this.FindName("ChannelList")));
        }
    }
}

