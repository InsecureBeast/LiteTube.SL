using System;
using System.Collections.Generic;
using System.Linq;
using LiteTube.Core.Common;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;
using LiteTube.Core.ViewModels.Nodes;

namespace LiteTube.Core.ViewModels.Search
{
    public class SearchChannelsViewModel : SearchBaseViewModel
    {
        public SearchChannelsViewModel(Func<IDataSource> geDataSource, IConnectionListener connectionListener, Action<bool> changeProgressIndicator)
            : base(SearchType.Channel, geDataSource, connectionListener, changeProgressIndicator)
        {
        }

        internal override void LoadItems(IResponceList responceList)
        {
            var list = responceList as IChannelList;
            if (list == null)
                return;

            AddItems(list.Items);
        }

        internal override void NavigateTo(NavigationObject navObject)
        {
            if (navObject == null)
                return;

            if (navObject.ViewModel == null)
                return;

            var viewModel = navObject.ViewModel as ChannelNodeViewModel;
            if (viewModel == null)
                return;

            var channel = viewModel.Channel;
            if (channel == null)
                return;

#if SILVERLIGHT
            NavigationHelper.GoToChannelPage(channel.Id, null);
#endif
        }

        private void AddItems(IEnumerable<IChannel> items)
        {
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.Id))
                    continue;
                Items.Add(new ChannelNodeViewModel(item));
            }
        }
    }
}
