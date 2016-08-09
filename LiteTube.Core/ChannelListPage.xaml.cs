using System.Windows.Navigation;
using LiteTube.Core.Common.Helpers;
using Microsoft.Phone.Controls;

namespace LiteTube.Core
{
    public partial class ChannelListPage : PhoneApplicationPage
    {
        public ChannelListPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);
        }
    }
}