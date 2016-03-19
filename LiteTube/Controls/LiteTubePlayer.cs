using Microsoft.PlayerFramework;
using System.Windows;

namespace LiteTube.Controls
{
    public class LiteTubePlayer : MediaPlayer
    {
        private static readonly DependencyProperty VideoTitleProperty = DependencyProperty.Register("VideoTitle", typeof(string), typeof(LiteTubePlayer), null);

        public object VideoTitle
        {
            get { return GetValue(VideoTitleProperty); }
            set { SetValue(VideoTitleProperty, value); }
        }
    }
}
