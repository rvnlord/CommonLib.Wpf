
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class LabelExtensions
    {
        public static Label ClearValue(this Label lbl)
        {
            var textBlocks = lbl.LogicalDescendants<TextBlock>().ToArray();
            if (textBlocks.Length == 1)
                textBlocks[0].Text = string.Empty;
            else
                lbl.Content = string.Empty;
            return lbl;
        }

        public static IEnumerable<Label> ClearValues(this IEnumerable<Label> lbls)
        {
            var arrLbls = lbls as Label[] ?? lbls.ToArray();
            foreach (var lbl in arrLbls)
                lbl.ClearValue();
            return arrLbls;
        }
    }
}
