using System;
using System.Windows;
using CommonLib.Wpf.Source.Common.Utils;
using CommonLib.Wpf.Source.Common.Utils.UtilClasses;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class WindowExtensions
    {
        //public static Window CenterOnScreen(this Window wnd)
        //{
        //    wnd.Position(PointUtils.CenteredWindowTopLeft(wnd.Size()));
        //    return wnd;
        //}

        public static WPFScreen Screen(this Window wnd)
        {
            return WPFScreen.GetScreenFrom(wnd);
        }

        public static Point WindowPointToScreen(this Window wnd, Point p)
        {
            if (wnd == null)
                throw new ArgumentNullException(nameof(wnd));

            var screenMousePos = wnd.PointToScreen(p);
            var screen = wnd.Screen().DeviceBounds;
            var screenWidth = screen.Width;
            var screenHeight = screen.Height;
            var screenDPIWidth = SystemParameters.FullPrimaryScreenWidth;
            var screenDPIHeight = SystemParameters.FullPrimaryScreenHeight;

            return new Point(
                screenDPIWidth / (screenWidth / screenMousePos.X),
                screenDPIHeight / (screenHeight / screenMousePos.Y));
        }

        public static Window SizeToContentAndUnlock(this Window wnd)
        {
            if (wnd == null)
                throw new ArgumentNullException(nameof(wnd));

            wnd.SizeToContent = SizeToContent.WidthAndHeight;
            wnd.SizeToContent = SizeToContent.Manual;
            return wnd;
        }

        public static Window SizeToContentAndKeep(this Window wnd)
        {
            if (wnd == null)
                throw new ArgumentNullException(nameof(wnd));

            wnd.SizeToContent = SizeToContent.WidthAndHeight;
            return wnd;
        }

        public static LoaderSpinnerWrapper GetLoader(this Window wnd)
        {
            return WpfAsyncUtils.GetLoader(wnd);
        }

        public static void SetLoaderStatus(this Window wnd, string status)
        {
            WpfAsyncUtils.SetLoaderStatus(wnd, status);
        }
    }
}
