using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace LiteTube.Core.Controls
{
    class GenericListView : LongListSelector
    {
        private static readonly DependencyProperty SelectedItems2Property = DependencyProperty.Register("SelectedItems2", typeof(object), typeof(GenericListView), null);
        private bool _isMergingItems;

        public GenericListView()
        {
            SelectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// SelectedItems2 is used as the property to bind the selected items to the ViewModel.
        /// Using a List as the data type did not seem to work for binding.  Since ItemSource is an object on the base class
        /// object was chosen for this property as well.
        /// 
        /// However, SelectedItems2 will need to bind to IList in order for it to work properly.
        /// </summary>
        public object SelectedItems2
        {
            get { return GetValue(SelectedItems2Property); }
            set { SetValue(SelectedItems2Property, value); }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ignore selection changed event when selection mode is not multiple.
            // This is mainly here because when changing from Multiple to Single, this event is fired and will cause
            // the items to be removed from SelectedItems2 which is not what is desired.
            //if (this.SelectionMode != SelectionMode.Multiple)
            //   return;

            if (_isMergingItems)
                return;

            try
            {
                _isMergingItems = true;

                var selectedItems2List = SelectedItems2 as IList;
                if (selectedItems2List == null)
                    return;

                foreach (var itemToAdd in e.AddedItems)
                {
                    selectedItems2List.Add(itemToAdd);
                }

                foreach (var itemToRemove in e.RemovedItems)
                {
                    selectedItems2List.Remove(itemToRemove);
                }
            }
            finally
            {
                _isMergingItems = false;
            }
        }
    }
}
