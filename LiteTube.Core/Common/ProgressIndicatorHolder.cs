using System;
using System.Collections.Generic;
using Microsoft.Phone.Shell;

namespace LiteTube.Core.Common
{
    public class ProgressIndicatorHolder
    {
        private ProgressIndicator _progressIndicator;
        private readonly List<Action> _listeners = new List<Action>();
        
        public ProgressIndicator ProgressIndicator
        {
            get { return _progressIndicator; }
            set 
            {
                _progressIndicator = value;
                Notify();
            }
        }

        public void Subscribe(Action action)
        {
            _listeners.Add(action);
        }

        public void Notify()
        {
            foreach (var action in _listeners.ToArray())
            {
                if (action != null)
                    action();
            }
        }
    }
}
