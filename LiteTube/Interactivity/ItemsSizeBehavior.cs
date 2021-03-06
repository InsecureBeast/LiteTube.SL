﻿using System.Windows.Controls;
using LiteTube.Common;
using LiteTube.Common.Helpers;

namespace LiteTube.Interactivity
{
    class ItemSizeBehavior : ItemWidthBehavior
    {
        protected override void SetItemSize()
        {
            base.SetItemSize();
            var height = _frameworkElement.Width / 1.778;
            _frameworkElement.Height = height + 95;
            var border = VisualHelper.FindChild<Border>(_frameworkElement);
                if (border != null)
                    border.Height = height;
        }
    }
}
