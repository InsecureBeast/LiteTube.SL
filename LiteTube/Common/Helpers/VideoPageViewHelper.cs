using LiteTube.Controls;
using LiteTube.ViewModels;
using Microsoft.Phone.Controls;
using System.Windows;
using Windows.Devices.Sensors;

namespace LiteTube.Common.Helpers
{
    public static class VideoPageViewHelper
    {
        public static void SensorOrientationChanged(PhoneApplicationPage page, SimpleOrientationSensorOrientationChangedEventArgs args)
        {
            LayoutHelper.InvokeFromUiThread(() =>
            {
                if (args.Orientation == SimpleOrientation.Rotated90DegreesCounterclockwise)
                {
                    page.SupportedOrientations = SupportedPageOrientation.Landscape;
                    page.Orientation = PageOrientation.LandscapeRight;
                    return;
                }
                if (args.Orientation == SimpleOrientation.Rotated270DegreesCounterclockwise)
                {
                    page.SupportedOrientations = SupportedPageOrientation.Landscape;
                    page.Orientation = PageOrientation.LandscapeLeft;
                    return;
                }

                if (args.Orientation != SimpleOrientation.NotRotated)
                    return;

                page.SupportedOrientations = SupportedPageOrientation.Portrait;
                page.Orientation = PageOrientation.Portrait;
            });
        }

        public static void PlayerIsFullScreenChanged(PhoneApplicationPage page, LiteTubePlayer player)
        {
            //_isFullScreen = player.IsFullScreen;
            if (player.IsFullScreen)
            {
                page.SupportedOrientations = SupportedPageOrientation.Landscape;
                page.Orientation = PageOrientation.LandscapeLeft;
                return;
            }

            page.SupportedOrientations = SupportedPageOrientation.Portrait;
            page.Orientation = PageOrientation.Portrait;
        }

        public static void SendComment(VideoPageViewModel viewModel)
        {
            if (viewModel == null)
                return;

            viewModel.CommentsViewModel.AddCommentCommand.Execute(null);
        }

        public static void AddToFavorites(VideoPageViewModel viewModel)
        {
            if (viewModel == null)
                return;

            viewModel.AddFavoritesCommand.Execute(null);
        }

        public static void CopyVideoUrl(VideoPageViewModel viewModel)
        {
            if (viewModel == null)
                return;

            Clipboard.SetText(string.Format("https://www.youtube.com/watch?v={0}", viewModel.VideoId));
        }

        public static bool IsLandscapeOrientation(PageOrientation orientation)
        {
            return orientation == PageOrientation.LandscapeLeft ||
                   orientation == PageOrientation.LandscapeRight ||
                   orientation == PageOrientation.Landscape;
        }

        public static void AddToPlaylist(VideoPageViewModel viewModel)
        {
            if (viewModel == null)
                return;

            viewModel.ShowContainer(viewModel.PlaylistListViewModel.IsContainerShown, viewModel.VideoId);
        }
    }
}
