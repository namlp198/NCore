﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Npc.Foundation.Helper
{
    /// <summary>
    /// VisualTreeHelper 확장
    /// </summary>
    public static class VisualTreeHelperEx
    {
        public static DependencyObject FindAncestorByType(DependencyObject element, Type type, bool specificTypeOnly)
        {
            if (element == null)
                return null;

            if (specificTypeOnly ? (element.GetType() == type)
                : (element.GetType() == type) || (element.GetType().IsSubclassOf(type)))
                return element;

            return VisualTreeHelperEx.FindAncestorByType(VisualTreeHelper.GetParent(element), type, specificTypeOnly);
        }

        public static DependencyObject FindAncestorByName(DependencyObject element, string name)
        {
            if (element == null)
                return null;

            if (element != null && (element is FrameworkElement) && (element as FrameworkElement).Name == name)
                return element;

            return VisualTreeHelperEx.FindAncestorByName(VisualTreeHelper.GetParent(element), name);
        }

        public static T FindAncestorByType<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                return default(T);
            }
            if (depObj is T)
            {
                return (T)depObj;
            }

            T parent = default(T);

            parent = VisualTreeHelperEx.FindAncestorByType<T>(VisualTreeHelper.GetParent(depObj));

            return parent;
        }

        public static Visual FindDescendantByName(Visual element, string name)
        {
            if (element != null && (element is FrameworkElement) && (element as FrameworkElement).Name == name)
                return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = VisualTreeHelperEx.FindDescendantByName(visual, name);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }

        public static Visual FindDescendantByType(Visual element, Type type)
        {
            return VisualTreeHelperEx.FindDescendantByType(element, type, true);
        }

        public static Visual FindDescendantByType(Visual element, Type type, bool specificTypeOnly)
        {
            if (element == null)
                return null;

            if (specificTypeOnly ? (element.GetType() == type)
                : (element.GetType() == type) || (element.GetType().IsSubclassOf(type)))
                return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = VisualTreeHelperEx.FindDescendantByType(visual, type, specificTypeOnly);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }

        public static T FindDescendantByType<T>(Visual element) where T : Visual
        {
            Visual temp = VisualTreeHelperEx.FindDescendantByType(element, typeof(T));

            return (T)temp;
        }

        public static Visual FindDescendantWithPropertyValue(Visual element,
            DependencyProperty dp, object value)
        {
            if (element == null)
                return null;

            if (element.GetValue(dp) == null)
            {
                return null;
            }

            if (element.GetValue(dp).Equals(value))
                return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = VisualTreeHelperEx.FindDescendantWithPropertyValue(visual, dp, value);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }
    }
}
