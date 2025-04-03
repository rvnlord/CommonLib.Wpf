using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib.Source.Common.Extensions;
using CommonLib.Wpf.Source.Common.Utils;
using MoreLinq.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class TextboxExtensions
    {
        public static TextBox ResetValue(this TextBox txt, bool force = false)
        {
            if (txt is null)
                throw new ArgumentNullException(nameof(txt));

            var text = Application.Current.Dispatcher.Invoke(() => txt.Text);
            var tag = Application.Current.Dispatcher.Invoke(() => txt.Tag);
            if (tag is null)
                return txt;

            var placeholder = tag.ToString() ?? "...";
            if (text != placeholder && !string.IsNullOrWhiteSpace(text) && !force)
                return txt;

            var bg = Application.Current.Dispatcher.Invoke(() => ((SolidColorBrush)txt.Foreground).Color);
            var newBrush = new SolidColorBrush(Color.FromArgb(128, bg.R, bg.G, bg.B));

            Application.Current.Dispatcher.Invoke(() =>
            {
                var textChangedHandlers = txt.RemoveEventHandlers("TextChanged");

                txt.FontStyle = FontStyles.Italic;
                txt.Foreground = newBrush;
                txt.Text = placeholder;

                txt.AddEventHandlers("TextChanged", textChangedHandlers);
            });

            return txt;
        }

        public static TextBox ClearValue(this TextBox txt, bool force = false)
        {
            if (txt is null)
                throw new ArgumentNullException(nameof(txt));

            var text = Application.Current.Dispatcher.Invoke(() => txt.Text);
            var tag = Application.Current.Dispatcher.Invoke(() => txt.Tag);
            if (tag is null)
                return txt;

            var placeholder = tag.ToString();
            if (text != placeholder && !force)
                return txt;

            var bg = Application.Current.Dispatcher.Invoke(() => ((SolidColorBrush)txt.Foreground).Color);
            var newBrush = new SolidColorBrush(Color.FromArgb(255, bg.R, bg.G, bg.B));

            Application.Current.Dispatcher.Invoke(() =>
            {
                var textChangedHandlers = txt.RemoveEventHandlers("TextChanged");

                txt.FontStyle = FontStyles.Normal;
                txt.Foreground = newBrush;
                txt.Text = string.Empty;

                txt.AddEventHandlers("TextChanged", textChangedHandlers);
            });

            return txt;
        }

        public static IEnumerable<TextBox> ClearValues(this IEnumerable<TextBox> txts, bool force = false)
        {
            var arrTxts = txts as TextBox[] ?? txts.ToArray();
            foreach (var txt in arrTxts)
                txt.ClearValue(force);
            return arrTxts;
        }

        public static IEnumerable<TextBox> ResetValues(this IEnumerable<TextBox> txts, bool force = false)
        {
            var arrTxts = txts as TextBox[] ?? txts.ToArray();
            foreach (var txt in arrTxts)
                txt.ResetValue(force);
            return arrTxts;
        }

        public static bool IsNullWhiteSpaceOrTag(this TextBox txtB)
        {
            if (txtB is null)
                throw new ArgumentNullException(nameof(txtB));

            return Application.Current.Dispatcher.Invoke(() => txtB.Text.IsNullWhiteSpaceOrDefault(txtB.Tag?.ToString() ?? ""));
        }

        public static TextBox NullifyIfTag(this TextBox txtB)
        {
            if (txtB == null)
                throw new ArgumentNullException(nameof(txtB));

            txtB.Text = txtB.Text.NullifyIf(text => text == txtB.Tag.ToString());
            return txtB;
        }

        public static void InitializeTextBoxPlaceholder(this TextBox txt)
        {
            txt.ResetValue();
            if (!txt.IsReadOnly && txt.IsEnabled)
            {
                txt.GotFocus += TxtAll_GotFocus;
                txt.LostFocus += TxtAll_LostFocus;
            }
        }

        public static void InitializeTextBoxPlaceholders(this IEnumerable<TextBox> txts)
        {
            txts.ForEach(InitializeTextBoxPlaceholder);
        }

        private static void TxtAll_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.ClearValue();
        }

        private static void TxtAll_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.ResetValue();
        }
    }
}
