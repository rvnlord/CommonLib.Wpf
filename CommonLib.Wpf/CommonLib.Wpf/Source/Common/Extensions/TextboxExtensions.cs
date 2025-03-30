using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib.Source.Common.Extensions;
using MoreLinq.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Controls.TextBox;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class TextboxExtensions
    {
        public static TextBox ResetValue(this TextBox txt, bool force = false)
        {
            if (txt == null)
                throw new ArgumentNullException(nameof(txt));

            var text = txt.Text;
            var tag = txt.Tag;
            if (tag == null)
                return txt;

            var placeholder = tag.ToString() ?? "...";
            if (text != placeholder && !string.IsNullOrWhiteSpace(text) && !force)
                return txt;

            var bg = ((SolidColorBrush)txt.Foreground).Color;
            var newBrush = new SolidColorBrush(Color.FromArgb(128, bg.R, bg.G, bg.B));

            txt.FontStyle = FontStyles.Italic;
            txt.Foreground = newBrush;
            txt.Text = placeholder;
            return txt;
        }

        public static TextBox ClearValue(this TextBox txt, bool force = false)
        {
            if (txt == null)
                throw new ArgumentNullException(nameof(txt));

            var text = txt.Text;
            var tag = txt.Tag;
            if (tag == null)
                return txt;

            var placeholder = tag.ToString();
            if (text != placeholder && !force)
                return txt;

            var bg = ((SolidColorBrush)txt.Foreground).Color;
            var newBrush = new SolidColorBrush(Color.FromArgb(255, bg.R, bg.G, bg.B));

            txt.FontStyle = FontStyles.Normal;
            txt.Foreground = newBrush;
            txt.Text = string.Empty;
            return txt;
        }

        public static bool IsNullWhiteSpaceOrTag(this TextBox txtB)
        {
            if (txtB == null)
                throw new ArgumentNullException(nameof(txtB));

            return txtB.Text.IsNullWhiteSpaceOrDefault(txtB.Tag?.ToString() ?? "");
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
