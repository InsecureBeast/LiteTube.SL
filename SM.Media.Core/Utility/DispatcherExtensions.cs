using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SM.Media.Core.Utility
{
    public static class DispatcherExtensions
    {
        public static Task DispatchAsync(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
            {
                action();
                return TplTaskExtensions.CompletedTask;
            }
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }));
            return (Task)tcs.Task;
        }

        public static Task<T> DispatchAsync<T>(this Dispatcher dispatcher, Func<T> action)
        {
            if (dispatcher.CheckAccess())
                return TaskEx.FromResult<T>(action());
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    tcs.TrySetResult(action());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }));
            return tcs.Task;
        }

        public static Task DispatchAsync(this Dispatcher dispatcher, Func<Task> action)
        {
            if (dispatcher.CheckAccess())
                return action();
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            dispatcher.BeginInvoke((Action)(() => action().ContinueWith((Action<Task>)(t =>
            {
                if (t.IsCanceled)
                    tcs.TrySetCanceled();
                if (t.IsFaulted)
                    tcs.TrySetException((Exception)t.Exception);
                tcs.TrySetResult(true);
            }))));
            return (Task)tcs.Task;
        }

        public static Task<T> DispatchAsync<T>(this Dispatcher dispatcher, Func<Task<T>> action)
        {
            if (dispatcher.CheckAccess())
                return action();
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            dispatcher.BeginInvoke((Action)(() => action().ContinueWith((Action<Task<T>>)(t =>
            {
                if (t.IsCanceled)
                    tcs.TrySetCanceled();
                if (t.IsFaulted)
                    tcs.TrySetException((Exception)t.Exception);
                tcs.TrySetResult(t.Result);
            }))));
            return tcs.Task;
        }
    }
}
