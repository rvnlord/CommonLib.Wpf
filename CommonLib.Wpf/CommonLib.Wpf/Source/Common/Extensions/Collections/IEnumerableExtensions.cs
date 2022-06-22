using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib.Source.Common.Extensions;
using CommonLib.Wpf.Source.Common.Utils.UtilClasses.Menu;
using MoreLinq;
using ContextMenu = CommonLib.Wpf.Source.Common.Utils.UtilClasses.Menu.ContextMenu;

namespace CommonLib.Wpf.Source.Common.Extensions.Collections
{
    public static class IEnumerableExtensions
    {
        public static void DetachFromParent(this IEnumerable<FrameworkElement> controls)
        {
            controls.ForEach(c => c.DetachFromParent());
        }

        public static void DetachFromParent(this FrameworkElement control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            ((Panel)control.Parent).Children.Remove(control);
        }

        public static void Highlight<T>(this IEnumerable<T> controls, Color color) where T : Control
        {
            controls.ForEach(control => control.Highlight(color));
        }

        public static DataGridTextColumn ByDataMemberName(this IEnumerable<DataGridTextColumn> columns, string dataMemberName)
        {
            return columns.Single(c => string.Equals(c.DataMemberName(), dataMemberName, StringComparison.Ordinal));
        }

        public static void EnableControls(this IEnumerable<object> controls)
        {
            if (controls == null)
                throw new ArgumentNullException(nameof(controls));

            foreach (var c in controls)
            {
                var piIsEnabled = c.GetType().GetProperty("IsEnabled");
                piIsEnabled?.SetValue(c, true);

                if (c.GetType() == typeof(ContextMenu))
                {
                    var cm = (ContextMenu)c;
                    if (cm.IsOpen())
                    {
                        var wnd = cm.Control.LogicalAncestor<Window>();
                        var handler = wnd.GetType().GetRuntimeMethods().FirstOrDefault(m => m.Name == $"cm{cm.Control.Name.Take(1).ToUpperInvariant()}{cm.Control.Name.Skip(1)}_Open");
                        handler?.Invoke(wnd, new object[] { cm, new ContextMenuOpenEventArgs(cm) });
                    }
                }
            }
        }

        public static void ToggleControls(this IEnumerable<object> controls)
        {
            if (controls == null)
                throw new ArgumentNullException(nameof(controls));

            foreach (var c in controls)
                c.SetProperty("IsEnabled", c.GetProperty<bool>("IsEnabled"));
        }
    }
}
