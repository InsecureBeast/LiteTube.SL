using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Sensors;
using LiteTube.DataClasses;
using LiteTube.ViewModels;
using System.Diagnostics;
using LiteTube.Common;
using Windows.Graphics.Display;
using System.Threading.Tasks;

namespace LiteTube
{
    public partial class VideoPage : PhoneApplicationPage
    {
        private SimpleOrientationSensor _sensor;
        private double _normalHeight;
        private double _normalWidth;

        public VideoPage()
        {
            InitializeComponent();

            _sensor = SimpleOrientationSensor.GetDefault();
            SupportedOrientations = SupportedPageOrientation.Portrait | SupportedPageOrientation.Landscape;

            Pivot.SelectionChanged += PivotOnSelectionChanged;
            Loaded += OnLoaded;
            player.IsFullScreenChanged += PlayerIsFullScreenChanged;
            player.MediaFailed += Player_MediaFailed;
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext != null)
                return;

            var model = PhoneApplicationService.Current.State["model"];
            DataContext = model;
            PhoneApplicationService.Current.State["model"] = null;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var minSize = ActualHeight > ActualWidth ? ActualHeight : ActualWidth;
            var maxSize = ActualHeight > ActualWidth ? ActualWidth : ActualHeight;
            _normalHeight = minSize;
            _normalWidth = maxSize;
            SetPlayerNormalState();

            _sensor.OrientationChanged += SensorOrientationChanged;
        }

        private void SensorOrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            OnOrientationChanged(null);
        }

        private void OnCommentButtonClick(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            if (button == null)
                return;

            var comment = button.DataContext as IComment;
            if (comment == null)
                return;

            var viewModel = DataContext as VideoPageViewModel;
            if (viewModel == null)
                return;

            //Frame.Navigate(typeof(ChannelPage), new ChannelPageViewModel(comment.AuthorChannelId, viewModel.DataSource));
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

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            LayoutHelper.InvokeFromUIThread(async () =>
            {
                var o = _sensor.GetCurrentOrientation();
                if (o.Equals(SimpleOrientation.Rotated90DegreesCounterclockwise))
                {
                    await GoToLanscapeState(PageOrientation.Landscape);
                    return;
                }

                if (o.Equals(SimpleOrientation.Rotated270DegreesCounterclockwise))
                {
                    await GoToLanscapeState(PageOrientation.LandscapeLeft);
                    return;
                }

                if (o.Equals(SimpleOrientation.NotRotated))
                {
                    await GoToPortraitState();
                }
            });
        }

        private void SetPlayerFullScreenState()
        {
            player.IsFullScreen = true;

            player.Width = _normalHeight;
            player.Height = _normalWidth;
            playerCanvas.Width = player.Width;
            playerCanvas.Height = player.Height;
            PlayerMover.Y = 0;
            return;
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

        private void SetVisibilityControls(Visibility visibility)
        {
            //CommandBar.Visibility = visibility;
            //NavPanel.Visibility = visibility;
            Pivot.Visibility = visibility;
        }

        private async Task SetFullScreenState()
        {
            //Меняем размеры плеера
            SetPlayerFullScreenState();
            //Спрячем остальные контролы
            SetVisibilityControls(Visibility.Collapsed);
            //Уберем кнопки навигации
            //ApplicationView.GetForCurrentView().SuppressSystemOverlays = true;
            //Спрячем статус бар
            SystemTray.IsVisible = false;
        }

        private async Task SetNormalState()
        {
            //Меняем размеры плеера
            SetPlayerNormalState();
            //Покажем остальные контролы
            SetVisibilityControls(Visibility.Visible);
            //Покажем кнопки навигации
            //ApplicationView.GetForCurrentView().SuppressSystemOverlays = false;
            //Покажем статус бар
            SystemTray.IsVisible = true;
        }

        private async void PlayerIsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var o = _sensor.GetCurrentOrientation();
            if (e.NewValue)
            {
                if (o.Equals(SimpleOrientation.Rotated90DegreesCounterclockwise))
                {
                    await GoToLanscapeState(PageOrientation.Landscape);
                    return;
                }

                if (o.Equals(SimpleOrientation.Rotated270DegreesCounterclockwise))
                {
                    await GoToLanscapeState(PageOrientation.LandscapeLeft);
                    return;
                }

                await GoToLanscapeState(PageOrientation.Landscape);
                return;
            }

            await GoToPortraitState();
        }

        private async Task GoToLanscapeState(PageOrientation displayOrientation)
        {
            Orientation = displayOrientation;
            await SetFullScreenState();
        }

        private async Task GoToPortraitState()
        {
            Orientation = PageOrientation.Portrait;
            await SetNormalState();
        }
    }
}