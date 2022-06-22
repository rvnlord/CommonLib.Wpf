using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommonLib.Source.Common.Extensions.Collections;

namespace CommonLib.Wpf.Source.Common.Extensions.Collections
{
    public static class UIElementCollectionExtensions
    {
        public static UIElementCollection ReplaceAll<T>(this UIElementCollection col, IEnumerable<T> en) where T : UIElement
        {
            var list = en.ToList();
            col.RemoveAll();
            col.AddRange(list);
            return col;
        }

        public static void RemoveAll(this UIElementCollection collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            while (collection.Count != 0)
                collection.RemoveAt(0);
        }

        public static void PrepandAllBefore(this UIElementCollection existingUiElements, IEnumerable<UIElement> newUiElements, UIElement existingUiElement)
        {
            if (existingUiElements == null)
                throw new ArgumentNullException(nameof(existingUiElements));
            if (newUiElements == null)
                throw new ArgumentNullException(nameof(newUiElements));

            existingUiElements.Remove(existingUiElement);
            foreach (var newUiElement in newUiElements)
                existingUiElements.Add(newUiElement);
            existingUiElements.Add(existingUiElement);
        }

        public static void PrepandAllBefore(this UIElementCollection existingUiElements, IEnumerable<UIElement> newUiElements, IEnumerable<UIElement> beforeUiElements)
        {
            if (existingUiElements == null)
                throw new ArgumentNullException(nameof(existingUiElements));
            if (newUiElements == null)
                throw new ArgumentNullException(nameof(newUiElements));

            var arrBeforeUiElements = beforeUiElements.ToArray();
            existingUiElements.RemoveRange(arrBeforeUiElements);
            foreach (var newUiElement in newUiElements)
                existingUiElements.Add(newUiElement);
            existingUiElements.AddRange(arrBeforeUiElements);
        }
    }
}
