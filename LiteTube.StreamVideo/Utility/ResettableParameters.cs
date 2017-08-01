using System;
using System.Threading;

namespace LiteTube.StreamVideo.Utility
{
  public class ResettableParameters<TParameters> where TParameters : class, new()
  {
    private readonly object _lock = new object();
    private TParameters _parameters;

    public TParameters Parameters
    {
      get
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          if (null == (object) this._parameters)
            this._parameters = Activator.CreateInstance<TParameters>();
          return this._parameters;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
      set
      {
        bool lockTaken = false;
        object obj = null;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          this._parameters = value;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
      }
    }
  }
}
