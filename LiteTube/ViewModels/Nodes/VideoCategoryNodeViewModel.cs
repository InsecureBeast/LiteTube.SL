﻿using LiteTube.DataClasses;

namespace LiteTube.ViewModels.Nodes
{
    public class VideoCategoryNodeViewModel
    {
        private readonly string _title;
        
        public VideoCategoryNodeViewModel(IVideoCategory category)
        {
            _title = category.Title;
            CategoryId = category.Id;
            ChannelId = category.ChannelId;
        }

        public string Title
        {
            get { return _title; }
        }

        public string CategoryId
        {
            get;
            private set;
        }

        public string ChannelId
        {
            get;
            private set;
        }

        public string Image
        {
            get { return null; }
        }
    }
}
