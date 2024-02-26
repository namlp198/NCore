
using Npc.Foundation.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NpcCore.Wpf.Panels
{
    public class RatioLocationPanel : Canvas
    {
        #region Attatched Properties
        private static void OnInvalideateArrangePropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dobj as UIElement;
            if (element != null)
            {
                var panel = VisualTreeHelperEx.FindAncestorByType<RatioLocationPanel>(element);
                if (panel != null)
                {
                    panel.InvalidateArrange();
                }
            }
        }

        public static readonly DependencyProperty RatioXProperty = DependencyProperty.RegisterAttached(
            "RatioX", typeof(double), typeof(RatioLocationPanel), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideateArrangePropertyChanged)));

        public static void SetRatioX(UIElement element, double value)
        {
            element.SetValue(RatioXProperty, value);
        }
        public static double GetRatioX(UIElement element)
        {
            return (double)element.GetValue(RatioXProperty);
        }

        public static readonly DependencyProperty RatioYProperty = DependencyProperty.RegisterAttached(
            "RatioY", typeof(double), typeof(RatioLocationPanel), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideateArrangePropertyChanged)));

        public static void SetRatioY(UIElement element, double value)
        {
            element.SetValue(RatioYProperty, value);
        }
        public static double GetRatioY(UIElement element)
        {
            return (double)element.GetValue(RatioYProperty);
        }
        #endregion


        public RatioLocationPanel()
        {
        }

        



        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (FrameworkElement fe in this.Children)
            {
                double rx = (double)fe.GetValue(RatioLocationPanel.RatioXProperty);
                double ry = (double)fe.GetValue(RatioLocationPanel.RatioYProperty);

                double x = arrangeSize.Width * rx;
                double y = arrangeSize.Height * ry;

                Canvas.SetLeft(fe, x);
                Canvas.SetTop(fe, y);

            }
            return base.ArrangeOverride(arrangeSize);

        }
    }
}
