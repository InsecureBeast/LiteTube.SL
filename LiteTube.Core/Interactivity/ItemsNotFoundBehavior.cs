using System;
using System.Windows;
using System.Windows.Interactivity;

namespace LiteTube.Core.Interactivity
{
    class ItemsNotFoundBehavior : IAttachedObject
    {
        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject dependencyObject)
        {
            if ((AssociatedObject != dependencyObject))
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach behavior multiple times.");

                AssociatedObject = AssociatedObject;
                //_richTextBox = AssociatedObject as FrameworkElement;
                //if (_richTextBox == null)
                //    return;

                //_richTextBox.Loaded += OnLoaded;
            }
        }

        public void Detach()
        {
            AssociatedObject = null;
        }
    }
}
