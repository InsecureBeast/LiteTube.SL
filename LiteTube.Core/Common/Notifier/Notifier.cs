using System;
using System.Collections.Generic;

namespace LiteTube.Core.Common.Notifier
{
    public interface IListener<in T>
    {
        void Notify(T e);
    }

    public class Notifier<T>
    {
        private readonly List<WeakReference> _listeners = new List<WeakReference>();

        public void Subscribe(IListener<T> listener)
        {
            _listeners.Add(new WeakReference(listener));
        }

        public void Unsubscribe(IListener<T> listener)
        {
            _listeners.RemoveAll(x => ReferenceEquals(listener, x.Target));
        }

        public void Notify(T e)
        {
            foreach (var l in _listeners.ToArray())
            {
                var listener = l.Target as IListener<T>;
                if (listener != null)
                    listener.Notify(e);
                else
                    _listeners.Remove(l);

            }
        }
    }
}
