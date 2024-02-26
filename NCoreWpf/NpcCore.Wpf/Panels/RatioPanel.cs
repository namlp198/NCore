
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
    public class RatioPanel : Canvas
    {
        #region Attatched Properties
        private static void OnInvalideateArrangePropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dobj as UIElement;
            if (element != null)
            {
                var panel = VisualTreeHelperEx.FindAncestorByType<RatioPanel>(element);
                if (panel != null)
                {
                    panel.InvalidateArrange();
                }
            }
        }

        public static readonly DependencyProperty RatioXProperty = DependencyProperty.RegisterAttached(
            "RatioX", typeof(double), typeof(RatioPanel), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideateArrangePropertyChanged)));

        public static void SetRatioX(UIElement element, double value)
        {
            element.SetValue(RatioXProperty, value);
        }
        public static double GetRatioX(UIElement element)
        {
            return (double)element.GetValue(RatioXProperty);
        }

        public static readonly DependencyProperty RatioYProperty = DependencyProperty.RegisterAttached(
            "RatioY", typeof(double), typeof(RatioPanel), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideateArrangePropertyChanged)));

        public static void SetRatioY(UIElement element, double value)
        {
            element.SetValue(RatioYProperty, value);
        }
        public static double GetRatioY(UIElement element)
        {
            return (double)element.GetValue(RatioYProperty);
        }

        public static readonly DependencyProperty RatioWidthProperty = DependencyProperty.RegisterAttached(
            "RatioWidth", typeof(double), typeof(RatioPanel), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideateArrangePropertyChanged)));

        public static void SetRatioWidth(UIElement element, double value)
        {
            element.SetValue(RatioWidthProperty, value);
        }
        public static double GetRatioWidth(UIElement element)
        {
            return (double)element.GetValue(RatioWidthProperty);
        }

        public static readonly DependencyProperty RatioHeightProperty = DependencyProperty.RegisterAttached(
            "RatioHeight", typeof(double), typeof(RatioPanel), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideateArrangePropertyChanged)));

        public static void SetRatioHeight(UIElement element, double value)
        {
            element.SetValue(RatioHeightProperty, value);
        }
        public static double GetRatioHeight(UIElement element)
        {
            return (double)element.GetValue(RatioHeightProperty);
        }


        //public bool IsFlipToHorizontal
        //{
        //    get { return (bool)GetValue(IsFlipToHorizontalProperty); }
        //    set { SetValue(IsFlipToHorizontalProperty, value); }
        //}
        //public static readonly DependencyProperty IsFlipToHorizontalProperty =
        //    DependencyProperty.Register("IsFlipToHorizontal", typeof(bool), typeof(RatioPanel), new PropertyMetadata(new PropertyChangedCallback(OnIsFlipToHorizontalChanged)));

        //private static void OnIsFlipToHorizontalChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        //{
        //    RatioPanel view = dobj as RatioPanel;
        //    if (view != null && e.NewValue is bool)
        //    {
        //        view.InvalidateArrange();
        //    }
        //}


        //public bool IsFlipToVertical
        //{
        //    get { return (bool)GetValue(IsFlipToVerticalProperty); }
        //    set { SetValue(IsFlipToVerticalProperty, value); }
        //}
        //public static readonly DependencyProperty IsFlipToVerticalProperty =
        //    DependencyProperty.Register("IsFlipToVertical", typeof(bool), typeof(RatioPanel), new PropertyMetadata(new PropertyChangedCallback(OnIsFlipToVerticalChanged)));

        //private static void OnIsFlipToVerticalChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        //{
        //    RatioPanel view = dobj as RatioPanel;
        //    if (view != null && e.NewValue is bool)
        //    {
        //        view.InvalidateArrange();
        //    }
        //}


        #endregion


        public RatioPanel()
        {
        }

        



        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (FrameworkElement fe in this.Children)
            {
                double rx = (double)fe.GetValue(RatioPanel.RatioXProperty);
                double ry = (double)fe.GetValue(RatioPanel.RatioYProperty);
                double rwidth = (double)fe.GetValue(RatioPanel.RatioWidthProperty);
                double rheight = (double)fe.GetValue(RatioPanel.RatioHeightProperty);

                //if (IsFlipToHorizontal == true)
                //{
                //    rx = (1.0d - rx) - rwidth;
                //}
                //if (IsFlipToVertical == true)
                //{
                //    ry = (1.0d - ry) - rheight;
                //}

                double x = (int)(arrangeSize.Width * rx);
                double y = (int)(arrangeSize.Height * ry);
                double w = Math.Max(0.0d, (int)(arrangeSize.Width * rwidth));
                double h = Math.Max(0.0d, (int)(arrangeSize.Height * rheight));

                fe.Width = w;
                fe.Height = h;
                fe.Measure(new Size(w, h));
                

                Canvas.SetLeft(fe, x);
                Canvas.SetTop(fe, y);

            }
            return base.ArrangeOverride(arrangeSize);

        }
    }
}
