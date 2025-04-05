using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommonLib.Wpf.Source.Common.Utils.TypeUtils;
using MoreLinq.Extensions;
using static CommonLib.Wpf.Source.Common.Utils.WpfAsyncUtils;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class LabelExtensions
    {
        public static Label SetText(this Label lbl, string text)
        {
            var tb = lbl.LogicalDescendants<TextBlock>().ToArray();
            var hasSingleTextBlock = tb.Length == 1;
            DispatchIfNeeded(() =>
            {
                if (hasSingleTextBlock)
                    tb.Single().Text = text;
                else
                    lbl.Content = text;
            });
            return lbl;
        }

        public static Label ClearText(this Label lbl) => lbl.SetText(string.Empty);

        public static IEnumerable<Label> SetText(this IEnumerable<Label> lbls, string text)
        {
            var arrLbls = lbls as Label[] ?? lbls.ToArray();
            foreach (var lbl in arrLbls)
                lbl.SetText(text);
            return arrLbls;
        }

        public static IEnumerable<Label> ClearText(this IEnumerable<Label> lbl) => lbl.SetText(string.Empty);

        public static Label SetForeground(this Label lbl, ControlState state)
        {
            var wnd = lbl.LogicalAncestor<Window>();
            Application.Current.Dispatcher.Invoke(() => lbl.Foreground = state switch
            {
                ControlState.Default => wnd.GetDefaultLabelForegroundBrush(),
                ControlState.Valid => wnd.GetValidForegroundBrush(),
                ControlState.Invalid => wnd.GetInvalidForegroundBrush(),
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            });
            return lbl;
        }

        public static Label SetDefaultForeground(this Label lbl) => lbl.SetForeground(ControlState.Default);
        public static Label SetValidForeground(this Label lbl) => lbl.SetForeground(ControlState.Valid);
        public static Label SetInvalidForeground(this Label lbl) => lbl.SetForeground(ControlState.Invalid);

        public static IEnumerable<Label> SetForeground(this IEnumerable<Label> lbls, ControlState state)
        {
            var arrLbls = lbls as Label[] ?? lbls.ToArray();
            arrLbls.ForEach(lbl => lbl.SetForeground(state));
            return arrLbls;
        }

        public static IEnumerable<Label> SetDefaultBackground(this IEnumerable<Label> lbl) => lbl.SetForeground(ControlState.Default);
        public static IEnumerable<Label> SetValidBackground(this IEnumerable<Label> lbl) => lbl.SetForeground(ControlState.Valid);
        public static IEnumerable<Label> SetInvalidBackground(this IEnumerable<Label> lbl) => lbl.SetForeground(ControlState.Invalid);
    }
}
