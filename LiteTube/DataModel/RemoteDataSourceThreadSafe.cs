using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using LiteTube.DataClasses;
using MyToolkit.Multimedia;

namespace LiteTube.DataModel
{
    class RemoteDataSourceThreadSafe : IRemoteDataSource
    {
        private readonly IRemoteDataSource _remote;
        private readonly Queue<Action> _actions = new Queue<Action>();
        private readonly Thread _thread;

        public RemoteDataSourceThreadSafe(IRemoteDataSource remote)
        {
            _remote = remote;
            _thread = new Thread(Processing) { IsBackground = true, Name = GetType().Name };
            _thread.Start();
        }

        private Action Dequeue()
        {
            lock (_actions)
            {
                while (_actions.Count == 0)
                    Monitor.Wait(_actions);
                return _actions.Peek();
            }
        }

        private void Enqueue(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
                Monitor.Pulse(_actions);
            }
        }

        private void Processing()
        {
            Action action = Dequeue();
            while (action != null)
            {
                action();
                Remove(action);
                action = Dequeue();
            }
        }

        private void Remove(Action action)
        {
            lock (_actions)
            {
                if (_actions.Count == 0)
                    return;
                if (_actions.Peek() == action)
                    _actions.Dequeue();
            }
        }

        public void Stop(bool cancel, bool wait)
        {
            lock (_actions)
            {
                if (cancel)
                    _actions.Clear();
                _actions.Enqueue(null);
                Monitor.Pulse(_actions);
            }
            if (wait)
                _thread.Join();
        }

        public void Login()
        {
            Enqueue(()=> _remote.Login());
        }

        public Task<string> ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args, string username)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            Task result= null;
            Enqueue(() => { result = _remote.Logout(); });
            return result;
        }

        public Task LoginSilently(string username)
        {
            Task result = null;
            Enqueue(() => { result = _remote.LoginSilently(username); });
            return result;
        }

        public bool IsAuthorized
        {
            get { return _remote.IsAuthorized; }
        }
        public Task<IEnumerable<IVideoCategory>> GetCategories(string culture)
        {
            Task<IEnumerable<IVideoCategory>> result = null;
            Enqueue(() => { result = _remote.GetCategories(culture); });
            return result;
        }

        public Task<IVideoList> GetActivity(string culture, int maxResult, string pageToken)
        {
            Task<IVideoList> result = null;
            Enqueue(() => { result = _remote.GetActivity(culture, maxResult, pageToken); });
            return result;
        }

        public Task<IVideoList> GetRecommended(string pageToken)
        {
            Task<IVideoList> result = null;
            Enqueue(() => { result = _remote.GetRecommended(pageToken); });
            return result;
        }

        public Task<IVideoList> GetMostPopular(string culture, int maxResult, string pageToken)
        {
            Task<IVideoList> result = null;
            Enqueue(() => { result = _remote.GetMostPopular(culture, maxResult, pageToken); });
            return result;
        }

        public Task<IChannel> GetChannel(string channelId)
        {
            Task<IChannel> result = null;
            Enqueue(() => { result = _remote.GetChannel(channelId); });
            return result;
        }

        public Task<IVideoList> GetRelatedVideos(string videoId, int maxResult, string pageToken)
        {
            Task<IVideoList> result = null;
            Enqueue(() => { result = _remote.GetRelatedVideos(videoId, maxResult, pageToken); });
            return result;
        }

        public Task<IVideoList> GetCategoryVideoList(string categoryId, string culture, int maxResult, string pageToken)
        {
            throw new NotImplementedException();
        }

        public Task<IVideoList> GetChannelVideoList(string channelId, string culture, int maxPageResult, string pageToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IGuideCategory>> GetGuideCategories(string culture)
        {
            throw new NotImplementedException();
        }

        public Task<IChannelList> GetChannels(string categoryId, string culture, int maxPageResult, string nextPageToken)
        {
            throw new NotImplementedException();
        }

        public Task<IVideoList> Search(string searchString, int maxResult, string nextPageToken)
        {
            throw new NotImplementedException();
        }

        public Task<ICommentList> GetComments(string videoId, int maxResult, string nextPageToken)
        {
            throw new NotImplementedException();
        }

        public Task<ISubscriptionList> GetSubscribtions(int maxResult, string nextPageToken)
        {
            throw new NotImplementedException();
        }

        public Task<IVideoList> GetHistory(int maxResult, string nextPageToken)
        {
            throw new NotImplementedException();
        }

        public bool IsSubscribed(string channelId)
        {
            throw new NotImplementedException();
        }

        public string GetSubscriptionId(string channelId)
        {
            throw new NotImplementedException();
        }

        public Task Subscribe(string channelId)
        {
            throw new NotImplementedException();
        }

        public Task Unsubscribe(string subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task SetRating(string videoId, RatingEnum rating)
        {
            throw new NotImplementedException();
        }

        public Task<RatingEnum> GetRating(string videoId)
        {
            throw new NotImplementedException();
        }

        public Task<YouTubeUri> GetVideoUriAsync(string videoId, YouTubeQuality quality)
        {
            throw new NotImplementedException();
        }

        public Task AddToFavorites(string videoId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromFavorites(string playlistItemId)
        {
            throw new NotImplementedException();
        }

        public Task<IResponceList> GetFavorites(int maxResult, string nextPageToken)
        {
            throw new NotImplementedException();
        }

        public Task<IVideoItem> GetVideoItem(string videoId)
        {
            throw new NotImplementedException();
        }

        public Task<IProfile> GetProfile()
        {
            throw new NotImplementedException();
        }

        public Task<IComment> AddComment(string channelId, string videoId, string text)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetAutoCompleteSearchItems(string query)
        {
            throw new NotImplementedException();
        }
    }
}
