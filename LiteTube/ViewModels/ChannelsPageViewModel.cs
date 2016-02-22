﻿using System;
using LiteTube.DataClasses;
using LiteTube.DataModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LiteTube.ViewModels.Nodes;
using LiteTube.Common;
using Microsoft.Phone.Shell;
using LiteTube.Common.Helpers;

namespace LiteTube.ViewModels
{
    class ChannelListPageViewModel : SectionBaseViewModel
    {
        private readonly string _categoryId;
        private readonly ObservableCollection<ChannelNodeViewModel> _channels;

        public ChannelListPageViewModel(string categoryId, string title, Func<IDataSource> getGeDataSource, IConnectionListener connectionListener)
            : base(getGeDataSource, connectionListener)
        {
            _uniqueId = categoryId;
            _categoryId = categoryId;
            Title = title;
            _channels = new ObservableCollection<ChannelNodeViewModel>();
            LayoutHelper.InvokeFromUIThread(async() => await FirstLoad());
        }

        public override string ToString()
        {
            return Title;
        }

        public ObservableCollection<ChannelNodeViewModel> CategoryItems
        {
            get { return _channels; }
        }

        public override void Notify(ConnectionEventArgs e)
        {
            base.Notify(e);
            if (e.IsConnected)
            {
                LayoutHelper.InvokeFromUIThread(async () =>
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
            var itemsList = _channels.ToList();
            foreach (var item in items)
            {
                if (itemsList.Exists(i => i.Id == item.Id))
                    continue;
                _channels.Add(new ChannelNodeViewModel(item));
            }

            IsLoading = false;
            if (!_channels.Any())
                IsEmpty = true;

            HideProgressIndicator();
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

            NavigationHelper.Navigate("/ChannelPage.xaml", new ChannelPageViewModel(channel.Id, _getGeDataSource, _connectionListener));
        }
    }
}
