using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Wpf.Source.Common.Utils.TypeUtils;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class DataGridExtensions
    {
        public static void ScrollToEnd(this DataGrid gv)
        {
            if (gv == null)
                throw new ArgumentNullException(nameof(gv));

            if (gv.Items.Count > 0)
                gv.GetScrollViewer()?.ScrollToEnd();
        }

        public static void ScrollToStart(this DataGrid gv)
        {
            if (gv == null)
                throw new ArgumentNullException(nameof(gv));

            if (gv.Items.Count > 0)
                gv.ScrollIntoView(gv.Items[0]);
        }

        public static void ScrollTo<T>(this DataGrid gv, T item)
        {
            if (gv == null)
                throw new ArgumentNullException(nameof(gv));

            if (gv.Items.Count > 0)
            {
                var sv = gv.GetScrollViewer();
                var items = gv.Items.Cast<T>().ToArray();
                var itemToScrollTo = items.Single(i => Equals(i, item));
                sv.ScrollToVerticalOffset(items.IndexOf_(itemToScrollTo));
            }
        }

        public static void SetSelecteditemsSource<T>(this DataGrid gv, ObservableCollection<T> items)
        {
            DataGridUtils.SetSelectedItems(gv, items);
        }
    }
}
