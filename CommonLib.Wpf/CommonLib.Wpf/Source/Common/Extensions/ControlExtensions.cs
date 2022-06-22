using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class ControlExtensions
    {
        public static void Position(this Control control, Point pos)
        {
            Canvas.SetLeft(control, pos.X);
            Canvas.SetTop(control, pos.Y);
        }

        public static Size Size(this Control control)
        {
            control.Refresh();
            return new Size(control.Width, control.Height);
        }

        private static async void AnimateChangeColor(this FrameworkElement control, Color color)
        {
            var colorAni = new ColorAnimation(color, new Duration(TimeSpan.FromMilliseconds(500)));
            await control.AnimateAsync("(Panel.Background).(SolidColorBrush.Color)", colorAni).ConfigureAwait(false);
        }

        public static void Highlight(this FrameworkElement control, Color color)
        {
            control.AnimateChangeColor(color);
        }
    }
}
