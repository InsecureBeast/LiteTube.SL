using System.Reflection;
using System.Windows;
using LiteTube.Core.Resources;
using Microsoft.Phone.Controls;

namespace LiteTube.Core
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