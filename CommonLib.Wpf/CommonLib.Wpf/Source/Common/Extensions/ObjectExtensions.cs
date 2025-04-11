using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Wpf.Source.Common.Utils;
using Control = System.Windows.Controls.Control;

namespace CommonLib.Wpf.Source.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static void AddEventHandlers(this object o, string eventName, List<Delegate> ehs)
        {
            WpfEventUtils.AddEventHandlers(o, eventName, ehs);
        }

        public static void AddEventHandlers(this object o, List<Delegate> ehs)
        {
            WpfEventUtils.AddEventHandlers(o, ehs);
        }

        public static List<Delegate> RemoveEventHandlers(this object o, string eventName)
        {
            return WpfEventUtils.RemoveEventHandlers(o, eventName);
        }

        public static Dictionary<T, List<Delegate>> RemoveEventHandlers<T>(this IEnumerable<T> controls, string eventName)
        {
            var listControls = controls as List<T> ?? controls.ToList(); 
            var dictHandlers = new Dictionary<T, List<Delegate>>();
            foreach (var control in listControls)
            {
                var eventHandlers = control.RemoveEventHandlers(eventName);
                if (eventHandlers.Any())
                    dictHandlers.Add(control, eventHandlers);
            }

            return dictHandlers;
        }

        public static void AddEventHandlers<T>(this IEnumerable<T> controls, string eventName, Dictionary<T, List<Delegate>> ehs)
        {
            var listControls = controls as List<T> ?? controls.ToList<T>();
            foreach (var control in listControls.Where(control => ehs.VorN(control) is not null))
                control.AddEventHandlers(eventName, ehs[control]);
        }

        public static void AddEventHandlers(this Dictionary<Control, List<Delegate>> ehs)
        {
            foreach (var (control, eventHandlers) in ehs)
                control.AddEventHandlers(eventHandlers);
        }
    }
}
