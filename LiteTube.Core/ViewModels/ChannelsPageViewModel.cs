﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteTube.Core.Common;
using LiteTube.Core.Common.Helpers;
using LiteTube.Core.DataClasses;
using LiteTube.Core.DataModel;
using LiteTube.Core.ViewModels.Nodes;
#if SILVERLIGHT

#endif


namespace LiteTube.Core.ViewModels
{
    class ChannelListPageViewModel : SectionBaseViewModel
    {
        private readonly string _categoryId;

        public ChannelListPageViewModel(string categoryId, string title, Func<IDataSource> getGeDataSource, IConnectionListener connectionListener)
            : base(getGeDataSource, connectionListener)
        {
            _uniqueId = categoryId;
            _categoryId = categoryId;
            Title = title;
            LayoutHelper.InvokeFromUiThread(async() => await FirstLoad());
        }

        public override string ToString()
        {
            return Title;
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            if (e.IsConnected)
            {
                LayoutHelper.InvokeFromUiThread(async () =>
                {
                    await FirstLoad();
                });
            }
        }

        internal override async Task<IResponceList> GetItems(string nextPageToken)
        {
            return await _getGeDataSource().GetChannels(_categoryId, nextPageToken);
        }

        internal override void LoadItems(IResponceList videoList)
        {
            var channelList = videoList as IChannelList;
            if (channelList == null)
                return;

            AddItems(channelList.Items);
        }

        internal void AddItems(IEnumerable<IChannel> items)
        {
            var itemsList = Items.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.Id))
                    continue;
                Items.Add(new ChannelNodeViewModel(item));
            }
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
    }
}
