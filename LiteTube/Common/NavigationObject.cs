using LiteTube.ViewModels.Nodes;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace LiteTube.Common
{
    public class NavigationObject
    {
        public NavigationObject(NodeViewModelBase nodeViewModel, Page page)
        {
            ViewModel = nodeViewModel;
            NavigationService = page.NavigationService;
        }

        public NavigationService NavigationService
        {
            get; private set;
        }

        public NodeViewModelBase ViewModel
        {
            get; private set;
        }
    }
}
