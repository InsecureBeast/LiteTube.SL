using System;

namespace SM.Media.Core.Buffering
{
  public class DefaultBufferingPolicy : IBufferingPolicy
  {
    private int _bytesMaximum = 8388608;
    private int _bytesMinimum = 307200;
    private int _bytesMinimumStarting = 102400;
    private TimeSpan _durationBufferingDone = TimeSpan.FromSeconds(9.0);
    private TimeSpan _durationBufferingMax = TimeSpan.FromSeconds(25.0);
    private TimeSpan _durationReadDisable = TimeSpan.FromSeconds(30.0);
    private TimeSpan _durationReadEnable = TimeSpan.FromSeconds(15.0);
    private TimeSpan _durationStartingDone = TimeSpan.FromSeconds(2.5);

    public int BytesMaximum
    {
      get
      {
        return this._bytesMaximum;
      }
      set
      {
        this._bytesMaximum = value;
      }
    }

    public int BytesMinimum
    {
      get
      {
        return this._bytesMinimum;
      }
      set
      {
        this._bytesMinimum = value;
      }
    }

    public int BytesMinimumStarting
    {
      get
      {
        return this._bytesMinimumStarting;
      }
      set
      {
        this._bytesMinimumStarting = value;
      }
    }

    public TimeSpan DurationReadEnable
    {
      get
      {
        return this._durationReadEnable;
      }
      set
      {
        this._durationReadEnable = value;
      }
    }

    public TimeSpan DurationBufferingDone
    {
      get
      {
        return this._durationBufferingDone;
      }
      set
      {
        this._durationBufferingDone = value;
      }
    }

    public TimeSpan DurationStartingDone
    {
      get
      {
        return this._durationStartingDone;
      }
      set
      {
        this._durationStartingDone = value;
      }
    }

    public TimeSpan DurationReadDisable
    {
      get
      {
        return this._durationReadDisable;
      }
      set
      {
        this._durationReadDisable = value;
      }
    }

    public TimeSpan DurationBufferingMax
    {
      get
      {
        return this._durationBufferingMax;
      }
      set
      {
        this._durationBufferingMax = value;
      }
    }

    public virtual bool ShouldBlockReads(bool isReadBlocked, TimeSpan durationBuffered, int bytesBuffered, bool isExhausted, bool isAllExhausted)
    {
      if (isAllExhausted)
        return false;
      if (bytesBuffered > this.BytesMaximum)
        return true;
      if (isExhausted || durationBuffered < this.DurationReadEnable)
        return false;
      if (durationBuffered > this.DurationReadDisable)
        return true;
      return isReadBlocked;
    }

    public virtual bool IsDoneBuffering(TimeSpan bufferDuration, int bytesBuffered, int bytesBufferedWhenExhausted, bool isStarting)
    {
      int num1 = Math.Max(0, bytesBuffered - bytesBufferedWhenExhausted);
      TimeSpan timeSpan = isStarting ? this.DurationStartingDone : this.DurationBufferingDone;
      int num2 = isStarting ? this.BytesMinimumStarting : this.BytesMinimum;
      return bufferDuration >= timeSpan && num1 >= num2 || bytesBuffered >= this.BytesMaximum || bufferDuration > this.DurationBufferingMax;
    }

    public virtual float GetProgress(TimeSpan bufferDuration, int bytesBuffered, int bytesBufferedWhenExhausted, bool isStarting)
    {
      TimeSpan timeSpan = isStarting ? this.DurationStartingDone : this.DurationBufferingDone;
      int num1 = isStarting ? this.BytesMinimumStarting : this.BytesMinimum;
      int num2 = Math.Max(0, bytesBuffered - bytesBufferedWhenExhausted);
      float num3 = Math.Max(Math.Max(Math.Min(Math.Max(0.0f, (float) bufferDuration.Ticks / (float) timeSpan.Ticks), (float) num2 / (float) num1), (float) bytesBuffered / (float) this.BytesMaximum), Math.Max(0.0f, (float) bufferDuration.Ticks / (float) this.DurationBufferingMax.Ticks));
      if ((double) num3 > 1.0)
        num3 = 1f;
      else if ((double) num3 < 0.0)
        num3 = 0.0f;
      return num3;
    }
  }
}
