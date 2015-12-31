using System.Windows;
using System;
using System.Windows.Interactivity;

namespace LiteTube.Interactivity
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
                //_frameworkElement = AssociatedObject as FrameworkElement;
                //if (_frameworkElement == null)
                //    return;

                //_frameworkElement.Loaded += OnLoaded;
            }
        }

        public void Detach()
        {
            AssociatedObject = null;
        }
    }
}
