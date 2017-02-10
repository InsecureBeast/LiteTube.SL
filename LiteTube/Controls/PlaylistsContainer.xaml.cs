using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;

namespace LiteTube.Controls
{
    public partial class PlaylistsContainer : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = 
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(PlaylistsContainer), new PropertyMetadata(false, IsOpenChanged));

        public PlaylistsContainer()
        {
            InitializeComponent();
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private static void IsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PlaylistsContainer;
            if (control == null)
                return;

            var isOpen = e.NewValue as bool?;
            if (isOpen == null)
                return;

            if (isOpen == true)
                control.LayoutRoot.Visibility = Visibility.Visible;
            if (isOpen == false)
                control.LayoutRoot.Visibility = Visibility.Collapsed;
        }
    }
}
