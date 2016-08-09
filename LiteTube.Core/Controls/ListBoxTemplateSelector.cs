using System.Windows;
using System.Windows.Controls;
using LiteTube.Core.ViewModels.Nodes;

namespace LiteTube.Core.Controls
{
    public abstract class DataTemplateSelector : ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            ContentTemplate = SelectTemplate(newContent, this);
        }
    }


    public class ListBoxTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Adv
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
            if (item is AddNodeViewModel)
                return Adv;

            return Normal;
        }
    }
}
