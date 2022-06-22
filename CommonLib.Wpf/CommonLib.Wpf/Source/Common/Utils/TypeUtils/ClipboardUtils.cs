using System;
using System.Text;
using System.Windows;
using CommonLib.Source.Common.Utils.UtilClasses;

namespace CommonLib.Wpf.Source.Common.Utils.TypeUtils
{
    public static class ClipboardUtils
    {
        private static string GetOpenClipboardWindowText()
        {
            return NativeMethods.GetClipBoardWindowText();
        }

        public static ActionStatus TrySetText(string text)
        {
            Exception lastEx = null;

            for (var i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(text);
                    return ActionStatus.Success();
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                }
            }

            var sbMessage = new StringBuilder();

            sbMessage.Append(lastEx?.Message);
            sbMessage.Append(Environment.NewLine);
            sbMessage.Append(Environment.NewLine);
            sbMessage.Append("Problem:");
            sbMessage.Append(Environment.NewLine);
            sbMessage.Append(GetOpenClipboardWindowText());

            return new ActionStatus(ErrorCode.CannotSetClipboardText, sbMessage.ToString());
        }
    }
}
