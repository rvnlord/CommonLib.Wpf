using System.Windows.Controls;
using MoreLinq.Extensions;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class GridExtensions
    {
        public static void RefreshDataGrids(this Grid grid) => grid.LogicalDescendants<DataGrid>().ForEach(dg => dg.Refresh());
    }
}
