using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Threading.Tasks;

namespace LiteTube.DataModel.Database
{
    class LocalDataContext : DataContext, IDatabase
    {
        public LocalDataContext(string fileOrConnection) : base(fileOrConnection)
        {
        }

        public LocalDataContext(string fileOrConnection, MappingSource mapping) : base(fileOrConnection, mapping)
        {
        }

        // Specify a table for the device history items.
        public Table<HistoryItem> HistoryItems;

        // Specify a table for the download item. video on pro version only
        //public Table<DownloadItem> Categories;
        public void AddHistoryVideo(string videoId)
        {
            var item = new HistoryItem
            {
                DateView = DateTime.Now,
                VideoId = videoId
            };

            HistoryItems.InsertOnSubmit(item);
        }

        public Task<IEnumerable<IHistoryVideo>> GetHistoryVideos()
        {
            return Task.Run(() =>
            {
                var items = HistoryItems.OfType<IHistoryVideo>().Select(i => i);
                return items.AsEnumerable();
            });
        }
    }

    [Table]
    public class HistoryItem : INotifyPropertyChanged, INotifyPropertyChanging, IHistoryVideo
    {
        // Define ID: private field, public property, and database column.
        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value) 
                    return;
                
                NotifyPropertyChanging("Id");
                _id = value;
                NotifyPropertyChanged("Id");
            }
        }

        // Define item video id: private field, public property, and database column.
        private string _videoId;

        [Column]
        public string VideoId
        {
            get { return _videoId; }
            set
            {
                if (_videoId != value)
                {
                    NotifyPropertyChanging("VideoId");
                    _videoId = value;
                    NotifyPropertyChanged("VideoId");
                }
            }
        }

        // Define item date view: private field, public property, and database column.
        private DateTime _dateView;

        [Column]
        public DateTime DateView
        {
            get { return _dateView; }
            set
            {
                if (_dateView != value)
                {
                    NotifyPropertyChanging("DateView");
                    _dateView = value;
                    NotifyPropertyChanged("DateView");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
