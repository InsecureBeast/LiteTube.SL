// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.ResettableParameters`1
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using System;
using System.Threading;

namespace SM.Media.Utility
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
        object obj;
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
        object obj;
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
