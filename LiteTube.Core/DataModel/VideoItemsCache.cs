using System.Collections.Generic;
using LiteTube.Core.DataClasses;

namespace LiteTube.Core.DataModel
{
    class VideoItemsCache
    {
        private Dictionary<string, IVideoItem> cache;

        public VideoItemsCache()
        {
            cache = new Dictionary<string, IVideoItem>();
        }

        public void SetItem(IVideoItem item)
        {
            cache[item.Details.VideoId] = item;
        }

        public IVideoItem GetItem(string id)
        {
            IVideoItem item;
            if (cache.TryGetValue(id, out item))
            {
                return item;
            }

            return null;
        }

        public IEnumerable<IVideoItem> GetItems(IEnumerable<string> ids)
        {
            var result = new List<IVideoItem>();
            foreach (var id in ids)
            {
                var item = GetItem(id);
                if (item == null)
                    continue;

                result.Add(item);
            }

            return result;
        }
    }
}
