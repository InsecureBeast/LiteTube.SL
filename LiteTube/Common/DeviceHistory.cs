using System;
using System.Collections.Generic;

namespace LiteTube.Common
{
    public interface IDeviceHistory
    {
        void Add(string videoId);
        void Remove(string videoId);
        IEnumerable<string> GetHistory();
    }

    class DeviceHistory : IDeviceHistory
    {
        private Dictionary<string, DateTime> _history;
        public DeviceHistory()
        {
            _history = SettingsHelper.LoadHistory();
        }

        public void Add(string videoId)
        {
            _history[videoId] = DateTime.Now;
            SettingsHelper.SaveHistory(_history);
        }

        public IEnumerable<string> GetHistory()
        {
            return _history.Keys;
        }

        public void Remove(string videoId)
        {
            _history.Remove(videoId);
        }
    }
}
