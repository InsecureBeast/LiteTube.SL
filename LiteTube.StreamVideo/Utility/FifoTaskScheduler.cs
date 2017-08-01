using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiteTube.StreamVideo.Utility
{
  public sealed class FifoTaskScheduler : TaskScheduler, IDisposable
  {
    private readonly LinkedList<Task> _tasks = new LinkedList<Task>();
    private readonly SignalTask _workerTask;

    public override int MaximumConcurrencyLevel
    {
      get
      {
        return 1;
      }
    }

    public FifoTaskScheduler(CancellationToken cancellationToken)
    {
      this._workerTask = new SignalTask(new Func<Task>(this.Worker), cancellationToken);
    }

    private Task Worker()
    {
      try
      {
        while (true)
        {
          bool lockTaken = false;
          LinkedList<Task> linkedList = null;
          Task task;
          try
          {
            Monitor.Enter((object) (linkedList = this._tasks), ref lockTaken);
            if (0 == this._tasks.Count)
              return TplTaskExtensions.CompletedTask;
            task = this._tasks.First.Value;
            this._tasks.RemoveFirst();
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit((object) linkedList);
          }
          this.TryExecuteTask(task);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("FifoTaskScheduler.Worker() failed " + ex.Message);
      }
      return TplTaskExtensions.CompletedTask;
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
      bool lockTaken = false;
      LinkedList<Task> linkedList = null;
      try
      {
        Monitor.Enter((object) (linkedList = this._tasks), ref lockTaken);
        return (IEnumerable<Task>) Enumerable.ToArray<Task>((IEnumerable<Task>) this._tasks);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) linkedList);
      }
    }

    protected override void QueueTask(Task task)
    {
      bool lockTaken = false;
      LinkedList<Task> linkedList = null;
      try
      {
        Monitor.Enter((object) (linkedList = this._tasks), ref lockTaken);
        this._tasks.AddLast(task);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) linkedList);
      }
      this._workerTask.Fire();
    }

    protected override bool TryDequeue(Task task)
    {
      bool lockTaken = false;
      LinkedList<Task> linkedList = null;
      try
      {
        Monitor.Enter((object) (linkedList = this._tasks), ref lockTaken);
        return this._tasks.Remove(task);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) linkedList);
      }
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
      return false;
    }

    public void Dispose()
    {
      using (this._workerTask)
        ;
      bool lockTaken = false;
      LinkedList<Task> linkedList = null;
      Task[] taskArray;
      try
      {
        Monitor.Enter((object) (linkedList = this._tasks), ref lockTaken);
        taskArray = Enumerable.ToArray<Task>((IEnumerable<Task>) this._tasks);
        this._tasks.Clear();
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit((object) linkedList);
      }
      Debug.Assert(0 == taskArray.Length, "FifoTaskScheduler: Pending tasks abandoned");
    }
  }
}
