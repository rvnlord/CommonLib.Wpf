using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonLib.Wpf.Source.Common.Utils
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(int hwnd, StringBuilder text, int count);

        public static string GetClipBoardWindowText()
        {
            var sb = new StringBuilder(501);
            var hwnd = GetOpenClipboardWindow();
            _ = GetWindowText(hwnd.ToInt32(), sb, 500);
            return sb.ToString();
        }
    }
}
