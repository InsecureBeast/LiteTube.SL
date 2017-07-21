// Decompiled with JetBrains decompiler
// Type: SM.Media.Utility.TaskCollector
// Assembly: SM.Media, Version=1.5.3.0, Culture=neutral, PublicKeyToken=a8a96d0f02112ebc
// MVID: 36CDA6C8-9742-4B9A-8F0F-25CFBA3563E6
// Assembly location: D:\Programming\WP\phonesm-1.5.3-beta\bin\Debug\SM.Media.dll

using SM.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Media.Utility
{
  public class TaskCollector
  {
    public static readonly TaskCollector Default = new TaskCollector();
    private readonly object _lock = new object();
    private readonly Dictionary<Task, string> _tasks = new Dictionary<Task, string>();

    public void Add(Task task, string description)
    {
      if (task.IsCompleted)
      {
        TaskCollector.ReportException(task, description);
      }
      else
      {
        bool lockTaken = false;
        object obj;
        try
        {
          Monitor.Enter(obj = this._lock, ref lockTaken);
          Debug.Assert(!this._tasks.ContainsKey(task));
          this._tasks[task] = description;
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(obj);
        }
        task.ContinueWith(new Action<Task>(this.Cleanup));
      }
    }

    [Conditional("DEBUG")]
    public void Wait()
    {
      bool lockTaken = false;
      object obj;
      KeyValuePair<Task, string>[] keyValuePairArray;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        keyValuePairArray = Enumerable.ToArray<KeyValuePair<Task, string>>((IEnumerable<KeyValuePair<Task, string>>) this._tasks);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      if (keyValuePairArray == null || 0 == keyValuePairArray.Length)
        return;
      try
      {
        TaskEx.WhenAll(Enumerable.Select<KeyValuePair<Task, string>, Task>((IEnumerable<KeyValuePair<Task, string>>) keyValuePairArray, (Func<KeyValuePair<Task, string>, Task>) (t => t.Key))).Wait();
      }
      catch (AggregateException ex)
      {
        foreach (Exception exception in ex.InnerExceptions)
          Debug.WriteLine("TaskCollector.Wait() Task wait failed: " + ex.Message);
      }
      catch (Exception ex)
      {
        Debug.WriteLine("TaskCollector.Wait() Task wait failed: " + ex.Message);
      }
    }

    private void Cleanup(Task task)
    {
      Debug.Assert(task.IsCompleted);
      bool condition = false;
      bool lockTaken = false;
      object obj;
      string description;
      try
      {
        Monitor.Enter(obj = this._lock, ref lockTaken);
        if (this._tasks.TryGetValue(task, out description))
          condition = this._tasks.Remove(task);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(obj);
      }
      Debug.Assert(condition, description ?? "No description");
      TaskCollector.ReportException(task, description);
    }

    private static void ReportException(Task task, string description)
    {
      try
      {
        AggregateException exception = task.Exception;
        if (null == exception)
          return;
        Debug.WriteLine("TaskCollector.Cleanup() task {0} failed: {1}{2}{3}", (object) description, (object) ExceptionExtensions.ExtendedMessage((Exception) exception), (object) Environment.NewLine, (object) exception.StackTrace);
        if (Debugger.IsAttached)
          Debugger.Break();
      }
      catch (Exception ex)
      {
        Debug.WriteLine("TaskCollector.Cleanup() cleanup of task {0} failed: {1}", (object) description, (object) ExceptionExtensions.ExtendedMessage(ex));
        if (!Debugger.IsAttached)
          return;
        Debugger.Break();
      }
    }
  }
}
