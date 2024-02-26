using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NpcCore.Wpf.Panels
{
    public class ZoomGridGuiderControl : Panel
    {

        public double GridSize
        {
            get { return (double)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }
        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register("GridSize", typeof(double), typeof(ZoomGridGuiderControl), new PropertyMetadata(20d, new PropertyChangedCallback(OnGridSizeChanged)));

        private static void OnGridSizeChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            ZoomGridGuiderControl view = dobj as ZoomGridGuiderControl;
            if (view != null && e.NewValue is double)
            {
                //view.InvalidateVisual();
            }
        }


        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom", typeof(double), typeof(ZoomGridGuiderControl), new PropertyMetadata(1.0d, new PropertyChangedCallback(OnZoomChanged)));

        private static void OnZoomChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            ZoomGridGuiderControl view = dobj as ZoomGridGuiderControl;
            if (view != null && e.NewValue is double)
            {
                //view.InvalidateVisual();
            }
        }


        public ZoomGridGuiderControl()
        {
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            //this.UseLayoutRounding = true;
            this.SnapsToDevicePixels = true;
            this.IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext dc)
        {
            Debug.WriteLine("GridGuide - Rendered");
            base.OnRender(dc);

            double unitSize = GridSize;

            bool isZoomOver = this.Zoom > 8d;

            double zoomedUnitSize = unitSize * Math.Min(8d, Zoom);



            if (zoomedUnitSize > unitSize * 4 && isZoomOver == false)
            {
                zoomedUnitSize = unitSize + (zoomedUnitSize % unitSize * 4);
            }

            if (this.Zoom < 1.0d)
            {
                zoomedUnitSize = unitSize;
            }

            double bigSize = zoomedUnitSize * 12;
            double middleSize = zoomedUnitSize * 8;
            double smallSize = zoomedUnitSize;

            Pen pen1 = new Pen(new SolidColorBrush(Color.FromArgb((byte)(255 * 0.8), 30, 30, 30)), 1);
            Pen pen2 = new Pen(new SolidColorBrush(Color.FromArgb((byte)(200 * 0.5), 30, 30, 30)), 1);
            Pen pen3 = new Pen(new SolidColorBrush(Color.FromArgb((byte)(200 * 0.3), 30, 30, 30)), 1);

            int i = 0;
            for (double p = 0; p < this.ActualWidth; p += smallSize, i++)
            {
                var a = new Point(p, 0);
                var b = new Point(p, this.ActualHeight);

                Draw(dc, pen1, pen2, pen3, i, a, b);
            }

            i = 0;
            for (double p = 0; p < this.ActualHeight; p += smallSize, i++)
            {
                var a = new Point(0, p);
                var b = new Point(this.ActualWidth, p);

                Draw(dc, pen1, pen2, pen3, i, a, b);
            }
        }

        private void Draw(DrawingContext dc, Pen pen1, Pen pen2, Pen pen3, int i, Point a, Point b)
        {
            if (i % 12 == 0)
            {
                dc.DrawLine(pen1, a, b);
            }
            else if (i % 12 % 4 == 0)
            {
                dc.DrawLine(pen2, a, b);
            }
            else
            {
                dc.DrawLine(pen3, a, b);
            }
        }
    }
}
