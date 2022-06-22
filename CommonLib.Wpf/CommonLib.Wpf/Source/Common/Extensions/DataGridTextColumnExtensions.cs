using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class DataGridTextColumnExtensions
    {
        public static string DataMemberName(this DataGridTextColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            return ((Binding)column.Binding).Path.Path;
        }
    }
}
