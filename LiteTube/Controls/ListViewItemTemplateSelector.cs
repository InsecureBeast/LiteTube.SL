using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LiteTube.Controls
{
    class ListViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Large
        {
            get;
            set;
        }

        public DataTemplate Normal
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Normal;
        }
    }
}
