using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Input;
using System.Windows.Media;

namespace NpcCore.Wpf.Controls
{
    public class ScrollViewerExt_Basic : ScrollViewer
    {
        private ImageExt_Basic m_imageExt_Basic;
        private Grid m_grid;
        private int zoomLevel = 1;

        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;

        public ScrollViewerExt_Basic()
        {
            this.ScrollChanged += ScrollViewerEx_ScrollChanged;
            this.MouseLeftButtonUp += ScrollViewerEx_MouseLeftButtonUp;
            this.PreviewMouseLeftButtonUp += ScrollViewerEx_MouseLeftButtonUp;
            this.PreviewMouseLeftButtonDown += ScrollViewerEx_PreviewMouseLeftButtonDown;
            this.PreviewMouseWheel += ScrollViewerEx_PreviewMouseWheel;
            this.PreviewMouseRightButtonDown += ScrollViewerEx_PreviewMouseRightButtonDown;
            this.MouseMove += ScrollViewerEx_MouseMove;
        }

        private void ScrollViewerEx_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Reset();
        }
        /// <summary>
        /// Reset grid contain ImageEx
        /// </summary>
        public void Reset()
        {
            // reset zoom
            var st = GetScaleTransform(m_grid);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            // reset pan
            var tt = GetTranslateTransform(m_grid);
            tt.X = 0.0;
            tt.Y = 0.0;
        }
        private TranslateTransform GetTranslateTransform(Grid element)
        {
            return (TranslateTransform)((TransformGroup)element.LayoutTransform)
              .Children.First(tr => tr is TranslateTransform);
        }

        private ScaleTransform GetScaleTransform(Grid element)
        {
            return (ScaleTransform)((TransformGroup)element.LayoutTransform)
              .Children.First(tr => tr is ScaleTransform);
        }

        private void ScrollViewerEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(this);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                this.ScrollToHorizontalOffset(this.HorizontalOffset - dX);
                this.ScrollToVerticalOffset(this.VerticalOffset - dY);
            }
        }

        private void ScrollViewerEx_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = GetScaleTransform(m_grid);
            var tt = GetTranslateTransform(m_grid);

            double zoom = e.Delta > 0 ? .2 : -.2;
            if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
                return;

            Point relative = Mouse.GetPosition(m_grid);
            lastMousePositionOnTarget = relative;
            double absoluteX;
            double absoluteY;

            absoluteX = relative.X * st.ScaleX + tt.X;
            absoluteY = relative.Y * st.ScaleY + tt.Y;

            st.ScaleX += zoom;
            st.ScaleY += zoom;

            tt.X = absoluteX - relative.X * st.ScaleX;
            tt.Y = absoluteY - relative.Y * st.ScaleY;

            var centerOfViewport = new Point(this.ViewportWidth / 2, this.ViewportHeight / 2);
            lastCenterPositionOnTarget = this.TranslatePoint(centerOfViewport, m_grid);
            e.Handled = true;
        }

        private void ScrollViewerEx_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_imageExt_Basic.IsSelectingRoi || m_imageExt_Basic.EnableMeasureSegLineTool || 
                m_imageExt_Basic.IsMeasuringSegLine || m_imageExt_Basic.EnableMeasureCircleTool || 
                m_imageExt_Basic.IsMeasuringCircle || m_imageExt_Basic.EnableSelectROIPolygonTool || 
                m_imageExt_Basic.IsSelectingPolygon)
                return;
            var mousePos = e.GetPosition(this);
            if (mousePos.X <= this.ViewportWidth && mousePos.Y < this.ViewportHeight) //make sure we still can use the scrollbars
            {
                this.Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(this);
            }
        }

        private void ScrollViewerEx_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            this.ReleaseMouseCapture();
            lastDragPoint = null;
        }

        private void ScrollViewerEx_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(this.ViewportWidth / 2, this.ViewportHeight / 2);
                        Point centerOfTargetNow = this.TranslatePoint(centerOfViewport, m_grid);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(m_grid);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / m_grid.Width;
                    double multiplicatorY = e.ExtentHeight / m_grid.Height;

                    double newOffsetX = this.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = this.VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    this.ScrollToHorizontalOffset(newOffsetX);
                    this.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
        public Point? LastMousePositionOnTarget
        {
            get { return lastMousePositionOnTarget; }
            set { lastMousePositionOnTarget = value; }
        }
        public Point? LastCenterPositionOnTarget
        {
            get { return lastCenterPositionOnTarget; }
            set { lastCenterPositionOnTarget = value; }
        }
        public Grid Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }
        public ImageExt_Basic ImageExt_Basic
        {
            get
            {
                if (m_imageExt_Basic != null)
                    return m_imageExt_Basic;
                else return null;
            }
            set { m_imageExt_Basic = value; }
        }
        public int ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }
    }
}
