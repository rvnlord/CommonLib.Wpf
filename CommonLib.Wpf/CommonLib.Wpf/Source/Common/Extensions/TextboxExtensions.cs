using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonLib.Source.Common.Extensions;
using CommonLib.Wpf.Source.Common.Utils.TypeUtils;
using MoreLinq.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static CommonLib.Wpf.Source.Common.Utils.WpfAsyncUtils;
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

            var disp = txt.LogicalAncestor<Window>().Dispatcher;
            var text = DispatchIfNeeded(() => txt.Text);
            var tag = DispatchIfNeeded(() => txt.Tag);
            if (tag is null)
                return txt;

            var placeholder = tag.ToString() ?? "...";
            if (text != placeholder && !string.IsNullOrWhiteSpace(text) && !force)
                return txt;

            var bg = DispatchIfNeeded(() => ((SolidColorBrush)txt.Foreground).Color);
            var newBrush = new SolidColorBrush(Color.FromArgb(128, bg.R, bg.G, bg.B));

            DispatchIfNeeded(() =>
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

            var disp = txt.LogicalAncestor<Window>().Dispatcher;
            var text = DispatchIfNeeded(() => txt.Text);
            var tag = DispatchIfNeeded(() => txt.Tag);
            if (tag is null)
                return txt;

            var placeholder = tag.ToString();
            if (text != placeholder && !force)
                return txt;

            var bg = DispatchIfNeeded(() => ((SolidColorBrush)txt.Foreground).Color);
            var newBrush = new SolidColorBrush(Color.FromArgb(255, bg.R, bg.G, bg.B));

            DispatchIfNeeded(() =>
            {
                var textChangedHandlers = txt.RemoveEventHandlers("TextChanged");

                txt.FontStyle = FontStyles.Normal;
                txt.Foreground = newBrush;
                txt.Text = string.Empty;

                txt.AddEventHandlers("TextChanged", textChangedHandlers);
            });

            return txt;
        }

        public static TextBox SetValue(this TextBox txt, string value)
        {
            if (value.IsNullOrWhiteSpace())
                return txt.IsFocused ? txt.ClearValue(true) : txt.ResetValue(true);

            txt.ClearValue(true).Text = value;
            txt.CaretIndex = value?.Length ?? 0;
            return txt;
        }

        public static TextBox SetText(this TextBox txt, string value) => txt.SetValue(value);

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

        public static bool IsNullWhiteSpaceOrTag(this TextBox txt)
        {
            if (txt is null)
                throw new ArgumentNullException(nameof(txt));
            
            return DispatchIfNeeded(() => txt.Text.IsNullWhiteSpaceOrDefault(txt.Tag?.ToString() ?? ""));
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

        public static TextBox SetBackground(this TextBox txt, ControlState background)
        {
            var wnd = txt.LogicalAncestor<Window>();
            if (txt.IsReadOnly) // || !txt.IsEnabled) // for now ignoring disabled as disabled state is temporary, i.e.: when the loader is visible. I don't really validatee disabled controls either and if they are in disabled state, re-enabling them would cause the background setting (set in this method) to work anyway.
            {
                var readOnlyBorder = (Border)txt.Template.FindName("ReadOnlyVisualElement", txt);
                readOnlyBorder.Background = background switch
                {
                    ControlState.Default => wnd.GetInputDisabledBackgroundBrush(),
                    ControlState.Valid => wnd.GetValidBackgroundBrush(),
                    ControlState.Invalid => wnd.GetInvalidBackgroundBrush(),
                    _ => throw new ArgumentOutOfRangeException(nameof(background), background, null)
                };
            }
            else
            {
                txt.Background = background switch
                {
                    ControlState.Default => wnd.GetDefaultTextBoxBackgroundBrush(),
                    ControlState.Valid => wnd.GetValidBackgroundBrush(),
                    ControlState.Invalid => wnd.GetInvalidBackgroundBrush(),
                    _ => throw new ArgumentOutOfRangeException(nameof(background), background, null)
                };
            }
            return txt;
        }

        public static TextBox SetDefaultBackground(this TextBox txt) => txt.SetBackground(ControlState.Default);
        public static TextBox SetValidBackground(this TextBox txt) => txt.SetBackground(ControlState.Valid);
        public static TextBox SetInvalidBackground(this TextBox txt) => txt.SetBackground(ControlState.Invalid);

        public static IEnumerable<TextBox> SetBackground(this IEnumerable<TextBox> txts, ControlState background)
        {
            var arrTxts = txts as TextBox[] ?? txts.ToArray();
            arrTxts.ForEach(txt => txt.SetBackground(background));
            return arrTxts;
        }

        public static IEnumerable<TextBox> SetDefaultBackground(this IEnumerable<TextBox> txt) => txt.SetBackground(ControlState.Default);
        public static IEnumerable<TextBox> SetValidBackground(this IEnumerable<TextBox> txt) => txt.SetBackground(ControlState.Valid);
        public static IEnumerable<TextBox> SetInvalidBackground(this IEnumerable<TextBox> txt) => txt.SetBackground(ControlState.Invalid);
    }
}
