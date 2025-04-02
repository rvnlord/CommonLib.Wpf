using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Wpf.Source.Common.Utils;
using Infragistics.Windows.Editors;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class PanelExtensions
    {
        public static void ShowLoader(this Panel control, LoaderType loaderType = LoaderType.InfragisticsLoader)
        {
            WpfAsyncUtils.ShowLoader(control, loaderType);
        }

        public static void HideLoader(this Panel control)
        {
            WpfAsyncUtils.HideLoader(control);
        }

        public static bool HasLoader(this Panel control)
        {
            return WpfAsyncUtils.HasLoader(control);
        }

        public static async Task<Exception> AsyncWithLoader(this Panel loaderContainer, IEnumerable<object> objectsToDisable, Action action, LoaderType loaderType = LoaderType.InfragisticsLoader)
            => await WpfAsyncUtils.AsyncWithLoader(loaderContainer, objectsToDisable, action, loaderType);

        public static async Task<Exception> AsyncWithLoader(this Panel loaderContainer, IEnumerable<object> objectsToDisable, Func<Task> action, LoaderType loaderType = LoaderType.InfragisticsLoader)
            => await WpfAsyncUtils.AsyncWithLoader(loaderContainer, objectsToDisable, action, loaderType);

        public static async Task<Exception> AsyncWithLoader(this Panel loaderContainer, IEnumerable<object> objectsToDisable, Func<List<object>> action, LoaderType loaderType = LoaderType.InfragisticsLoader)
            => await WpfAsyncUtils.AsyncWithLoader(loaderContainer, objectsToDisable, action, loaderType);

        public static IEnumerable<FrameworkElement> InputControls(this Panel pnl) 
            => pnl.LogicalDescendants<ButtonBase, TextBoxBase, Selector, ValueEditor>();
    }
}
