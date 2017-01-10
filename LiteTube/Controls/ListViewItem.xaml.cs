using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LiteTube.Controls
{
    public partial class ListViewItem : UserControl
    {
        public ListViewItem()
        {
            InitializeComponent();

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(new MenuItem() { Header = "Copy" });

            ContextMenuService.SetContextMenu(LayoutRoot, contextMenu);
        }
    }
}
