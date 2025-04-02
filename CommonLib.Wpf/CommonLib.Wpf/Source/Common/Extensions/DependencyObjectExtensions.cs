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
        public static IEnumerable<DependencyObject> LogicalChildren(this DependencyObject depObj)
        {
            if (depObj is null) yield break;
            var children = Application.Current.Dispatcher.Invoke(() => LogicalTreeHelper.GetChildren(depObj).Cast<object>().ToArray());
            foreach (var rawChild in children)
            {
                if (rawChild is not DependencyObject depObjRawChild) continue;
                yield return depObjRawChild;
            }
        }

        public static IEnumerable<T> LogicalChildren<T>(this DependencyObject depObj) where T : UIElement
        {
            if (depObj is null) yield break;
            var children = Application.Current.Dispatcher.Invoke(() => LogicalTreeHelper.GetChildren(depObj).Cast<object>().ToArray());
            foreach (var rawChild in children)
            {
                if (rawChild is not DependencyObject depObjRawChild) continue;
                if (depObjRawChild is T tChild)
                    yield return tChild;
            }
        }

        public static IEnumerable<DependencyObject> LogicalDescendants(this DependencyObject depObj)
        {
            if (depObj is null) yield break;
            foreach (var child in depObj.LogicalChildren())
            {
                yield return child;
                foreach (var descendant in child.LogicalDescendants())
                    yield return descendant;
            }
        }

        public static IEnumerable<T> LogicalDescendants<T>(this DependencyObject depObj) where T : UIElement
        {
            if (depObj is null) yield break;
            foreach (var rawChild in depObj.LogicalChildren())
            {
                if (rawChild is not { }) continue;
                if (rawChild is T tChild)
                    yield return tChild;
                foreach (var tDescendant in LogicalDescendants<T>(rawChild))
                    yield return tDescendant;
            }
        }

        public static IEnumerable<Control> LogicalDescendants<T1, T2>(this DependencyObject depObj)
            where T1 : UIElement
            where T2 : UIElement
        {
            return depObj.LogicalDescendants<T1>().Cast<Control>().ConcatMany(depObj.LogicalDescendants<T2>().Cast<Control>());
        }

        public static IEnumerable<Control> LogicalDescendants<T1, T2, T3>(this DependencyObject depObj)
            where T1 : UIElement
            where T2 : UIElement
            where T3 : UIElement
        {
            return depObj.LogicalDescendants<T1>().Cast<Control>().ConcatMany(
                depObj.LogicalDescendants<T2>().Cast<Control>(),
                depObj.LogicalDescendants<T3>().Cast<Control>());
        }
        
        public static IEnumerable<Control> LogicalDescendants<T1, T2, T3, T4>(this DependencyObject depObj)
            where T1 : UIElement
            where T2 : UIElement
            where T3 : UIElement
            where T4 : UIElement
        {
            return depObj.LogicalDescendants<T1>().Cast<Control>().ConcatMany(
                depObj.LogicalDescendants<T2>().Cast<Control>(),
                depObj.LogicalDescendants<T3>().Cast<Control>(),
                depObj.LogicalDescendants<T4>().Cast<Control>());
        }

        public static IEnumerable<Control> LogicalDescendants<T1, T2, T3, T4, T5>(this DependencyObject depObj)
            where T1 : UIElement
            where T2 : UIElement
            where T3 : UIElement
            where T4 : UIElement
            where T5 : UIElement
        {
            return depObj.LogicalDescendants<T1>().Cast<Control>().ConcatMany(
                depObj.LogicalDescendants<T2>().Cast<Control>(),
                depObj.LogicalDescendants<T3>().Cast<Control>(),
                depObj.LogicalDescendants<T4>().Cast<Control>(),
                depObj.LogicalDescendants<T5>().Cast<Control>());
        }

        public static IEnumerable<Control> LogicalDescendants<T1, T2, T3, T4, T5, T6>(this DependencyObject depObj)
            where T1 : UIElement
            where T2 : UIElement
            where T3 : UIElement
            where T4 : UIElement
            where T5 : UIElement
            where T6 : UIElement
        {
            return depObj.LogicalDescendants<T1>().Cast<Control>().ConcatMany(
                depObj.LogicalDescendants<T2>().Cast<Control>(),
                depObj.LogicalDescendants<T3>().Cast<Control>(),
                depObj.LogicalDescendants<T4>().Cast<Control>(),
                depObj.LogicalDescendants<T5>().Cast<Control>(),
                depObj.LogicalDescendants<T6>().Cast<Control>());
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

        public static DependencyObject ClearLabels(this DependencyObject depObj)
        {
            depObj.LogicalDescendants<Label>().ClearValues();
            return depObj;
        }
    }
}
