using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace NpcCore.Wpf.Helpers
{
    public static class ExtensionHelper
    {
        /// <summary>
        /// Find ContentControl
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <returns>IEnumerable&lt;ContentControl&gt;.</returns>
        public static IEnumerable<ContentControl> FindContentControl(FrameworkElement frameworkElement)
        {
            return frameworkElement?.FindChildren<ContentControl>().Where(w => w.GetType() == typeof(ContentControl));
        }

        /// <summary>
        /// Finds the children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="forceUsingTheVisualTreeHelper">if set to <c>true</c> [force using the visual tree helper].</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject source, bool forceUsingTheVisualTreeHelper = false)
            where T : DependencyObject
        {
            if (source != null)
            {
                foreach (DependencyObject childObject in source.GetChildObjects(forceUsingTheVisualTreeHelper))
                {
                    DependencyObject child = childObject;
                    if (child is T)
                        yield return (T)child;
                    foreach (T child1 in child.FindChildren<T>(forceUsingTheVisualTreeHelper))
                        yield return child1;
                }
            }
        }

        /// <summary>
        /// Finds the children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        public static T FindChildren<T>(this DependencyObject source, string name)
            where T : DependencyObject
        {
            if (source != null)
            {
                return FindChildren<T>(source).FirstOrDefault(m => GetPropertyValue<string>(m, "Name") == name);
            }

            return null;
        }

        /// <summary>
        /// Gets the child objects.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="forceUsingTheVisualTreeHelper">if set to <c>true</c> [force using the visual tree helper].</param>
        /// <returns>IEnumerable&lt;DependencyObject&gt;.</returns>
        public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent, bool forceUsingTheVisualTreeHelper = false)
        {
            if (!forceUsingTheVisualTreeHelper && (parent is ContentElement || parent is FrameworkElement))
            {
                foreach (object child in LogicalTreeHelper.GetChildren(parent))
                {
                    if (child is DependencyObject obj)
                        yield return obj;
                }
            }
            else if (parent is Visual || parent is Visual3D)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; ++i)
                    yield return VisualTreeHelper.GetChild(parent, i);
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>T.</returns>
        public static T GetParent<T>(this DependencyObject dependencyObject) where T : class
        {
            DependencyObject target = dependencyObject;
            do
            {
                target = VisualTreeHelper.GetParent(target);
            }
            while (target != null && !(target is T));
            return target as T;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="name">The name.</param>
        /// <returns>T.</returns>
        public static T GetParent<T>(this DependencyObject dependencyObject, string name) where T : class
        {
            DependencyObject target = dependencyObject;
            do
            {
                target = VisualTreeHelper.GetParent(target);

                if (target is T && GetPropertyValue<string>(target, "Name") == name)
                    break;
            }
            while (target != null);

            return target as T;
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src">The source.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentNullException">propName</exception>
        public static T GetPropertyValue<T>(this object src, string propName)
        {
            if (src == null)
                return default(T);

            if (propName == null)
                throw new ArgumentNullException(nameof(propName));

            // [NCS-2695] CID 171170 Dereference null return value
            //return (T)src.GetType().GetProperty(propName).GetValue(src, null);
            return (T)src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">propName</exception>
        public static object GetPropertyValue(this object src, string propName)
        {
            if (src == null)
                return null;

            if (propName == null)
                throw new ArgumentNullException(nameof(propName));

            // [NCS-2695] CID 171197 Dereference null return value
            //return src.GetType().GetProperty(propName).GetValue(src, null);
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="propName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">propName</exception>
        public static void SetPropertyValue(object src, string propName, object value)
        {
            if (src == null)
                return;

            if (propName == null)
                throw new ArgumentNullException(nameof(propName));

            var propertyInfo = src.GetType().GetProperty(propName);
            if (propertyInfo != null)
                propertyInfo.SetValue(src, value);
        }

        /// <summary>
        /// Orders the by sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId">The type of the t identifier.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="order">The order.</param>
        /// <param name="idSelector">The identifier selector.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public static IEnumerable<T> OrderBySequence<T, TId>(
            this IEnumerable<T> source,
            IEnumerable<TId> order,
            Func<T, TId> idSelector)
        {
            var lookup = source.ToLookup(idSelector, t => t);
            foreach (var id in order)
            {
                foreach (var t in lookup[id])
                {
                    yield return t;
                }
            }
        }


        /// <summary>
        /// Gets the scale transform.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>ScaleTransform.</returns>
        public static ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform).Children.FirstOrDefault(tr => tr is ScaleTransform);
        }

        /// <summary>
        /// Gets the translate transform.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>TranslateTransform.</returns>
        public static TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform).Children.FirstOrDefault(tr => tr is TranslateTransform);
        }
    }
}
