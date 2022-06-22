using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Wpf.Source.Common.Utils
{
    public static class WpfEventUtils
    {
        private static BindingFlags AllBindings => BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        private static readonly Dictionary<Type, List<FieldInfo>> _dicEventFieldInfos = new();

        public static void AddEventHandlers(object o, string eventName, List<Delegate> eventHandlers)
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));
            if (eventHandlers == null)
                throw new ArgumentNullException(nameof(eventHandlers));

            var ei = o.GetType().GetEvents().Single(e => e.Name == eventName);
            foreach (var eventHandler in eventHandlers)
                ei.AddEventHandler(o, eventHandler);
        }

        private static EventHandlerList GetStaticEventHandlerList(Type t, object obj)
        {
            var mi = t.GetMethod("get_Events", AllBindings);
            return (EventHandlerList)mi?.Invoke(obj, Array.Empty<object>());
        }

        private static IEnumerable<FieldInfo> GetTypeEventFields(Type t)
        {
            if (_dicEventFieldInfos.ContainsKey(t))
                return _dicEventFieldInfos[t];

            var lst = new List<FieldInfo>();
            BuildEventFields(t, lst);
            _dicEventFieldInfos.Add(t, lst);
            return lst;
        }

        private static void BuildEventFields(Type t, List<FieldInfo> lst)
        {
            var events = t.GetEvents(AllBindings);
            foreach (var ei in events)
            {
                var dt = ei.DeclaringType;
                var eventFields = dt?.GetFields(AllBindings).Where(f =>
                    f.Name.In(ei.Name + "Event", ei.Name) ||
                    f.FieldType.FullName?.ContainsAny($".{ei.Name}Args,", $".{ei.Name}EventArgs,") == true).ToArray();

                if (eventFields != null)
                    lst.AddRange(eventFields);
            }
        }

        public static List<Delegate> RemoveEventHandlers(object o, string EventName)
        {
            var delegates = new List<Delegate>();
            if (o == null) return delegates;

            var t = o.GetType();
            var eventFields = GetTypeEventFields(t);
            EventHandlerList static_event_handlers = null;

            foreach (var fi in eventFields)
            {
                if (EventName.IsNullOrWhiteSpace() || !EventName.Remove("Event").In(fi.Name.Remove("Event")) && fi.FieldType.FullName?.ContainsAny($".{EventName}Args,", $".{EventName}EventArgs,") != true)
                    continue;

                if (fi.FieldType == typeof(RoutedEvent))
                {
                    var wrt = fi.GetValue(o);
                    var EventHandlersStoreType = t.GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
                    var EventHandlersStore = EventHandlersStoreType?.GetValue(o, null);
                    var storeType = EventHandlersStore?.GetType();
                    var getRoutedEventHandlers = storeType?.GetMethod("GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public);
                    var Params = new[] { wrt };
                    var ret = (RoutedEventHandlerInfo[])getRoutedEventHandlers?.Invoke(EventHandlersStore, Params);
                    var ei = t.GetEvent(fi.Name[..^5], AllBindings);
                    if (ret == null) continue;
                    foreach (var routedEventHandlerInfo in ret)
                    {
                        delegates.Add(routedEventHandlerInfo.Handler);
                        ei?.RemoveEventHandler(o, routedEventHandlerInfo.Handler);
                    }

                }
                else if (fi.IsStatic)
                {
                    if (static_event_handlers == null)
                        static_event_handlers = GetStaticEventHandlerList(t, o);
                    var idx = fi.GetValue(o);
                    var eh = static_event_handlers[idx];
                    var dels = eh?.GetInvocationList();
                    if (dels == null) continue;
                    var ei = t.GetEvent(fi.Name, AllBindings);
                    foreach (var del in dels)
                    {
                        delegates.Add(del);
                        ei?.RemoveEventHandler(o, del);
                    }
                }
                else
                {
                    var ei = t.GetEvent(fi.Name, AllBindings);
                    if (ei == null) continue;
                    var val = fi.GetValue(o);
                    if (val is not Delegate mdel) continue;
                    foreach (var del in mdel.GetInvocationList())
                    {
                        delegates.Add(del);
                        ei.RemoveEventHandler(o, del);
                    }
                }
            }

            return delegates;
        }
    }
}
