using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LiteTube.Common.Helpers;
using LiteTube.ViewModels;
using System.Diagnostics;
using Windows.Devices.Sensors;
using LiteTube.Common;
using Microsoft.PlayerFramework;

namespace LiteTube
{
    public partial class VideoPage : PhoneApplicationPage
    {
        private double _normalHeight;
        private double _normalWidth;
        private readonly SimpleOrientationSensor _sensor;

        public VideoPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Pivot.SelectionChanged += PivotOnSelectionChanged;
            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            _sensor = SimpleOrientationSensor.GetDefault();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);
            _sensor.OrientationChanged += Sensor_OrientationChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _sensor.OrientationChanged -= Sensor_OrientationChanged;
            //player.Dispose();
        }

        private void Sensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            LayoutHelper.InvokeFromUIThread(() => 
            {
                if (args.Orientation == SimpleOrientation.Rotated90DegreesCounterclockwise)
                {
                    SupportedOrientations = SupportedPageOrientation.Landscape;
                    Orientation = PageOrientation.LandscapeRight;
                    return;
                }
                if (args.Orientation == SimpleOrientation.Rotated270DegreesCounterclockwise)
                {
                    SupportedOrientations = SupportedPageOrientation.Landscape;
                    Orientation = PageOrientation.LandscapeLeft;
                    return;
                }

                if (args.Orientation != SimpleOrientation.NotRotated) 
                    return;
                
                SupportedOrientations = SupportedPageOrientation.Portrait;
                Orientation = PageOrientation.Portrait;
            });
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            if (e.Orientation == PageOrientation.LandscapeLeft ||
                e.Orientation == PageOrientation.LandscapeRight ||
                e.Orientation == PageOrientation.Landscape)
            {
                SetPlayerFullScreenState();
                SetVisibilityControls(Visibility.Collapsed);
                return;
            }

            SetPlayerNormalState();
            SetVisibilityControls(Visibility.Visible);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var actualWidth = App.Current.Host.Content.ActualWidth;
            var actualHeight = App.Current.Host.Content.ActualHeight;
            var minSize = actualHeight > actualWidth ? actualHeight : actualWidth;
            var maxSize = actualHeight > actualWidth ? actualWidth : actualHeight;
            _normalHeight = minSize;
            _normalWidth = maxSize;

            SetPlayerNormalState();
        }

        private async void PivotOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;
            if (pivot == null)
                return;

            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            switch (pivot.SelectedIndex)
            {
                case 0:
                    Debug.WriteLine("details");
                    break;

                case 1:
                    Debug.WriteLine("related");
                    if (!viewModel.RelatedVideosViewModel.IsEmpty)
                        await viewModel.RelatedVideosViewModel.FirstLoad();
                    break;

                case 2:
                    Debug.WriteLine("comments");
                    if (!viewModel.CommentsViewModel.IsEmpty)
                        await viewModel.CommentsViewModel.FirstLoad();
                    break;
            }
        }

        private void Home_Click(object sender, EventArgs e)
        {
            NavigationHelper.GoHome();
        }

        private void SetPlayerNormalState()
        {
            player.IsFullScreen = false;
            player.Width = _normalWidth;
            player.Height = _normalWidth / 1.778;
            playerCanvas.Width = player.Width;
            playerCanvas.Height = player.Height;
            PlayerMover.Y = 0;
        }

        private void SetPlayerFullScreenState()
        {
            player.IsFullScreen = true;
            player.Width = _normalHeight;
            player.Height = _normalWidth;
            playerCanvas.Width = player.Width;
            playerCanvas.Height = player.Height;
            PlayerMover.Y = 0;
        }

        private void SetVisibilityControls(Visibility visibility)
        {
            ApplicationBar.IsVisible = visibility == Visibility.Visible;
            //NavPanel.Visibility = visibility;
            Pivot.Visibility = visibility;
            SystemTray.IsVisible = visibility == Visibility.Visible;
        }

        private void PlayerIsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (player.IsFullScreen)
            {
                SupportedOrientations = SupportedPageOrientation.Landscape; 
                Orientation = PageOrientation.LandscapeLeft;
                return;
            }

            SupportedOrientations = SupportedPageOrientation.Portrait;
            Orientation = PageOrientation.Portrait;
        }

        private void Find_Click(object sender, EventArgs e)
        {
            var datasource = App.ViewModel.DataSource;
            NavigationHelper.Navigate("/SearchPage.xaml", new SearchPageViewModel(datasource));
        }
    }
}