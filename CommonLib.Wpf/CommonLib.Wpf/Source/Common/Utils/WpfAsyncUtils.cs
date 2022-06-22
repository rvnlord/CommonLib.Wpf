using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils.TypeUtils;
using CommonLib.Wpf.Source.Common.Extensions;
using CommonLib.Wpf.Source.Common.Extensions.Collections;
using Infragistics.Controls.Interactions;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MoreLinq;

namespace CommonLib.Wpf.Source.Common.Utils
{
    public static class WpfAsyncUtils
    {
        public static async Task<Exception> AsyncWithLoader(Panel loaderContainer, IReadOnlyCollection<object> objectsToDisable, Action action)
        {
            return await Task.Run(async () =>
            {
                var wnd = loaderContainer.LogicalAncestor<MetroWindow>();
                var dispatcher = wnd.Dispatcher;
                if (dispatcher == null)
                    throw new NullReferenceException(nameof(dispatcher));
                var actuallyDisabledControls = objectsToDisable;
                Exception exception = null;

                dispatcher.Invoke(() =>
                {
                    if (objectsToDisable != null && objectsToDisable.Any())
                        objectsToDisable.DisableControls();
                    loaderContainer.ShowLoader();
                });

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exception = ex;
                    await dispatcher.Invoke(async () => await wnd.ShowMessageAsync("Error occured", ex.Message).ConfigureAwait(false)).ConfigureAwait(false);
                }
                finally
                {
                    dispatcher.Invoke(() =>
                    {
                        loaderContainer.HideLoader();
                        var gridMain = wnd.LogicalDescendants<Grid>().Single(g => g.Name == "gridMain");
                        if (gridMain.HasLoader()) return;

                        if (actuallyDisabledControls != null && actuallyDisabledControls.Any())
                            actuallyDisabledControls.EnableControls();
                    });
                }

                return exception;
            }).ConfigureAwait(false);
        }

        public static Task AsyncWithLoader(Panel loaderContainer, IReadOnlyCollection<object> objectsToDisable, Func<List<object>> action)
        {
            var task = Task.Run(async () =>
            {
                var wnd = loaderContainer.LogicalAncestor<MetroWindow>();
                var dispatcher = wnd.Dispatcher;
                if (dispatcher == null)
                    throw new NullReferenceException(nameof(dispatcher));

                var actuallyDisabledControls = objectsToDisable;
                dispatcher.Invoke(() =>
                {
                    if (objectsToDisable != null && objectsToDisable.Any())
                        objectsToDisable.DisableControls();
                    loaderContainer.ShowLoader();
                });

                try
                {
                    actuallyDisabledControls = action();
                }
                catch (Exception ex)
                {
                    await dispatcher.Invoke(async () => await wnd.ShowMessageAsync("Error occured", ex.Message).ConfigureAwait(false)).ConfigureAwait(false);
                }
                finally
                {
                    dispatcher.Invoke(() =>
                    {
                        loaderContainer.HideLoader();
                        var gridMain = wnd.LogicalDescendants<Grid>().Single(g => g.Name == "gridMain");
                        if (gridMain.HasLoader()) return;

                        if (actuallyDisabledControls != null && actuallyDisabledControls.Any())
                            actuallyDisabledControls.EnableControls();
                    });
                }
            });
            return task;
        }

        public static void ShowLoader(Panel control, LoaderType loaderType = LoaderType.InfragisticsLoader)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            var rect = new Rectangle
            {
                Margin = new Thickness(0),
                Fill = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
                Name = "prLoaderContainer"
            };

            var loader = loaderType switch
            {
                LoaderType.InfragisticsLoader => (FrameworkElement) new XamBusyIndicator
                {
                    Name = "prLoader",
                },
                LoaderType.MahAppsLoader => new ProgressRing
                {
                    Foreground = (Brush)control.LogicalAncestor<Window>().FindResource("AccentColorBrush"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 80,
                    Height = 80,
                    IsActive = true,
                    Name = "prLoader"
                },
                _ => throw new ArgumentOutOfRangeException(nameof(loaderType), loaderType, null)
            };

            var status = new TextBlock
            {
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 14,
                Margin = new Thickness(0, 125, 0, 0),
                Text = "Loading...",
                Name = "prLoaderStatus"
            };

            var zIndex = MoreEnumerable.Append(control.LogicalDescendants<FrameworkElement>(), control).MaxBy_(Panel.GetZIndex).First().ZIndex();
            Panel.SetZIndex(rect, zIndex + 1);
            //if (loader != null)
            Panel.SetZIndex(loader, zIndex + 1);
            Panel.SetZIndex(status, zIndex + 1);

            control.Children.AddRange(new[] { rect, loader, status });
        }

        public static void HideLoader(Panel control)
        {
            var loaders = control.LogicalDescendants<FrameworkElement>().Where(c => c.Name == "prLoader").ToArray();
            var loaderContainers = control.LogicalDescendants<FrameworkElement>().Where(c => c.Name == "prLoaderContainer").ToArray();
            var loaderStatuses = control.LogicalDescendants<FrameworkElement>().Where(c => c.Name == "prLoaderStatus").ToArray();
            var loaderParts = ArrayUtils.ConcatMany(loaders, loaderContainers, loaderStatuses);

            loaderParts.ForEach(lp => control.Children.Remove(lp));
        }

        public static bool HasLoader(Panel control)
        {
            return control.LogicalDescendants<Rectangle>().Any(r => r.Name == "prLoaderContainer");
        }

        public static LoaderSpinnerWrapper GetLoader(Window wnd)
        {
            if (wnd == null)
                throw new ArgumentNullException(nameof(wnd));

            return wnd.Dispatcher?.Invoke(() =>
            {
                return new LoaderSpinnerWrapper
                {
                    LoaderControl = wnd.LogicalDescendants<FrameworkElement>().First(c => c.Name == "prLoader"),
                    LoaderStatus = wnd.LogicalDescendants<TextBlock>().First(c => c.Name == "prLoaderStatus")
                };
            });
        }

        public static void SetLoaderStatus(Window wnd, string status)
        {
            if (wnd == null)
                throw new ArgumentNullException(nameof(wnd));

            var tbLoaderStatus = wnd.GetLoader().LoaderStatus;
            wnd.Dispatcher?.Invoke(() =>
            {
                tbLoaderStatus.Text = status;
            });
        }
    }

    public class LoaderSpinnerWrapper
    {
        public FrameworkElement LoaderControl { get; set; }
        public TextBlock LoaderStatus { get; set; }
    }

    public enum LoaderType
    {
        InfragisticsLoader,
        MahAppsLoader
    }
}
