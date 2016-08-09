using System.Windows.Navigation;
using LiteTube.Core.Common.Helpers;
using Microsoft.Phone.Controls;

namespace LiteTube.Core
{
    public partial class SectionPage : PhoneApplicationPage
    {
        public SectionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedTo(this);
        }
    }
}