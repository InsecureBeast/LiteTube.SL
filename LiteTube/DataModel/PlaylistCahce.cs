﻿using LiteTube.Common;
using LiteTube.DataClasses;
using System.Collections.Generic;

namespace LiteTube.DataModel
{
    class PlaylistCahce
    {
        private Dictionary<string, IPlaylistItemList> _cahce;

        public PlaylistCahce()
        {
            _cahce = new Dictionary<string, IPlaylistItemList>();
        }

        public IPlaylistItemList GetPlaylistItemList (string pageToken)
        {
            if (pageToken == null)
                pageToken = string.Empty;

            IPlaylistItemList result;
            if (!_cahce.TryGetValue(pageToken, out result))
                return null;
            return result;
        }

        public void AddPlaylistItemList(string pageToken, IPlaylistItemList list)
        {
            if (pageToken == null)
                pageToken = string.Empty;

            IPlaylistItemList result;
            if (!_cahce.TryGetValue(pageToken, out result))
                _cahce.Add(pageToken, list);
        }
    }
}
