using System.Windows.Controls;
using System.Windows.Navigation;
using LiteTube.Core.ViewModels.Nodes;
#if SILVERLIGHT

#else
using Windows.UI.Xaml.Controls;
#endif
namespace LiteTube.Core.Common
{
    public class NavigationObject
    {
        public NavigationObject(NodeViewModelBase nodeViewModel, Page page)
        {
            ViewModel = nodeViewModel;
#if SILVERLIGHT
            NavigationService = page.NavigationService;
#endif
        }

#if SILVERLIGHT
        public NavigationService NavigationService
        {
            get; private set;
        }
#endif
        public NodeViewModelBase ViewModel
        {
            get; private set;
        }
    }
}
