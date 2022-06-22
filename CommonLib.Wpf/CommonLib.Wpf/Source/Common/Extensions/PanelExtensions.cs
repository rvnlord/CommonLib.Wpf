using System.Windows.Controls;
using CommonLib.Wpf.Source.Common.Utils;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class PanelExtensions
    {
        public static void ShowLoader(this Panel control)
        {
            WpfAsyncUtils.ShowLoader(control);
        }

        public static void HideLoader(this Panel control)
        {
            WpfAsyncUtils.HideLoader(control);
        }

        public static bool HasLoader(this Panel control)
        {
            return WpfAsyncUtils.HasLoader(control);
        }
    }
}
