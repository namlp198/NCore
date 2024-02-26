
using Npc.Foundation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NpcCore.Wpf.Panels
{
    public class LogicalLocationCanvas : Canvas
    {
        #region AP
        private static void OnInvalideatePropertyChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dobj as UIElement;
            if (element != null)
            {
                var panel = VisualTreeHelperEx.FindAncestorByType<LogicalLocationCanvas>(element);
                if (panel != null)
                {
                    panel.LogicalUpdate();
                }
            }
        }

        public static readonly DependencyProperty OriginWidthProperty = DependencyProperty.RegisterAttached(
            "OriginWidth", typeof(double), typeof(LogicalLocationCanvas), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideatePropertyChanged)));

        public static void SetOriginWidth(UIElement element, double value)
        {
            element.SetValue(OriginWidthProperty, value);
        }
        public static double GetOriginWidth(UIElement element)
        {
            return (double)element.GetValue(OriginWidthProperty);
        }

        public static readonly DependencyProperty OriginHeightProperty = DependencyProperty.RegisterAttached(
            "OriginHeight", typeof(double), typeof(LogicalLocationCanvas), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnInvalideatePropertyChanged)));

        public static void SetOriginHeight(UIElement element, double value)
        {
            element.SetValue(OriginHeightProperty, value);
        }
        public static double GetOriginHeight(UIElement element)
        {
            return (double)element.GetValue(OriginHeightProperty);
        } 
        #endregion

        public double BaseWidth
        {
            get { return (double)GetValue(BaseWidthProperty); }
            set { SetValue(BaseWidthProperty, value); }
        }
        public static readonly DependencyProperty BaseWidthProperty =
            DependencyProperty.Register("BaseWidth", typeof(double), typeof(LogicalLocationCanvas), new PropertyMetadata(new PropertyChangedCallback(OnBaseWidthChanged)));

        private static void OnBaseWidthChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            LogicalLocationCanvas view = dobj as LogicalLocationCanvas;
            if (view != null && e.NewValue is double)
            {
                view.LogicalUpdate();
            }
        }

        public double BaseHeight
        {
            get { return (double)GetValue(BaseHeightProperty); }
            set { SetValue(BaseHeightProperty, value); }
        }
        public static readonly DependencyProperty BaseHeightProperty =
            DependencyProperty.Register("BaseHeight", typeof(double), typeof(LogicalLocationCanvas), new PropertyMetadata(new PropertyChangedCallback(OnBaseHeightChanged)));

        private static void OnBaseHeightChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            LogicalLocationCanvas view = dobj as LogicalLocationCanvas;
            if (view != null && e.NewValue is double)
            {
                view.LogicalUpdate();
            }
        }

        public LogicalLocationCanvas()
        {
            this.SizeChanged += LogicalLocationCanvas_SizeChanged;
        }

        private void LogicalLocationCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.LogicalUpdate();
            CurruntLogicalRatio = new Point(BaseWidth / this.ActualWidth, BaseHeight / this.ActualHeight);
        }

        public void LogicalUpdate()
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            double aWidth = constraint.Width;
            double aHeight = constraint.Height;

            double wr = aWidth / BaseWidth;
            double hr = aHeight / BaseHeight;

            foreach (FrameworkElement fe in this.Children)
            {
                double width = Convert.ToDouble(fe.GetValue(LogicalLocationCanvas.OriginWidthProperty));
                double height = Convert.ToDouble(fe.GetValue(LogicalLocationCanvas.OriginHeightProperty));

                if (double.IsInfinity(wr) == false && double.IsInfinity(hr) == false &&
                    double.IsInfinity(width) == false && double.IsInfinity(height) == false)
                {
                    fe.Width = width * wr;
                    fe.Height = height * hr;
                }
            }

            if (double.IsInfinity(constraint.Width) == true || double.IsInfinity(constraint.Height) == true)
            {
                return base.MeasureOverride(constraint);
            }
            else
            {
                return constraint;
            }
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //System.Diagnostics.Debug.WriteLine("Logical - ArrangeOverride Start");

            double aWidth = arrangeSize.Width;
            double aHeight = arrangeSize.Height;

            double wr = aWidth / BaseWidth;
            double hr = aHeight / BaseHeight;
            CurruntLogicalRatio = new Point(BaseWidth / aWidth, BaseHeight / aHeight);

            foreach (FrameworkElement fe in this.Children)
            {
                double x = Canvas.GetLeft(fe);
                double y = Canvas.GetTop(fe);

                if (double.IsNaN(x) == false && double.IsNaN(y) == false
                    && double.IsNaN(fe.Width) == false && double.IsNaN(fe.Height) == false)
                {
                    Rect ar = new Rect(
                        x * wr,
                        y * hr,
                        fe.Width,
                        fe.Height);

                    fe.Arrange(ar);
                    //Canvas.SetLeft(fe, ar.Left);
                    //Canvas.SetTop(fe, ar.Top);
                }
            }
            //sw.Stop();
            //System.Diagnostics.Debug.WriteLine("Logical - ArrangeOverride End : " + sw.Elapsed);
            return arrangeSize;
            //return base.ArrangeOverride(arrangeSize);
        }

        public Point CurruntLogicalRatio
        {
            get { return (Point)GetValue(CurruntLogicalRatioProperty); }
            set { SetValue(CurruntLogicalRatioProperty, value); }
        }
        public static readonly DependencyProperty CurruntLogicalRatioProperty =
            DependencyProperty.Register("CurruntLogicalRatio", typeof(Point), typeof(LogicalLocationCanvas));

    }
}
