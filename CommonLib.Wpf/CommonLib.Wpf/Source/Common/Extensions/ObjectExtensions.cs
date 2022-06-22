using System;
using System.Collections.Generic;
using CommonLib.Wpf.Source.Common.Utils;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static void AddEventHandlers(this object o, string eventName, List<Delegate> ehs)
        {
            WpfEventUtils.AddEventHandlers(o, eventName, ehs);
        }

        public static List<Delegate> RemoveEventHandlers(this object o, string eventName)
        {
            return WpfEventUtils.RemoveEventHandlers(o, eventName);
        }
    }
}
