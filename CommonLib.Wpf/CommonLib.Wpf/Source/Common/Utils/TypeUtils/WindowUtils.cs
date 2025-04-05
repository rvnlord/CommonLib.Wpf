using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using CommonLib.Source.Common.Extensions;
using CommonLib.Wpf.Source.Common.Extensions;
using MahApps.Metro.Controls;
using MoreLinq.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static CommonLib.Wpf.Source.Common.Utils.WpfAsyncUtils;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;
using Label = System.Windows.Controls.Label;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace CommonLib.Wpf.Source.Common.Utils.TypeUtils
{
    public static class WindowUtils
    {
        private static NotifyIcon _notifyIcon;
        private static bool _restoreForDragMove;
        private static MetroWindow _window;

        public static MetroWindow WindowOf(object o) => _window ??= ((DependencyObject)o).Window();

        public static void InitializeCommonComponents(this MetroWindow window, Bitmap notifyIcon)
        {
            var btnMinimizeToTray = window.LogicalDescendants<Button>().Single(c => c.Name.EqualsInvariant("btnMinimizeToTray"));
            btnMinimizeToTray.Click += BtnMinimizeToTray_Click;
            btnMinimizeToTray.MouseEnter += BtnMinimizeToTray_MouseEnter;
            btnMinimizeToTray.MouseLeave += BtnMinimizeToTray_MouseLeave;

            var btnMinimize = window.LogicalDescendants<Button>().Single(c => c.Name.EqualsInvariant("btnMinimize"));
            btnMinimize.Click += BtnMinimize_Click;
            btnMinimize.MouseEnter += BtnMinimize_MouseEnter;
            btnMinimize.MouseLeave += BtnMinimize_MouseLeave;

            var btnClose = window.LogicalDescendants<Button>().Single(c => c.Name.EqualsInvariant("btnClose"));
            btnClose.Click += BtnClose_Click;
            btnClose.MouseEnter += BtnClose_MouseEnter;
            btnClose.MouseLeave += BtnClose_MouseLeave;

            var gridTitleBar = window.LogicalDescendants<Grid>().Single(c => c.Name.EqualsInvariant("gridTitleBar"));
            gridTitleBar.MouseLeftButtonDown += GridTitleBar_MouseLeftButtonDown;
            gridTitleBar.MouseLeftButtonUp += GridTitleBar_MouseLeftButtonUp;
            gridTitleBar.MouseEnter += GridTitleBar_MouseEnter;
            gridTitleBar.MouseLeave += GridTitleBar_MouseLeave;
            gridTitleBar.MouseMove += GridTitleBar_MouseMove;
            
            var iconHandle = notifyIcon.EnsureWin7().GetHicon();
            var icon = Icon.FromHandle(iconHandle);
            
            _notifyIcon = new NotifyIcon
            {
                BalloonTipTitle = window.LogicalDescendants<Label>().Single(lbl => lbl.Name.EqualsInvariant("lblWindowTitle")).Content.ToString() ?? "",
                BalloonTipText = @"is hidden here",
                Icon = icon
            };
            _notifyIcon.Click += NotifyIcon_Click;
        }

        public static void BtnMinimizeToTray_Click(object sender, RoutedEventArgs e)
        {
            var window = WindowOf(sender);
            window.WindowState = WindowState.Minimized;
            window.ShowInTaskbar = false;
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(1500);
        }

        public static void BtnMinimizeToTray_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Color.FromRgb(0, 0, 180));
        }

        public static void BtnMinimizeToTray_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = Brushes.Transparent;
        }

        public static void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var window = WindowOf(sender);
            window.WindowState = WindowState.Minimized;
        }

        public static void BtnMinimize_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Color.FromRgb(76, 76, 76));
        }

        public static void BtnMinimize_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = Brushes.Transparent;
        }

        public static void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = WindowOf(sender);
            window.Close();
        }

        public static void BtnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Color.FromRgb(76, 76, 76));
            ((Button)sender).Foreground = Brushes.Black;
        }

        public static void BtnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = Brushes.Transparent;
            ((Button)sender).Foreground = Brushes.White;
        }

        public static void GridTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = WindowOf(sender);
            if (e.ClickCount == 2)
            {
                if (window.ResizeMode != ResizeMode.CanResize && window.ResizeMode != ResizeMode.CanResizeWithGrip)
                    return;

                window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
            else
            {
                _restoreForDragMove = window.WindowState == WindowState.Maximized;
                window.DragMove();
            }
        }

        public static void GridTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_restoreForDragMove || e.LeftButton != MouseButtonState.Pressed) 
                return;
            if (!OperatingSystem.IsWindowsVersionAtLeast(7))
                throw new PlatformNotSupportedException();

            var window = WindowOf(sender);
            _restoreForDragMove = false;

            var wndMousePos = e.MouseDevice.GetPosition(window);
            var screenMousePos = window.WindowPointToScreen(wndMousePos);

            window.Left = screenMousePos.X - window.Width / (window.ActualWidth / wndMousePos.X);
            window.Top = screenMousePos.Y - window.Height / (window.ActualHeight / wndMousePos.Y);

            window.WindowState = WindowState.Normal;

            window.DragMove();
        }

        public static void GridTitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _restoreForDragMove = false;
        }

        public static void GridTitleBar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(7))
                throw new PlatformNotSupportedException();
            var window = WindowOf(sender);
            ((Grid)sender).Highlight(((SolidColorBrush)window.FindResource("MouseOverTitleBarBrush")).Color);
        }

        public static void GridTitleBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(7))
                throw new PlatformNotSupportedException();
            var window = WindowOf(sender);
            ((Grid)sender).Highlight(((SolidColorBrush)window.FindResource("DefaultWindowBrush")).Color);
        }

        public static void NotifyIcon_Click(object sender, EventArgs e)
        {
            var window = WindowOf(sender); // here sender is "Notify Icon" not "Dependency Object / Control" like everywhere else but I am trying to be clever because at this point "_window" field should be initialized by something else anyway so method shouldn't throw
            window.ShowInTaskbar = true;
            _notifyIcon.Visible = false;
            window.WindowState = WindowState.Normal;

            if (window.IsVisible)
                window.Activate();
            else
                window.Show();
        }

        public static void InitializeTextBoxPlaceholders(this Window window)
        {
            window.LogicalDescendants<TextBox>().InitializeTextBoxPlaceholders();
        }

        public static void BindCopyButtons(this Window window)
        {
            var btnsCopy = window.LogicalDescendants<Button>().Where(b => b.Name.StartsWith("btnCopy")).ToArray();
            btnsCopy.BindCopyButtons();
        }
        
        public static Brush GetInputDisabledBackgroundBrush(this Window window) => DispatchIfNeeded(() => (Brush)window.FindResource("InputDisabledBackgroundBrush"));
        public static Brush GetValidBackgroundBrush(this Window window) => DispatchIfNeeded(() => (Brush)window.FindResource("ValidBackgroundBrush"));
        public static Brush GetInvalidBackgroundBrush(this Window window) => DispatchIfNeeded(() => (Brush)window.FindResource("InvalidBackgroundBrush"));
        public static Brush GetValidForegroundBrush(this Window window) => DispatchIfNeeded(() => (Brush)window.FindResource("ValidForegroundBrush"));
        public static Brush GetInvalidForegroundBrush(this Window window) => DispatchIfNeeded(() => (Brush)window.FindResource("InvalidForegroundBrush"));
        public static Brush GetDefaultTextBoxBackgroundBrush(this Window window) => DispatchIfNeeded(() => (Brush)window.FindResource("TextBoxBackgroundBrush"));
        public static Brush GetDefaultLabelForegroundBrush(this Window window) => DispatchIfNeeded(() => (Brush)window.FindResource("TextBoxBackgroundBrush"));
    }
}
