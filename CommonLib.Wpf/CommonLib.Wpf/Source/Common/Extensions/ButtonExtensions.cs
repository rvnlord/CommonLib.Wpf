using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommonLib.Source.Common.Extensions;
using CommonLib.Wpf.Source.Common.Utils.TypeUtils;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class ButtonExtensions
    {
        public static Button BindCopyButton(this Button btn)
        {
            btn.Click -= BtnCopy_Click;
            btn.Click += BtnCopy_Click;
            return btn;
        }

        public static IEnumerable<Button> BindCopyButtons(this IEnumerable<Button> btns)
        {
            var arrBtns = btns as Button[] ?? btns.ToArray();
            foreach (var btn in arrBtns)
                btn.BindCopyButton();
            return arrBtns;
        }

        private static void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var txt = btn.LogicalAncestor<Window>().LogicalDescendants<TextBox>().Single(txt => txt.Name.After("txt").EqualsInvariant(btn.Name.After("btnCopy")));
            ClipboardUtils.TrySetText(txt.IsNullWhiteSpaceOrTag() ? string.Empty : txt.Text);
        }
    }
}
