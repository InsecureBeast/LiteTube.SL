using LiteTube.DataClasses;
using System.Linq;
using System.Collections.Generic;

namespace LiteTube.DataModel
{
    class ResponceListCache<T> where T : IResponceList
    {
        private Dictionary<string, T> _cache;

        public ResponceListCache()
        {
            _cache = new Dictionary<string, T>();
        }

        public void SetItem(T item)
        {
            if (string.IsNullOrEmpty(item.PrevPageToken))
            {
                _cache[string.Empty] = item;
                return;
            }

            _cache[item.PrevPageToken] = item;
        }

        public T GetItem(string nextPageToken)
        {
            var pageToken = string.Empty;

            if (nextPageToken == null)
                return _cache.Values.Last();

            T item;
            if (_cache.TryGetValue(nextPageToken, out item))
            {
                return item;
            }

            return default(T);
        }

        public IEnumerable<T> GetItems(IEnumerable<string> pageTokens)
        {
            var result = new List<T>();
            foreach (var pageToken in pageTokens)
            {
                var item = GetItem(pageToken);
                if (item == null)
                    continue;

                result.Add(item);
            }

            return result;
        }

        internal void Clear()
        {
            _cache.Clear();
        }
    }

    class ActivityListCache
    {
        private Dictionary<string, IVideoList> cache;

        public ActivityListCache()
        {
            cache = new Dictionary<string, IVideoList>();
        }

        public void SetItem(IVideoList item)
        {
            if (string.IsNullOrEmpty(item.PrevPageToken))
            {
                cache[string.Empty] = item;
                return;
            }
                
            cache[item.PrevPageToken] = item;
        }

        public IVideoList GetItem(string nextPageToken)
        {
            IVideoList item;
            if (cache.TryGetValue(nextPageToken, out item))
            {
                return item;
            }

            return null;
        }

        public IEnumerable<IVideoList> GetItems(IEnumerable<string> pageTokens)
        {
            var result = new List<IVideoList>();
            foreach (var pageToken in pageTokens)
            {
                var item = GetItem(pageToken);
                if (item == null)
                    continue;

                result.Add(item);
            }

            return result;
        }
    }
}
