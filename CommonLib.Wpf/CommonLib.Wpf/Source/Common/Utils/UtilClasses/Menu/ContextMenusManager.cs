﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonLib.Source.Common.Extensions.Collections;
using MoreLinq;

namespace CommonLib.Wpf.Source.Common.Utils.UtilClasses.Menu
{
    public static class ContextMenusManager
    {
        private static Panel _contextMenusContainer;

        public static Panel ContextMenusContainer
        {
            get => _contextMenusContainer;
            set
            {
                _contextMenusContainer = value ?? throw new ArgumentNullException(nameof(value));
                _contextMenusContainer.PreviewMouseLeftButtonDown += ContextMenusContainer_PreviewMouseLeftButtonDown;
            }
        }

        public static Dictionary<FrameworkElement, ContextMenu> ContextMenus { get; } = new Dictionary<FrameworkElement, ContextMenu>();

        public static ContextMenu ContextMenu(FrameworkElement fe)
        {
            if (fe == null)
                throw new ArgumentNullException(nameof(fe));

            if (ContextMenus.VorN(fe) == null)
            {
                ContextMenus[fe] = new ContextMenu(fe);
                fe.PreviewMouseRightButtonUp += ContextMenu_PreviewMouseRightButtonUp;
            }

            return ContextMenus[fe];
        }

        public static void CloseAll()
        {
            ContextMenus.ForEach(cm => cm.Value?.Close());
        }

        public static void CloseAll(Func<ContextMenu, bool> selector)
        {
            ContextMenus.Select(kvp => kvp.Value).Where(selector).ForEach(cm => cm?.Close());
        }

        public static void ContextMenu_PreviewMouseRightButtonUp(object s, MouseButtonEventArgs e)
        {
            var f = (FrameworkElement)s;
            ContextMenus[f].Open();
        }

        private static void ContextMenusContainer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseAll(cm => !cm.IsHovered());
        }
    }
}
