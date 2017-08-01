using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LiteTube.StreamVideo.Metadata
{
  public class MetadataSink : IMetadataSink
  {
    private readonly object _lock = new object();
    private readonly MetadataState _metadataState = new MetadataState();
    private readonly Queue<ITrackMetadata> _pendingTracks = new Queue<ITrackMetadata>();
    private TimeSpan _position;

    public virtual void ReportStreamMetadata(TimeSpan timestamp, IStreamMetadata streamMetadata)
    {
      Debug.WriteLine(string.Concat(new object[4]
      {
        (object) "MetadataSink.ReportStreamMetadata() ",
        (object) timestamp,
        (object) " ",
        (object) streamMetadata
      }));
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._metadataState.StreamTimestamp = timestamp;
        this._metadataState.StreamMetadata = streamMetadata;
        this._metadataState.SegmentMetadata = (ISegmentMetadata) null;
        this._metadataState.TrackMetadata = (ITrackMetadata) null;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    public virtual void ReportSegmentMetadata(TimeSpan timestamp, ISegmentMetadata segmentMetadata)
    {
      Debug.WriteLine(string.Concat(new object[4]
      {
        (object) "MetadataSink.ReportSegmentMetadata() ",
        (object) timestamp,
        (object) " ",
        (object) segmentMetadata
      }));
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._metadataState.SegmentTimestamp = timestamp;
        this._metadataState.SegmentMetadata = segmentMetadata;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    public virtual void ReportTrackMetadata(ITrackMetadata trackMetadata)
    {
      if (trackMetadata == null)
        throw new ArgumentNullException("trackMetadata");
      Debug.WriteLine("MetadataSink.ReportTrackMetadata() " + (object) trackMetadata);
      TimeSpan? timeStamp = trackMetadata.TimeStamp;
      if (!timeStamp.HasValue)
        throw new ArgumentException("A timestamp is required", "trackMetadata");
      timeStamp = trackMetadata.TimeStamp;
      if (timeStamp.Value < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException("trackMetadata", "The timestamp cannot be negative");
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._pendingTracks.Enqueue(trackMetadata);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    public virtual void ReportConfigurationMetadata(IConfigurationMetadata configurationMetadata)
    {
      Debug.WriteLine("MetadataSink.ReportMediaStreamMetadata() " + (object) configurationMetadata);
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._metadataState.ConfigurationMetadata = configurationMetadata;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    private TimeSpan? ProcessPendingTracks()
    {
      TimeSpan? nullable;
      while (this._pendingTracks.Count > 0)
      {
        if (null == this._metadataState.TrackMetadata)
        {
          ITrackMetadata trackMetadata = this._pendingTracks.Dequeue();
          this._metadataState.TrackMetadata = trackMetadata;
          nullable = trackMetadata.TimeStamp;
          Debug.Assert(nullable.HasValue, "Invalid track metadata (no timestamp)");
        }
        else
        {
          TimeSpan? timeStamp = this._metadataState.TrackMetadata.TimeStamp;
          nullable = timeStamp;
          TimeSpan timeSpan1 = this._position;
          if ((nullable.HasValue ? (nullable.GetValueOrDefault() > timeSpan1 ? 1 : 0) : 0) != 0)
            return timeStamp;
          ITrackMetadata trackMetadata1 = this._pendingTracks.Peek();
          nullable = trackMetadata1.TimeStamp;
          Debug.Assert(nullable.HasValue, "Invalid track metadata (no timestamp)");
          nullable = trackMetadata1.TimeStamp;
          TimeSpan timeSpan2 = this._position;
          if ((nullable.HasValue ? (nullable.GetValueOrDefault() > timeSpan2 ? 1 : 0) : 0) != 0)
            return trackMetadata1.TimeStamp;
          ITrackMetadata trackMetadata2 = this._pendingTracks.Dequeue();
          Debug.Assert(object.ReferenceEquals((object) trackMetadata1, (object) trackMetadata2), "Dequeue track mismatch");
          this._metadataState.TrackMetadata = trackMetadata1;
        }
      }
      if (null != this._metadataState.TrackMetadata)
      {
        TimeSpan? timeStamp = this._metadataState.TrackMetadata.TimeStamp;
        Debug.Assert(timeStamp.HasValue, "Invalid track metadata (no timestamp)");
        nullable = timeStamp;
        TimeSpan timeSpan = this._position;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() > timeSpan ? 1 : 0) : 0) != 0)
          return timeStamp;
      }
      nullable = new TimeSpan?();
      return nullable;
    }

    public virtual void Reset()
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._position = TimeSpan.Zero;
        this._metadataState.StreamMetadata = (IStreamMetadata) null;
        this._metadataState.StreamTimestamp = TimeSpan.Zero;
        this._metadataState.SegmentMetadata = (ISegmentMetadata) null;
        this._metadataState.SegmentTimestamp = TimeSpan.Zero;
        this._metadataState.TrackMetadata = (ITrackMetadata) null;
        this._pendingTracks.Clear();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }

    public virtual TimeSpan? Update(MetadataState state, TimeSpan position)
    {
      bool lockTaken = false;
      object obj = null;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        this._position = position;
        TimeSpan? nullable1 = this.ProcessPendingTracks();
        IStreamMetadata streamMetadata = this._metadataState.StreamMetadata;
        ISegmentMetadata segmentMetadata = this._metadataState.SegmentMetadata;
        state.SegmentMetadata = segmentMetadata;
        state.SegmentTimestamp = this._metadataState.SegmentTimestamp;
        state.StreamMetadata = streamMetadata;
        state.StreamTimestamp = this._metadataState.StreamTimestamp;
        state.TrackMetadata = this._metadataState.TrackMetadata;
        state.ConfigurationMetadata = this._metadataState.ConfigurationMetadata;
        int num1;
        if (this._metadataState.StreamTimestamp > this._position)
        {
          TimeSpan streamTimestamp = this._metadataState.StreamTimestamp;
          TimeSpan? nullable2 = nullable1;
          num1 = (nullable2.HasValue ? (streamTimestamp < nullable2.GetValueOrDefault() ? 1 : 0) : 0) == 0 ? 1 : 0;
        }
        else
          num1 = 1;
        if (num1 == 0)
          nullable1 = new TimeSpan?(this._metadataState.StreamTimestamp);
        int num2;
        if (this._metadataState.SegmentTimestamp > this._position)
        {
          TimeSpan segmentTimestamp = this._metadataState.SegmentTimestamp;
          TimeSpan? nullable2 = nullable1;
          num2 = (nullable2.HasValue ? (segmentTimestamp < nullable2.GetValueOrDefault() ? 1 : 0) : 0) == 0 ? 1 : 0;
        }
        else
          num2 = 1;
        if (num2 == 0)
          nullable1 = new TimeSpan?(this._metadataState.SegmentTimestamp);
        return nullable1;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
    }
  }
}
