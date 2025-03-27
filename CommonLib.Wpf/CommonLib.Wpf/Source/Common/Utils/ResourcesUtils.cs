using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Wpf.Source.Common.Utils
{
    public static class ResourcesUtils
    {
        public static System.Drawing.Bitmap GetIcon(string iconName = "NotifyIcon")
        {
            var assembly = Assembly.GetEntryAssembly() ?? throw new NullReferenceException("Entry Assembly is null");
            return (System.Drawing.Bitmap)(new ResourceManager($"{assembly.FullName.BeforeFirst(",")}.Properties.Resources", assembly).GetObject(iconName, CultureInfo.InvariantCulture));
        }
    }
}
