﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommonLib.Source.Common.Extensions;
using CommonLib.Source.Common.Extensions.Collections;
using CommonLib.Source.Common.Utils.UtilClasses;
using CommonLib.Wpf.Source.Common.Extensions;
using MoreLinq;
using Org.BouncyCastle.Math.EC;

namespace CommonLib.Wpf.Source.Common.Utils
{
    public static class DeferredControlsUpdateUtils
    {
        public static Dictionary<object, EnqueuedSetValue> UpdateQueue { get; } = new Dictionary<dynamic, EnqueuedSetValue>();

        public static EnqueuedSetValue EnqueueSetValue<TControl, TValue>(TControl control, TValue value)
        {
            var ev = new EnqueuedSetValue { Control = control, Value = value };
            UpdateQueue[control] = ev;
            return ev;
        }

        public static void UpdateControlValuesWithGridVisibility()
        {
            foreach (var ev in UpdateQueue.Values.ToArray())
            {
                if (ev.Grid != null && ev.Grid.Visibility == Visibility.Visible)
                    ev.UpdateControlValue();
            }
        }
    }

    public class EnqueuedSetValue
    {
        public object Control { get; set; }
        public object Value { get; set; }
        public Grid Grid { get; set; }

        public void OrSetDirectlyIfGridVisible(Grid grid)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            if (grid.Visibility != Visibility.Visible)
                Grid = grid;
            else
                UpdateControlValue();
        }

        public void UpdateControlValue()
        {
            if (Control is TextBox textBox)
            {
                textBox.Text = Value.ToString();
            }
            else if (Control.GetType().IsObservableCollectionType())
            {
                var elType = Control.GetType().GetGenericArguments()[0];
                var ocType = Control.GetType().GetGenericTypeDefinition();
                var constructedOcType = ocType.MakeGenericType(elType);
                var valuesType = Value.GetType().GetGenericTypeDefinition();
                var constructedListType = valuesType.MakeGenericType(elType);
                var oc = Control.CastToReflected(constructedOcType);
                var values = Value.CastToReflected(constructedListType);
                ObservableCollectionExtensions.ReplaceAll(oc, values);

                if (Grid != null)
                {
                    var grids = Grid.LogicalDescendants<DataGrid>().ToArray();
                    grids.ForEach(dg => dg.ScrollToStart());
                }
            }
            else if (Control is ComboBox ddl)
            {
                var dummyItem = (DdlItem) typeof(ECCurve.Config).GetFields().Single(f => f.Name.StartsWithInvariant($"{ddl.Name}_DUMMY_")).GetValue(null);
                ddl.ItemsSource = ((string[]) Value).Select((item, i) => new DdlItem(i + 1, item)).PrependEl(dummyItem).ToArray();
                ddl.SelectById(-1);
            }

            DeferredControlsUpdateUtils.UpdateQueue.Remove(Control); // 'Control' is key of 'this' 'EnqueuedValue' object
        }
    }
}
