using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteTube.ViewModels.Nodes
{
    static class NodeViewModelExtensions
    {
        public static bool IsLive(this NodeViewModelBase viewModel)
        {
            var vm = viewModel as VideoItemViewModel;
            var isLive = false;
            if (vm != null)
                isLive = vm.IsLive;

            return isLive;
        }
    }
}
