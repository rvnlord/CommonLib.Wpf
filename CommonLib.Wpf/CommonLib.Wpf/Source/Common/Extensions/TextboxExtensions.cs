﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class TextboxExtensions
    {
        public static TextBox ResetValue(this TextBox thisTxtBox, bool force = false)
        {
            if (thisTxtBox == null)
                throw new ArgumentNullException(nameof(thisTxtBox));

            var text = thisTxtBox.Text;
            var tag = thisTxtBox.Tag;
            if (tag == null)
                return thisTxtBox;

            var placeholder = tag.ToString() ?? "...";
            if (text != placeholder && !string.IsNullOrWhiteSpace(text) && !force)
                return thisTxtBox;

            var currBg = ((SolidColorBrush)thisTxtBox.Foreground).Color;
            var newBrush = new SolidColorBrush(Color.FromArgb(128, currBg.R, currBg.G, currBg.B));

            thisTxtBox.FontStyle = FontStyles.Italic;
            thisTxtBox.Foreground = newBrush;
            thisTxtBox.Text = placeholder;
            return thisTxtBox;
        }

        public static TextBox ClearValue(this TextBox thisTxtBox, bool force = false)
        {
            if (thisTxtBox == null)
                throw new ArgumentNullException(nameof(thisTxtBox));

            var text = thisTxtBox.Text;
            var tag = thisTxtBox.Tag;
            if (tag == null)
                return thisTxtBox;

            var placeholder = tag.ToString();
            if (text != placeholder && !force)
                return thisTxtBox;

            var currBg = ((SolidColorBrush)thisTxtBox.Foreground).Color;
            var newBrush = new SolidColorBrush(Color.FromArgb(255, currBg.R, currBg.G, currBg.B));

            thisTxtBox.FontStyle = FontStyles.Normal;
            thisTxtBox.Foreground = newBrush;
            thisTxtBox.Text = string.Empty;
            return thisTxtBox;
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
    }
}
