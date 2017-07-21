using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SM.Media.Core.TransportStream.TsParser;

namespace SM.Media.Core
{
  public sealed class TsTimestamp : ITsTimestamp
  {
    public static bool EnableDiscontinutityFilter = true;
    private static readonly TimeSpan MaximumError = TimeSpan.FromMilliseconds(5.0);
    private readonly List<TsTimestamp.PacketsState> _packetsStates = new List<TsTimestamp.PacketsState>();
    private TimeSpan? _timestampOffset;

    public TimeSpan StartPosition { get; set; }

    public TimeSpan? Offset
    {
      get
      {
        return this._timestampOffset;
      }
    }

    public void Flush()
    {
      this._timestampOffset = new TimeSpan?();
    }

    public bool ProcessPackets()
    {
      if (this._packetsStates.Count <= 0)
        return false;
      bool flag1 = TsTimestamp.EnableDiscontinutityFilter;
      if (!this._timestampOffset.HasValue)
      {
        TimeSpan timeSpan1 = TimeSpan.MaxValue;
        TimeSpan timeSpan2 = TimeSpan.MaxValue;
        foreach (TsTimestamp.PacketsState packetsState in this._packetsStates)
        {
          bool flag2 = false;
          if (packetsState.IsMedia)
          {
            foreach (TsPesPacket tsPesPacket in (IEnumerable<TsPesPacket>) packetsState.Packets)
            {
              if (null != tsPesPacket)
              {
                flag2 = true;
                if (tsPesPacket.PresentationTimestamp < timeSpan1)
                  timeSpan1 = tsPesPacket.PresentationTimestamp;
                TimeSpan? nullable = tsPesPacket.DecodeTimestamp;
                TimeSpan timeSpan3 = timeSpan2;
                if ((nullable.HasValue ? (nullable.GetValueOrDefault() < timeSpan3 ? 1 : 0) : 0) != 0 && tsPesPacket.DecodeTimestamp.HasValue)
                  timeSpan2 = tsPesPacket.DecodeTimestamp.Value;
              }
            }
            if (!flag2)
              return false;
          }
        }
        TimeSpan timeSpan4 = timeSpan1;
        if (timeSpan2 < timeSpan4)
          timeSpan4 = timeSpan2;
        this._timestampOffset = new TimeSpan?(timeSpan4 - this.StartPosition);
        Debug.WriteLine("TsTimestamp.ProcessPackets() syncing pts {0} dts {1} target {2} => offset {3}", (object) timeSpan1, (object) timeSpan2, (object) this.StartPosition, (object) this._timestampOffset);
      }
      else if (flag1)
      {
        foreach (TsTimestamp.PacketsState packetsState in this._packetsStates)
        {
          if (packetsState.Packets.Count > 0 && (packetsState.Duration.HasValue && packetsState.PresentationTimestamp.HasValue))
          {
            TsPesPacket tsPesPacket = Enumerable.First<TsPesPacket>((IEnumerable<TsPesPacket>) packetsState.Packets);
            if (null != tsPesPacket)
            {
              TimeSpan timeSpan1 = tsPesPacket.PresentationTimestamp - this._timestampOffset.Value;
              TimeSpan timeSpan2 = packetsState.PresentationTimestamp.Value + packetsState.Duration.Value;
              TimeSpan timeSpan3 = timeSpan1 - timeSpan2;
              if (!(timeSpan3 < TsTimestamp.MaximumError) || !(timeSpan3 > -TsTimestamp.MaximumError))
              {
                TimeSpan timeSpan4 = tsPesPacket.PresentationTimestamp - timeSpan2;
                Debug.WriteLine("TsTimestamp.ProcessPackets() resyncing expected pts {0} actual pts {1} target {2} => offset {3} (was {4})", (object) timeSpan2, (object) timeSpan1, (object) this.StartPosition, (object) timeSpan4, (object) this._timestampOffset);
                this._timestampOffset = new TimeSpan?(timeSpan4);
              }
            }
          }
        }
      }
      TimeSpan? nullable1 = this._timestampOffset;
      TimeSpan timeSpan = TimeSpan.Zero;
      if ((!nullable1.HasValue ? 1 : (nullable1.GetValueOrDefault() != timeSpan ? 1 : 0)) != 0)
        this.AdjustTimestamps(this._timestampOffset.Value);
      if (flag1)
      {
        foreach (TsTimestamp.PacketsState packetsState in this._packetsStates)
        {
          if (packetsState.Packets.Count > 0)
          {
            TsPesPacket tsPesPacket = Enumerable.Last<TsPesPacket>((IEnumerable<TsPesPacket>) packetsState.Packets);
            if (null != tsPesPacket)
            {
              packetsState.PresentationTimestamp = new TimeSpan?(tsPesPacket.PresentationTimestamp);
              packetsState.DecodeTimestamp = tsPesPacket.DecodeTimestamp;
              packetsState.Duration = !tsPesPacket.Duration.HasValue ? (null == packetsState.GetDuration ? new TimeSpan?() : packetsState.GetDuration(tsPesPacket)) : tsPesPacket.Duration;
            }
          }
        }
      }
      return true;
    }

    public void RegisterMediaStream(MediaStream mediaStream, Func<TsPesPacket, TimeSpan?> getDuration)
    {
      this._packetsStates.Add(new TsTimestamp.PacketsState()
      {
        Packets = mediaStream.Packets,
        GetDuration = getDuration,
        IsMedia = null != mediaStream.ConfigurationSource
      });
    }

    private void AdjustTimestamps(TimeSpan offset)
    {
      foreach (TsTimestamp.PacketsState packetsState in this._packetsStates)
      {
        foreach (TsPesPacket tsPesPacket1 in (IEnumerable<TsPesPacket>) packetsState.Packets)
        {
          if (null != tsPesPacket1)
          {
            tsPesPacket1.PresentationTimestamp -= offset;
            if (tsPesPacket1.DecodeTimestamp.HasValue)
            {
              TsPesPacket tsPesPacket2 = tsPesPacket1;
              TimeSpan? nullable1 = tsPesPacket2.DecodeTimestamp;
              TimeSpan timeSpan = offset;
              TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(nullable1.GetValueOrDefault() - timeSpan) : new TimeSpan?();
              tsPesPacket2.DecodeTimestamp = nullable2;
            }
          }
        }
      }
    }

    private class PacketsState
    {
      public TimeSpan? DecodeTimestamp;
      public TimeSpan? Duration;
      public Func<TsPesPacket, TimeSpan?> GetDuration;
      public bool IsMedia;
      public ICollection<TsPesPacket> Packets;
      public TimeSpan? PresentationTimestamp;
    }
  }
}
