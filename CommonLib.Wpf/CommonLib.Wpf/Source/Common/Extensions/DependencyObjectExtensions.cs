using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib.Source.Common.Extensions.Collections;
using MahApps.Metro.Controls;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<T> LogicalDescendants<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj is null) yield break;

            var children = Application.Current.Dispatcher.Invoke(() => LogicalTreeHelper.GetChildren(depObj).Cast<object>().ToArray());

            foreach (var rawChild in children)
            {
                if (rawChild is not DependencyObject depObjRawChild) continue;
                if (depObjRawChild is T tChild)
                    yield return tChild;

                foreach (var childOfChild in LogicalDescendants<T>(depObjRawChild))
                    yield return childOfChild;
            }
        }

        public static IEnumerable<Control> LogicalDescendants<T1, T2>(this DependencyObject depObj)
            where T1 : DependencyObject
            where T2 : DependencyObject
        {
            return LogicalDescendants<T1>(depObj).Cast<Control>().ConcatMany(LogicalDescendants<T2>(depObj).Cast<Control>());
        }
        
        public static ScrollViewer GetScrollViewer(this DependencyObject o)
        {
            if (o is ScrollViewer sv) { return sv; }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                    continue;
                return result;
            }

            return null;
        }

        public static T LogicalAncestor<T>(this DependencyObject child) where T : DependencyObject
        {
            while (true)
            {
                var parentObject = LogicalTreeHelper.GetParent(child);
                if (parentObject == null) return null;
                if (parentObject is T parent) return parent;
                child = parentObject;
            }
        }

        public static T LogicalAncestor<T>(this DependencyObject child, Func<T, bool> condition) where T : DependencyObject
        {
            while (true)
            {
                var parentObject = LogicalTreeHelper.GetParent(child);
                if (parentObject == null) return null;
                if (parentObject is T parent && condition(parent)) return parent;
                child = parentObject;
            }
        }

        public static IEnumerable<Visual> VisualDescendants(this Visual parent)
        {
            if (parent == null)
                yield break;
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is not Visual child)
                    continue;
                yield return child;
                foreach (var grandChild in child.VisualDescendants())
                    yield return grandChild;
            }
        }

        public static IEnumerable<T> VisualDescendants<T>(this Visual parent) where T : Visual
        {
            if (parent == null)
                yield break;
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is not Visual child)
                    continue;
                if (child is T tChild)
                    yield return tChild;
                foreach (var grandChild in child.VisualDescendants<T>())
                    yield return grandChild;
            }
        }

        public static MetroWindow Window(this DependencyObject child) => child.LogicalAncestor<MetroWindow>();
    }
}
