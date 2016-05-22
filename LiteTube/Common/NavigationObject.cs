using LiteTube.ViewModels.Nodes;
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Navigation;
#else
using Windows.UI.Xaml.Controls;
#endif
namespace LiteTube.Common
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
