using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NpcCore.Wpf.Controls
{
    public class ImageExt_Basic : Image, INotifyPropertyChanged
    {
        #region Implement Property Changed
        //
        // Summary:
        //     Occurs when a property value changes.
        public event PropertyChangedEventHandler PropertyChanged;

        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        //   onChanged:
        //     Action that is called after the property value has been changed.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support System.Runtime.CompilerServices.CallerMemberNameAttribute.
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   args:
        //     The PropertyChangedEventArgs
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
        #endregion

        AnchorPoint m_dragAnchor = AnchorPoint.None;
        HitType m_mouseHitType = HitType.None;

        #region Solidbrush
        SolidColorBrush colorBgRect = (SolidColorBrush)new BrushConverter().ConvertFrom("#3d424d");
        SolidColorBrush colorBgCorner = (SolidColorBrush)new BrushConverter().ConvertFrom("#ce3b3f");
        SolidColorBrush colorPen = (SolidColorBrush)new BrushConverter().ConvertFrom("#F3E9DF");
        SolidColorBrush colorCrossLine = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD764");
        double m_dThicknessPen = 0.5;
        #endregion

        #region Event
        public delegate void MouseMove_Handler(int nX, int nY, int r, int g, int b);
        public event MouseMove_Handler MouseMoveEndEvent;

        public static readonly RoutedEvent SelectedROI = EventManager.RegisterRoutedEvent(
            "SelectedROI",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler SelectedROIEvent
        {
            add
            {
                base.AddHandler(SelectedROI, value);
            }
            remove
            {
                base.RemoveHandler(SelectedROI, value);
            }
        }

        public static readonly RoutedEvent SelectedROIPolygon = EventManager.RegisterRoutedEvent(
            "SelectedROIPolygon",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler SelectedROIPolygonEvent
        {
            add
            {
                base.AddHandler(SelectedROIPolygon, value);
            }
            remove
            {
                base.RemoveHandler(SelectedROIPolygon, value);
            }
        }

        public static readonly RoutedEvent SaveFullImage = EventManager.RegisterRoutedEvent(
            "SaveFullImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler SaveFullImageEvent
        {
            add
            {
                base.AddHandler(SaveFullImage, value);
            }
            remove
            {
                base.RemoveHandler(SaveFullImage, value);
            }
        }

        public static readonly RoutedEvent SaveROIImage = EventManager.RegisterRoutedEvent(
            "SaveROIImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler SaveROIImageEvent
        {
            add
            {
                base.AddHandler(SaveROIImage, value);
            }
            remove
            {
                base.RemoveHandler(SaveROIImage, value);
            }
        }

        public static readonly RoutedEvent TrainLocator = EventManager.RegisterRoutedEvent(
            "TrainLocator",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler TrainLocatorEvent
        {
            add
            {
                base.AddHandler(TrainLocator, value);
            }
            remove
            {
                base.RemoveHandler(TrainLocator, value);
            }
        }

        public static readonly RoutedEvent Fit = EventManager.RegisterRoutedEvent(
            "Fit",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler FitEvent
        {
            add
            {
                base.AddHandler(Fit, value);
            }
            remove
            {
                base.RemoveHandler(Fit, value);
            }
        }

        public static readonly RoutedEvent MeasureSegLine = EventManager.RegisterRoutedEvent(
            "MeasureSegLine",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler MeasureSegLineEvent
        {
            add
            {
                base.AddHandler(MeasureSegLine, value);
            }
            remove
            {
                base.RemoveHandler(MeasureSegLine, value);
            }
        }

        public static readonly RoutedEvent MeasureCircle = EventManager.RegisterRoutedEvent(
            "MeasureCircle",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler MeasureCircleEvent
        {
            add
            {
                base.AddHandler(MeasureCircle, value);
            }
            remove
            {
                base.RemoveHandler(MeasureCircle, value);
            }
        }

        public static readonly RoutedEvent CountPixel = EventManager.RegisterRoutedEvent(
           "CountPixel",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt_Basic));
        public event RoutedEventHandler CountPixelEvent
        {
            add
            {
                base.AddHandler(CountPixel, value);
            }
            remove
            {
                base.RemoveHandler(CountPixel, value);
            }
        }

        public static readonly RoutedEvent CountBlob = EventManager.RegisterRoutedEvent(
          "CountBlob",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler CountBlobEvent
        {
            add
            {
                base.AddHandler(CountBlob, value);
            }
            remove
            {
                base.RemoveHandler(CountBlob, value);
            }
        }

        public static readonly RoutedEvent FindLine = EventManager.RegisterRoutedEvent(
           "FindLine",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt_Basic));
        public event RoutedEventHandler FindLineEvent
        {
            add
            {
                base.AddHandler(FindLine, value);
            }
            remove
            {
                base.RemoveHandler(FindLine, value);
            }
        }

        public static readonly RoutedEvent FindCircle = EventManager.RegisterRoutedEvent(
          "FindCircle",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler FindCircleEvent
        {
            add
            {
                base.AddHandler(FindCircle, value);
            }
            remove
            {
                base.RemoveHandler(FindCircle, value);
            }
        }

        public static readonly RoutedEvent PCA = EventManager.RegisterRoutedEvent(
          "PCA",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler PCAEvent
        {
            add
            {
                base.AddHandler(PCA, value);
            }
            remove
            {
                base.RemoveHandler(PCA, value);
            }
        }

        public static readonly RoutedEvent KNearest = EventManager.RegisterRoutedEvent(
          "KNearest",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler KNearestEvent
        {
            add
            {
                base.AddHandler(KNearest, value);
            }
            remove
            {
                base.RemoveHandler(KNearest, value);
            }
        }

        public static readonly RoutedEvent SVM = EventManager.RegisterRoutedEvent(
          "SVM",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler SVMEvent
        {
            add
            {
                base.AddHandler(SVM, value);
            }
            remove
            {
                base.RemoveHandler(SVM, value);
            }
        }

        public static readonly RoutedEvent OCR = EventManager.RegisterRoutedEvent(
          "OCR",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler OCREvent
        {
            add
            {
                base.AddHandler(OCR, value);
            }
            remove
            {
                base.RemoveHandler(OCR, value);
            }
        }

        public static readonly RoutedEvent TemplateRotate = EventManager.RegisterRoutedEvent(
          "TemplateRotate",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler TemplateRotateEvent
        {
            add
            {
                base.AddHandler(TemplateRotate, value);
            }
            remove
            {
                base.RemoveHandler(TemplateRotate, value);
            }
        }

        public static readonly RoutedEvent Decode = EventManager.RegisterRoutedEvent(
          "Decode",
          RoutingStrategy.Bubble,
          typeof(RoutedEventHandler),
          typeof(ImageExt_Basic));
        public event RoutedEventHandler DecodeEvent
        {
            add
            {
                base.AddHandler(Decode, value);
            }
            remove
            {
                base.RemoveHandler(Decode, value);
            }
        }
        #endregion

        #region Member Variables
        private bool m_bDrag;
        private bool m_bCompletedSelectROI;
        private bool m_bEnableSelectRectROITool;
        private bool m_bEnableSelectPolygonROITool;
        private bool m_bEnableSelectCircleROITool;
        private bool m_bEnableInspectTool;
        private bool m_bEnableMeasureSegLineTool;
        private bool m_bEnableMeasureCircleTool;
        private bool m_bEnableRotate;
        private bool m_bEnableLocatorTool;
        private bool m_bEnableSelectRect;
        private bool m_bEnableSelectRectInside;
        private bool m_bSelectingRectROI;
        private bool m_bSelectingPolygonROI;
        private bool m_bSelectingCircleROI;
        private bool m_bMeasuringSegLine;
        private bool m_bMeasuringCircle;
        private bool m_bSelectedPt1CircleROI;
        private bool m_bSelectedPt2CircleROI;
        private bool m_bMeasureSegLineSelectedPt1;
        private bool m_bMeasureSegLineSelectedPt2;
        private bool m_bMeasureCircleSelectedPt1;
        private bool m_bMeasureCircleSelectedPt2;

        private Rect m_rectDrag;
        private Rect m_rectInsideDrag;
        private Rect m_rect;
        private Rect m_rectReal;
        private Rect m_rectInside;
        private Rect m_rectTransform;
        private Point m_centerPoint;
        private Point m_centerPointReal;
        private Point m_offsetRectPoint;
        private Point m_offsetRectInsidePoint;
        private Point m_offsetXYInsidePoint;
        private Point m_dragStartPoint;
        private Point m_dragStartOffsetPoint;
        private Point m_startPoint_MeasureSegLineTool;
        private Point m_endPoint_MeasureSegLineTool;
        private Point m_centerPoint_MeasureCircleTool;
        private Point m_radiusPoint_MeasureCircleTool;
        private Point m_currentPoint;
        private Point m_centerPointCircleROI;
        private Point m_radiusPointCircleROI;
        private Size m_szDragSize;
        private Single m_rectRotation;

        private emToolType m_toolType = emToolType.ToolType_Default;

        private double m_dComWidth = 20;
        private double m_dComOffset = 10;
        private double m_dRadiusCircleROI;

        private List<Point> m_listPointsPolygon;

        private System.Drawing.Bitmap m_bmp;
        #endregion

        #region Methods
        public void SetHitType(MouseEventArgs e)
        {
            // Compute a Screen to Rectangle transform 

            var mat = new Matrix();
            mat.RotateAt(m_rectRotation, m_centerPoint.X, m_centerPoint.Y);
            mat.Translate(m_offsetRectPoint.X, m_offsetRectPoint.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            var rect = m_rect;
            var rectTopLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectTopRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectBottomLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectBottomRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidTop = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidBottom = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
            var ellipse = new EllipseGeometry(new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top - 20), m_dComWidth * 0.7, m_dComWidth * 0.7);

            Rect rectChild = new Rect();
            Rect rectTopLeftChild = new Rect();
            Rect rectTopRightChild = new Rect();
            Rect rectBottomLeftChild = new Rect();
            Rect rectBottomRightChild = new Rect();
            Rect rectMidTopChild = new Rect();
            Rect rectMidBottomChild = new Rect();
            Rect rectMidLeftChild = new Rect();
            Rect rectMidRightChild = new Rect();
            if (m_bEnableLocatorTool)
            {
                rectChild = m_rectInside;
                rectTopLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                rectTopRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                rectBottomLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                rectBottomRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                rectMidTopChild = new Rect(m_rectInside.Left + m_rectInside.Width / 2 - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                rectMidBottomChild = new Rect(m_rectInside.Left + m_rectInside.Width / 2 - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                rectMidLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top + m_rectInside.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
                rectMidRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top + m_rectInside.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
            }

            if (rectTopLeft.Contains(point) || rectTopLeftChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.TopLeft;
                m_mouseHitType = HitType.TopLeftChild;
                SetMouseCusor();
            }
            else if (rectTopRight.Contains(point) || rectTopRightChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.TopRight;
                m_mouseHitType = HitType.TopRightChild;
                SetMouseCusor();
            }
            else if (rectBottomLeft.Contains(point) || rectBottomLeftChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.BottomLeft;
                m_mouseHitType = HitType.BottomLeftChild;
                SetMouseCusor();
            }
            else if (rectBottomRight.Contains(point) || rectBottomRightChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.BottomRight;
                m_mouseHitType = HitType.BottomRightChild;
                SetMouseCusor();
            }
            else if (rectMidTop.Contains(point) || rectMidTopChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.MidTop;
                m_mouseHitType = HitType.MidTopChild;
                SetMouseCusor();
            }
            else if (rectMidBottom.Contains(point) || rectMidBottomChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.MidBottom;
                m_mouseHitType = HitType.MidBottomChild;
                SetMouseCusor();
            }
            else if (rectMidLeft.Contains(point) || rectMidLeftChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.MidLeft;
                m_mouseHitType = HitType.MidLeftChild;
                SetMouseCusor();
            }
            else if (rectMidRight.Contains(point) || rectMidRightChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.MidRight;
                m_mouseHitType = HitType.MidRightChild;
                SetMouseCusor();
            }
            else if (ellipse.FillContains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.Rotate;
                SetMouseCusor();
            }
            else if (rect.Contains(point) || rectChild.Contains(point))
            {
                m_bSelectingRectROI = true;
                m_mouseHitType = HitType.Body;
                m_mouseHitType = HitType.BodyChild;
                SetMouseCusor();
            }

            else
            {
                m_bSelectingRectROI = false;
                m_mouseHitType = HitType.None;
                m_mouseHitType = HitType.NoneChild;
                SetMouseCusor();
            }
        }
        public void SetMouseCusor()
        {
            Cursor desired_cursor = Cursors.Arrow;
            switch (m_mouseHitType)
            {
                case HitType.None:
                case HitType.NoneChild:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                case HitType.BodyChild:
                    desired_cursor = Cursors.SizeAll;
                    break;
                case HitType.Rotate:
                    desired_cursor = Cursors.UpArrow;
                    break;
                case HitType.TopLeft:
                case HitType.TopLeftChild:
                case HitType.BottomRight:
                case HitType.BottomRightChild:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.TopRight:
                case HitType.BottomLeft:
                case HitType.TopRightChild:
                case HitType.BottomLeftChild:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.MidTop:
                case HitType.MidBottom:
                case HitType.MidTopChild:
                case HitType.MidBottomChild:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.MidLeft:
                case HitType.MidRight:
                case HitType.MidLeftChild:
                case HitType.MidRightChild:
                    desired_cursor = Cursors.SizeWE;
                    break;
                default:
                    break;
            }
            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }
        public void ResetImageEx()
        {
            m_rect = new Rect(new Point(100, 100), new Size(220, 160));
            m_rectTransform = m_rect;
            m_offsetRectPoint = new Point(0, 0);
            m_rectRotation = 0;
            m_centerPoint = new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top + m_rect.Height / 2);
            m_centerPointReal = new Point(m_centerPoint.X + m_offsetRectPoint.X, m_centerPoint.Y + m_offsetRectPoint.Y);

            EnableSelectROIRectTool = false;
            EnableLocatorTool = false;
            EnableInspectTool = false;
            EnableMeasureSegLineTool = false;

            Reset();
        }
        public void Reset()
        {
            // reset zoom
            var st = GetScaleTransform(this);
            st.ScaleX = 1.0;
            st.ScaleY = 1.0;

            // reset pan
            var tt = GetTranslateTransform(this);
            tt.X = 0.0;
            tt.Y = 0.0;
        }
        private TranslateTransform GetTranslateTransform(UIElement element)
        {
            return (TranslateTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is TranslateTransform);
        }
        private ScaleTransform GetScaleTransform(UIElement element)
        {
            return (ScaleTransform)((TransformGroup)element.RenderTransform)
              .Children.First(tr => tr is ScaleTransform);
        }
        private void InitializeAllRect()
        {
            // Params: Rect ROI and Locator Tool
            m_rect = new Rect(new Point(100, 100), new Size(220, 160));
            m_rectReal = new Rect(new Point(m_rect.X + m_offsetRectPoint.X, m_rect.Y + m_offsetRectPoint.Y), new Size(m_rect.Width, m_rect.Height));
            m_rectInside = new Rect(new Point(m_rect.X + 60, m_rect.Y + 40), new Size(100, 80));
            m_rectTransform = m_rect;
            m_offsetXYInsidePoint = new Point((m_rect.Width - m_rectInside.Width) / 2, (m_rect.Height - m_rectInside.Height) / 2);
            m_offsetRectPoint = new Point(0, 0);
            m_rectRotation = 0;
            m_centerPoint = new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top + m_rect.Height / 2);
            m_centerPointReal = new Point(m_centerPoint.X + m_offsetRectPoint.X, m_centerPoint.Y + m_offsetRectPoint.Y);
        }
        private void InitializeCircleTool()
        {
            // Param: Circle ROI
            m_centerPointCircleROI = new Point(0, 0);
            m_radiusPointCircleROI = new Point(0, 0);
            m_dRadiusCircleROI = 0.0;
        }

        #region "Cross and Dot Products"
        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        private float CrossProductLength(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static float DotProduct(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }
        #endregion // Cross and Dot Products

        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        private float GetAngle(float Ax, float Ay, float Bx, float By, float Cx, float Cy)
        {
            // Get the dot product.
            float dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            float cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return (float)Math.Atan2(cross_product, dot_product);
        }
        // Return true if the point is in the polygon.
        private bool PointInPolygon(float X, float Y)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = m_listPointsPolygon.Count - 1;
            float total_angle = GetAngle(
                (float)m_listPointsPolygon[max_point].X, (float)m_listPointsPolygon[max_point].Y,
                X, Y,
                (float)m_listPointsPolygon[0].X, (float)m_listPointsPolygon[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    (float)m_listPointsPolygon[i].X, (float)m_listPointsPolygon[i].Y,
                    X, Y,
                    (float)m_listPointsPolygon[i + 1].X, (float)m_listPointsPolygon[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }
        private bool CheckIsPointInsideCircle(Point pt, Point centerPt, double dRadius)
        {
            bool bRet = false;

            double dDeltaX = Math.Abs(pt.X - centerPt.X );
            double dDeltaY = Math.Abs(pt.Y - centerPt.Y);
            double dHypoteuse = Math.Round(Math.Sqrt(dDeltaX * dDeltaX + dDeltaY * dDeltaY), 1);
            if(dHypoteuse <= Math.Round(dRadius, 1))
            {
                bRet = true;
            }

            return bRet;
        }
        #endregion

        #region Draw Polygon

        #endregion

        #region Constructor
        public ImageExt_Basic()
        {
            InitializeAllRect();
            InitializeCircleTool();

            m_listPointsPolygon = new List<Point>();

            TransformGroup group = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            group.Children.Add(st);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            this.RenderTransform = group;
            this.RenderTransformOrigin = new Point(0.0, 0.0);

            InitContextMenuRoiMode();
            InitContextMenuDefault();
            InitContextMenuLocator();
            InitContextMenuInspecTools();

            this.MouseDown += ImageEx_MouseDown;
            this.MouseMove += ImageEx_MouseMove;
            this.MouseUp += ImageEx_MouseUp;
            this.MouseWheel += ImageEx_MouseWheel;

            this.MouseLeftButtonUp += ImageEx_MouseLeftButtonUp;
            this.PreviewMouseRightButtonDown += ImageEx_PreviewMouseRightButtonDown;
        }
        #endregion

        #region ContextMenu

        // context menu default 
        private ContextMenu m_ctxMnuDefault;

        // context menu in Select Roi tool mode and locator tool mode
        private ContextMenu m_ctxMnuRoiMode;

        // context menu in Locator tool
        private ContextMenu m_ctxMnuLocator;

        // context menu inspect tools
        private ContextMenu m_ctxMnuInspectTool;

        // Get roi and save image
        private MenuItem m_selectRectROIItem;
        private MenuItem m_selectPolygonROIItem;
        private MenuItem m_selectCircleROIItem;

        // default mode
        private MenuItem m_fitItem;
        private MenuItem m_measureItem;
        private MenuItem m_measureSegLineItem;
        private MenuItem m_measureCircleItem;
        private MenuItem m_selectModeTool;
        private MenuItem m_selectRoiMode;
        private MenuItem m_selectRectItem;
        private MenuItem m_selectPolyItem;
        private MenuItem m_selectCircleItem;
        private MenuItem m_locatorMode;
        private MenuItem m_saveFullImageItem;
        private MenuItem m_saveROIImageItem;

        // inspect tools
        private MenuItem m_countPixelItem;
        private MenuItem m_countBlobItem;
        private MenuItem m_findLineItem;
        private MenuItem m_findCircleItem;
        private MenuItem m_pcaItem;
        private MenuItem m_kNearestItem;
        private MenuItem m_svmItem;
        private MenuItem m_ocrItem;
        private MenuItem m_templateRotateItem;
        private MenuItem m_decodeItem;


        // the chooses in locator contextmenu
        private MenuItem m_trainItem;

        // method
        private void InitContextMenuRoiMode()
        {
            m_ctxMnuRoiMode = new ContextMenu();

            m_selectRectROIItem = new MenuItem();
            m_selectRectROIItem.Header = "Select Rect ROI";
            m_selectRectROIItem.Name = "mnuSelectRectROI";
            m_selectRectROIItem.Click += mnuSelectRectROI_Click;
            m_selectRectROIItem.FontFamily = new FontFamily("Georgia");
            m_selectRectROIItem.FontWeight = FontWeights.Regular;
            m_selectRectROIItem.FontSize = 14;
            m_selectRectROIItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_rectangle_n@3x.png", UriKind.RelativeOrAbsolute))
            };

            m_selectPolygonROIItem = new MenuItem();
            m_selectPolygonROIItem.Header = "Select Polygon ROI";
            m_selectPolygonROIItem.Name = "mnuSelectPolygonROI";
            m_selectPolygonROIItem.Click += mnuSelectPolygonROI_Click;
            m_selectPolygonROIItem.FontFamily = new FontFamily("Georgia");
            m_selectPolygonROIItem.FontWeight = FontWeights.Regular;
            m_selectPolygonROIItem.FontSize = 14;
            m_selectPolygonROIItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_ig_polygon_n@3x.png", UriKind.RelativeOrAbsolute))
            };

            m_selectCircleROIItem = new MenuItem();
            m_selectCircleROIItem.Header = "Select Circle ROI";
            m_selectCircleROIItem.Name = "mnuSelectCircleROI";
            m_selectCircleROIItem.Click += mnuSelectCircleROI_Click;
            m_selectCircleROIItem.FontFamily = new FontFamily("Georgia");
            m_selectCircleROIItem.FontWeight = FontWeights.Regular;
            m_selectCircleROIItem.FontSize = 14;
            m_selectCircleROIItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_circle_n@3x.png", UriKind.RelativeOrAbsolute))
            };

            m_ctxMnuRoiMode.Items.Add(m_selectRectROIItem);
            m_ctxMnuRoiMode.Items.Add(m_selectPolygonROIItem);
            m_ctxMnuRoiMode.Items.Add(m_selectCircleROIItem);
        }
        private void InitContextMenuDefault()
        {
            m_ctxMnuDefault = new ContextMenu();

            m_fitItem = new MenuItem();
            m_fitItem.Header = "Fit";
            m_fitItem.Name = "mnuFit";
            m_fitItem.Click += mnuFit_Click;
            m_fitItem.FontFamily = new FontFamily("Georgia");
            m_fitItem.FontWeight = FontWeights.Regular;
            m_fitItem.FontSize = 14;
            m_fitItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/zoom_to_fit_s@3x.png", UriKind.RelativeOrAbsolute))
            };

            // select mode tool item
            m_selectModeTool = new MenuItem();
            m_selectModeTool.Header = "Mode Tool";
            m_selectModeTool.Name = "mnuModeTool";
            m_selectModeTool.FontFamily = new FontFamily("Georgia");
            m_selectModeTool.FontWeight = FontWeights.Regular;
            m_selectModeTool.FontSize = 14;
            m_selectModeTool.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/btn_setting_50.png", UriKind.RelativeOrAbsolute))
            };

            // select Roi mode
            m_selectRoiMode = new MenuItem();
            m_selectRoiMode.Header = "Select ROI";
            m_selectRoiMode.Name = "mnuSelectRoi";
            m_selectRoiMode.FontFamily = new FontFamily("Georgia");
            m_selectRoiMode.FontWeight = FontWeights.Regular;
            m_selectRoiMode.FontSize = 14;
            m_selectRoiMode.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/Cursor/mc_ig_include_n@2x.png", UriKind.RelativeOrAbsolute))
            };

            // select rectangle
            m_selectRectItem = new MenuItem();
            m_selectRectItem.Header = "Rectangle";
            m_selectRectItem.Name = "mnuSelectRect";
            m_selectRectItem.Click += mnuSelectRect_Click;
            m_selectRectItem.FontFamily = new FontFamily("Georgia");
            m_selectRectItem.FontWeight = FontWeights.Regular;
            m_selectRectItem.FontSize = 14;
            m_selectRectItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_c_rectangle_n@3x.png", UriKind.RelativeOrAbsolute))
            };

            // select polygon
            m_selectPolyItem = new MenuItem();
            m_selectPolyItem.Header = "Polygon";
            m_selectPolyItem.Name = "mnuSelectPolygon";
            m_selectPolyItem.Click += mnuSelectPolygon_Click;
            m_selectPolyItem.FontFamily = new FontFamily("Georgia");
            m_selectPolyItem.FontWeight = FontWeights.Regular;
            m_selectPolyItem.FontSize = 14;
            m_selectPolyItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_c_polygon_n@3x.png", UriKind.RelativeOrAbsolute))
            };

            // select circle
            m_selectCircleItem = new MenuItem();
            m_selectCircleItem.Header = "Circle";
            m_selectCircleItem.Name = "mnuSelectCircle";
            m_selectCircleItem.Click += mnuSelectCircle_Click;
            m_selectCircleItem.FontFamily = new FontFamily("Georgia");
            m_selectCircleItem.FontWeight = FontWeights.Regular;
            m_selectCircleItem.FontSize = 14;
            m_selectCircleItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_c_circle_n@3x.png", UriKind.RelativeOrAbsolute))
            };

            // add 2 item select rectangle and polygon into menu Roi mode
            m_selectRoiMode.Items.Add(m_selectRectItem);
            m_selectRoiMode.Items.Add(m_selectPolyItem);
            m_selectRoiMode.Items.Add(m_selectCircleItem);

            // locator mode
            m_locatorMode = new MenuItem();
            m_locatorMode.Header = "Locator";
            m_locatorMode.Name = "mnuLocator";
            m_locatorMode.Click += mnuLocator_Click;
            m_locatorMode.FontFamily = new FontFamily("Georgia");
            m_locatorMode.FontWeight = FontWeights.Regular;
            m_locatorMode.FontSize = 14;
            m_locatorMode.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_baseteaching_s@3x.png", UriKind.RelativeOrAbsolute))
            };

            // measure item
            m_measureItem = new MenuItem();
            m_measureItem.Header = "Measure";
            m_measureItem.Name = "mnuMeasure";
            m_measureItem.FontFamily = new FontFamily("Georgia");
            m_measureItem.FontWeight = FontWeights.Regular;
            m_measureItem.FontSize = 14;
            m_measureItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_toolbar_ruler_s_2.png", UriKind.RelativeOrAbsolute))
            };

            // measure seg line item
            m_measureSegLineItem = new MenuItem();
            m_measureSegLineItem.Header = "Measure Segment Line";
            m_measureSegLineItem.Name = "mnuMeasureSegLine";
            m_measureSegLineItem.Click += mnuMeasureSegLine_Click;
            m_measureSegLineItem.FontFamily = new FontFamily("Georgia");
            m_measureSegLineItem.FontWeight = FontWeights.Regular;
            m_measureSegLineItem.FontSize = 14;

            // measure circle item
            m_measureCircleItem = new MenuItem();
            m_measureCircleItem.Header = "Measure Circle";
            m_measureCircleItem.Name = "mnuMeasureCircle";
            m_measureCircleItem.Click += mnuMeasureCircle_Click;
            m_measureCircleItem.FontFamily = new FontFamily("Georgia");
            m_measureCircleItem.FontWeight = FontWeights.Regular;
            m_measureCircleItem.FontSize = 14;

            m_measureItem.Items.Add(m_measureSegLineItem);
            m_measureItem.Items.Add(m_measureCircleItem);

            // save Image
            m_saveFullImageItem = new MenuItem();
            m_saveFullImageItem.Header = "Save Full Image";
            m_saveFullImageItem.Name = "mnuSaveFullImage";
            m_saveFullImageItem.Click += mnuSaveFullImage_Click;
            m_saveFullImageItem.FontFamily = new FontFamily("Georgia");
            m_saveFullImageItem.FontWeight = FontWeights.Regular;
            m_saveFullImageItem.FontSize = 14;

            // add 2 mode: select Roi and locator into select mode tool item
            m_selectModeTool.Items.Add(m_selectRoiMode);
            m_selectModeTool.Items.Add(m_locatorMode);

            m_ctxMnuDefault.Items.Add(m_fitItem);
            m_ctxMnuDefault.Items.Add(m_selectModeTool);

            m_ctxMnuDefault.Items.Add(m_measureItem);
            m_ctxMnuDefault.Items.Add(m_saveFullImageItem);
            m_ctxMnuDefault.PlacementTarget = this;
            m_ctxMnuDefault.IsOpen = false;

        }
        private void InitContextMenuLocator()
        {
            m_ctxMnuLocator = new ContextMenu();

            m_trainItem = new MenuItem();
            m_trainItem = new MenuItem();
            m_trainItem.Header = "Train";
            m_trainItem.Name = "mnuTrain";
            m_trainItem.Click += mnuTrain_Click;
            m_trainItem.FontFamily = new FontFamily("Georgia");
            m_trainItem.FontWeight = FontWeights.Regular;
            m_trainItem.FontSize = 14;

            m_ctxMnuLocator.Items.Add(m_trainItem);
            m_ctxMnuLocator.PlacementTarget = this;
            m_ctxMnuLocator.IsOpen = false;
        }
        private void InitContextMenuInspecTools()
        {
            m_ctxMnuInspectTool = new ContextMenu();

            // Count pixel
            m_countPixelItem = new MenuItem();
            m_countPixelItem.Header = "Count Pixel";
            m_countPixelItem.Name = "mnuCntPixel";
            m_countPixelItem.Click += mnuCntPixel_Click;
            m_countPixelItem.FontFamily = new FontFamily("Georgia");
            m_countPixelItem.FontWeight = FontWeights.Regular;
            m_countPixelItem.FontSize = 14;

            // Count blob
            m_countBlobItem = new MenuItem();
            m_countBlobItem.Header = "Count Blob";
            m_countBlobItem.Name = "mnuCntBlob";
            m_countBlobItem.Click += mnuCntBlob_Click;
            m_countBlobItem.FontFamily = new FontFamily("Georgia");
            m_countBlobItem.FontWeight = FontWeights.Regular;
            m_countBlobItem.FontSize = 14;

            // Find line
            m_findLineItem = new MenuItem();
            m_findLineItem.Header = "Find Line";
            m_findLineItem.Name = "mnuFindLine";
            m_findLineItem.Click += mnuFindLine_Click;
            m_findLineItem.FontFamily = new FontFamily("Georgia");
            m_findLineItem.FontWeight = FontWeights.Regular;
            m_findLineItem.FontSize = 14;

            // Find Circle
            m_findCircleItem = new MenuItem();
            m_findCircleItem.Header = "Find Circle";
            m_findCircleItem.Name = "mnuFindCircle";
            m_findCircleItem.Click += mnuFindCircle_Click;
            m_findCircleItem.FontFamily = new FontFamily("Georgia");
            m_findCircleItem.FontWeight = FontWeights.Regular;
            m_findCircleItem.FontSize = 14;

            // PCA
            m_pcaItem = new MenuItem();
            m_pcaItem.Header = "PCA";
            m_pcaItem.Name = "mnuPCA";
            m_pcaItem.Click += mnuPCA_Click;
            m_pcaItem.FontFamily = new FontFamily("Georgia");
            m_pcaItem.FontWeight = FontWeights.Regular;
            m_pcaItem.FontSize = 14;

            // K-Nearest
            m_kNearestItem = new MenuItem();
            m_kNearestItem.Header = "Train K-Nearest";
            m_kNearestItem.Name = "mnuTrainKNearest";
            m_kNearestItem.Click += mnuTrainKNearest_Click;
            m_kNearestItem.FontFamily = new FontFamily("Georgia");
            m_kNearestItem.FontWeight = FontWeights.Regular;
            m_kNearestItem.FontSize = 14;

            // SVM
            m_svmItem = new MenuItem();
            m_svmItem.Header = "Train SVM";
            m_svmItem.Name = "mnuTrainSVM";
            m_svmItem.Click += mnuTrainSVM_Click;
            m_svmItem.FontFamily = new FontFamily("Georgia");
            m_svmItem.FontWeight = FontWeights.Regular;
            m_svmItem.FontSize = 14;

            // OCR
            m_ocrItem = new MenuItem();
            m_ocrItem.Header = "OCR";
            m_ocrItem.Name = "mnuOCR";
            m_ocrItem.Click += mnuOCR_Click;
            m_ocrItem.FontFamily = new FontFamily("Georgia");
            m_ocrItem.FontWeight = FontWeights.Regular;
            m_ocrItem.FontSize = 14;

            // Template Rotate
            m_templateRotateItem = new MenuItem();
            m_templateRotateItem.Header = "Template Rotate";
            m_templateRotateItem.Name = "mnuTemplateRotate";
            m_templateRotateItem.Click += mnuTemplateRotate_Click;
            m_templateRotateItem.FontFamily = new FontFamily("Georgia");
            m_templateRotateItem.FontWeight = FontWeights.Regular;
            m_templateRotateItem.FontSize = 14;

            // Decode
            m_decodeItem = new MenuItem();
            m_decodeItem.Header = "Decode";
            m_decodeItem.Name = "mnuDecode";
            m_decodeItem.Click += mnuDecode_Click;
            m_decodeItem.FontFamily = new FontFamily("Georgia");
            m_decodeItem.FontWeight = FontWeights.Regular;
            m_decodeItem.FontSize = 14;
            m_decodeItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/ic_gt_barcode_s@3x.png", UriKind.RelativeOrAbsolute))
            };

            // Save ROI
            m_saveROIImageItem = new MenuItem();
            m_saveROIImageItem.Header = "Save ROI Image";
            m_saveROIImageItem.Name = "mnuSaveROIImage";
            m_saveROIImageItem.Click += mnuSaveROIImage_Click;
            m_saveROIImageItem.FontFamily = new FontFamily("Georgia");
            m_saveROIImageItem.FontWeight = FontWeights.Regular;
            m_saveROIImageItem.FontSize = 14;
            m_saveROIImageItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/NpcCore.Wpf;component/Resources/Images/btn_save_n@2x.png", UriKind.RelativeOrAbsolute))
            };

            m_ctxMnuInspectTool.Items.Add(m_countPixelItem);
            m_ctxMnuInspectTool.Items.Add(m_countBlobItem);
            m_ctxMnuInspectTool.Items.Add(m_findLineItem);
            m_ctxMnuInspectTool.Items.Add(m_findCircleItem);
            m_ctxMnuInspectTool.Items.Add(m_pcaItem);
            m_ctxMnuInspectTool.Items.Add(m_kNearestItem);
            m_ctxMnuInspectTool.Items.Add(m_svmItem);
            m_ctxMnuInspectTool.Items.Add(m_ocrItem);
            m_ctxMnuInspectTool.Items.Add(m_templateRotateItem);
            m_ctxMnuInspectTool.Items.Add(m_decodeItem);
            m_ctxMnuInspectTool.Items.Add(new Separator());
            m_ctxMnuInspectTool.Items.Add(m_saveROIImageItem);

            m_ctxMnuInspectTool.PlacementTarget = this;
            m_ctxMnuInspectTool.IsOpen = false;
        }

        #region Functions Handle Event for MenuItem Default
        private void mnuFit_Click(object sender, RoutedEventArgs e)
        {
            //this.Reset();
            //this.InvalidateVisual();
            RaiseEvent(new RoutedEventArgs(Fit, this));
        }
        private void mnuMeasureSegLine_Click(object sender, RoutedEventArgs e)
        {
            m_listPointsPolygon.Clear();
            EnableMeasureSegLineTool = true;
            RaiseEvent(new RoutedEventArgs(MeasureSegLine, this));
        }
        private void mnuMeasureCircle_Click(object sender, RoutedEventArgs e)
        {
            m_listPointsPolygon.Clear();
            EnableMeasureCircleTool = true;
            RaiseEvent(new RoutedEventArgs(MeasureSegLine, this));
        }
        private void mnuLocator_Click(object sender, RoutedEventArgs e)
        {
            InitializeAllRect();
            m_listPointsPolygon.Clear();
            EnableLocatorTool = true;
        }
        private void mnuTrain_Click(object sender, RoutedEventArgs e)
        {
            EnableLocatorTool = false;
            RaiseEvent(new RoutedEventArgs(TrainLocator, this));
        }
        private void mnuSelectRect_Click(object sender, RoutedEventArgs e)
        {
            InitializeAllRect();
            EnableSelectROIRectTool = true;
        }
        private void mnuSelectPolygon_Click(object sender, RoutedEventArgs e)
        {
            m_listPointsPolygon.Clear();
            EnableSelectROIPolygonTool = true;
        }
        private void mnuSelectCircle_Click(object sender, RoutedEventArgs e)
        {
            InitializeCircleTool();
            EnableSelectROICircleTool = true;
        }
        private void mnuSaveFullImage_Click(object sender, RoutedEventArgs e)
        {
            EnableSelectROIRectTool = false;
            RaiseEvent(new RoutedEventArgs(SaveFullImage, this));
        }
        private void mnuSaveROIImage_Click(object sender, RoutedEventArgs e)
        {
            EnableSelectROIRectTool = false;
            RaiseEvent(new RoutedEventArgs(SaveROIImage, this));
        }
        #endregion

        #region Functions Handle Event for ROI
        private void mnuSelectRectROI_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = true;
            EnableSelectROIRectTool = false;
            IsSelectingRectROI = false;
            EnableInspectTool = true;
            RaiseEvent(new RoutedEventArgs(SelectedROI, this));
        }
        private void mnuSelectPolygonROI_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = true;
            EnableSelectROIPolygonTool = false;
            EnableInspectTool = true;
            RaiseEvent(new RoutedEventArgs(SelectedROIPolygon, this));
        }
        private void mnuSelectCircleROI_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = true;
            EnableSelectROICircleTool = false;
            EnableInspectTool = true;
            RaiseEvent(new RoutedEventArgs(SelectedROIPolygon, this));
        }
        private void mnuCntPixel_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(CountPixel, this));
        }
        private void mnuCntBlob_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(CountBlob, this));
        }
        private void mnuFindLine_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(FindLine, this));
        }
        private void mnuFindCircle_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(FindCircle, this));
        }
        private void mnuPCA_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(PCA, this));
        }
        private void mnuTrainKNearest_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(KNearest, this));
        }
        private void mnuTrainSVM_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(SVM, this));
        }
        private void mnuOCR_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(OCR, this));
        }
        private void mnuTemplateRotate_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(TemplateRotate, this));
        }
        private void mnuDecode_Click(object sender, RoutedEventArgs e)
        {
            m_bCompletedSelectROI = false;
            EnableInspectTool = false;
            RaiseEvent(new RoutedEventArgs(Decode, this));
        }
        #endregion

        #endregion

        #region Handle Event
        private void ImageEx_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool && !m_bEnableSelectCircleROITool 
                && !m_bEnableLocatorTool && !m_bEnableInspectTool && !m_bEnableMeasureSegLineTool)
            {
                m_ctxMnuDefault.IsOpen = true;
                return;
            }

            var mat = new Matrix();
            if (m_bEnableRotate == true)
                mat.RotateAt(m_rectRotation, m_centerPoint.X, m_centerPoint.Y);
            mat.Translate(m_offsetRectPoint.X, m_offsetRectPoint.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            bool bInsidePoly = false;
            bool bInsideCircle = false; 

            if (m_listPointsPolygon.Count > 3)
            {
                bInsidePoly = PointInPolygon((float)point.X, (float)point.Y);
            }

            if(m_centerPointCircleROI != new Point(0,0) && m_dRadiusCircleROI > 0.0)
            {
                bInsideCircle = CheckIsPointInsideCircle(point, m_centerPointCircleROI, m_dRadiusCircleROI);
            }

            // Poly
            if (bInsidePoly && !m_bEnableSelectRectROITool && m_bEnableSelectPolygonROITool && !m_bEnableSelectCircleROITool
                && !m_bEnableLocatorTool && !m_bEnableInspectTool)
            {
                m_ctxMnuRoiMode.IsOpen = true;
                return;
            }
            if (bInsidePoly && !m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool && !m_bEnableSelectCircleROITool
               && !m_bEnableLocatorTool && m_bEnableInspectTool)
            {
                m_ctxMnuInspectTool.IsOpen = true;
                return;
            }

            // Circle
            if (bInsideCircle && !m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool && m_bEnableSelectCircleROITool
               && !m_bEnableLocatorTool && !m_bEnableInspectTool)
            {
                m_ctxMnuRoiMode.IsOpen = true;
                return;
            }
            if (bInsideCircle && !m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool && !m_bEnableSelectCircleROITool
               && !m_bEnableLocatorTool && m_bEnableInspectTool)
            {
                m_ctxMnuInspectTool.IsOpen = true;
                return;
            }

            if (m_rect.Contains(point) && m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool
                && !m_bEnableLocatorTool && !m_bEnableInspectTool)
            {
                m_ctxMnuRoiMode.IsOpen = true;
                return;
            }
            else if (m_rect.Contains(point) && !m_bEnableSelectRectROITool && m_bEnableLocatorTool && !m_bEnableInspectTool)
            {
                m_ctxMnuLocator.IsOpen = true;
                return;
            }
            else if (m_rect.Contains(point) && !m_bEnableSelectRectROITool && !m_bEnableLocatorTool && m_bEnableInspectTool)
            {
                m_ctxMnuInspectTool.IsOpen = true;
                return;
            }

        }
        private void ImageEx_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //var st = GetScaleTransform(this);
            //var tt = GetTranslateTransform(this);

            //double zoom = e.Delta > 0 ? .2 : -.2;
            //if (!(e.Delta > 0) && (st.ScaleX < .4 || st.ScaleY < .4))
            //    return;

            //Point relative = e.GetPosition(this);
            //double absoluteX;
            //double absoluteY;

            //absoluteX = relative.X * st.ScaleX + tt.X;
            //absoluteY = relative.Y * st.ScaleY + tt.Y;

            //st.ScaleX += zoom;
            //st.ScaleY += zoom;

            //tt.X = absoluteX - relative.X * st.ScaleX;
            //tt.Y = absoluteY - relative.Y * st.ScaleY;
        }
        private void ImageEx_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool && !m_bEnableSelectCircleROITool 
                && !m_bEnableLocatorTool && !m_bEnableMeasureSegLineTool && !m_bEnableMeasureCircleTool)
                return;

            if (m_bEnableMeasureSegLineTool && m_bMeasuringSegLine && m_bMeasureSegLineSelectedPt1 && m_bMeasureSegLineSelectedPt2)
            {
                m_startPoint_MeasureSegLineTool = new Point(0, 0);
                m_endPoint_MeasureSegLineTool = new Point(0, 0);

                m_bMeasureSegLineSelectedPt1 = false;
                m_bMeasureSegLineSelectedPt2 = false;
                m_bEnableMeasureSegLineTool = false;
                m_bMeasuringSegLine = false;

                return;
            }

            if (m_bEnableMeasureCircleTool && m_bMeasuringCircle && m_bMeasureCircleSelectedPt1 && m_bMeasureCircleSelectedPt2)
            {
                m_centerPoint_MeasureCircleTool = new Point(0, 0);
                m_radiusPoint_MeasureCircleTool = new Point(0, 0);

                m_bMeasureCircleSelectedPt1 = false;
                m_bMeasureCircleSelectedPt2 = false;
                m_bEnableMeasureCircleTool = false;
                m_bMeasuringCircle = false;

                return;
            }

            if (m_bEnableSelectPolygonROITool && m_bSelectingPolygonROI)
            {
                int numberOfPoint = m_listPointsPolygon.Count;
                if (numberOfPoint > 3)
                {
                    double dDeltaX = Math.Abs(Math.Round(m_listPointsPolygon[0].X - m_listPointsPolygon[numberOfPoint - 1].X, 1));
                    double dDeltaY = Math.Abs(Math.Round(m_listPointsPolygon[0].Y - m_listPointsPolygon[numberOfPoint - 1].Y, 1));

                    if (dDeltaX <= 2.0 && dDeltaY <= 2.0)
                    {
                        m_listPointsPolygon[numberOfPoint - 1] = m_listPointsPolygon[0];
                        m_bSelectingPolygonROI = false;
                    }
                }
            }

            if (m_bEnableSelectCircleROITool && m_bSelectingCircleROI && m_bSelectedPt1CircleROI && m_bSelectedPt2CircleROI)
            {
                double dDeltaX = Math.Round(Math.Abs(m_centerPointCircleROI.X - m_radiusPointCircleROI.X), 1);
                double dDeltaY = Math.Round(Math.Abs(m_centerPointCircleROI.Y - m_radiusPointCircleROI.Y), 1);

                m_dRadiusCircleROI = Math.Round(Math.Sqrt(dDeltaX * dDeltaX + dDeltaY * dDeltaY), 1);

                m_bSelectedPt1CircleROI = false;
                m_bSelectedPt2CircleROI = false;

                m_bSelectingCircleROI = false;

                return;
            }

            m_bDrag = false;

            m_offsetXYInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2;
            m_offsetXYInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2;

        }
        private void ImageEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool && !m_bEnableSelectCircleROITool && !m_bEnableLocatorTool)
            {
                if (m_bmp == null)
                    return;

                var pointEnd = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

                int nX = (int)(Math.Round(pointEnd.X, MidpointRounding.AwayFromZero));
                int nY = (int)(Math.Round(pointEnd.Y, MidpointRounding.AwayFromZero));

                if ((nX >= 0) && (nX < m_bmp.Width) && (nY >= 0) && (nY < m_bmp.Height))
                {

                    System.Drawing.Color color = m_bmp.GetPixel(nX, nY);
                    int r = color.R;
                    int g = color.G;
                    int b = color.B;
                    float hue = color.GetHue();
                    float saturation = color.GetSaturation();
                    float brightness = color.GetBrightness();

                    MouseMoveEndEvent?.Invoke(nX, nY, r, g, b);

                    //XY = String.Format("X: {0} Y: {1}", (int)p.X, (int)p.Y);
                    //RGB = String.Format("R: {0} G: {1} B: {2}", r.ToString(), g.ToString(), b.ToString());
                    //HSI = String.Format("H: {0} S: {1:0.000} I: {2:0.000}", (int)hue, saturation, brightness);
                }

                // Measure Segment Line
                if (m_bEnableMeasureSegLineTool && m_bMeasuringSegLine && m_bMeasureSegLineSelectedPt1 && !m_bMeasureSegLineSelectedPt2)
                {
                    m_endPoint_MeasureSegLineTool = pointEnd;
                    this.InvalidateVisual();
                }

                // Measure Circle
                if (m_bEnableMeasureCircleTool && m_bMeasuringCircle && m_bMeasureCircleSelectedPt1 && !m_bMeasureCircleSelectedPt2)
                {
                    m_radiusPoint_MeasureCircleTool = pointEnd;
                    this.InvalidateVisual();
                }

                return;
            }

            // Select Polygon
            if (m_bEnableSelectPolygonROITool && m_bSelectingPolygonROI)
            {
                if (m_bmp == null)
                    return;

                m_bDrag = true;

                var pointCur = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

                m_currentPoint = pointCur;
                //this.InvalidateVisual();

                return;
            }

            // Select Circle
            if (m_bEnableSelectCircleROITool && m_bSelectingCircleROI && m_bSelectedPt1CircleROI && !m_bSelectedPt2CircleROI)
            {
                if (m_bmp == null)
                    return;

                var pointCur = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

                m_radiusPointCircleROI = pointCur;
                this.InvalidateVisual();

                return;
            }

            SetHitType(e);
            if (!m_bDrag)
                return;

            var mat = new Matrix();
            if (m_bEnableRotate == true)
                mat.RotateAt(m_rectRotation, m_centerPoint.X, m_centerPoint.Y);
            mat.Translate(m_offsetRectPoint.X, m_offsetRectPoint.Y);
            mat.Invert();

            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            Point offsetSize;
            Point clamped;

            switch (m_dragAnchor)
            {
                case AnchorPoint.TopLeft:
                case AnchorPoint.TopLeftChild:
                    if (m_dragAnchor == AnchorPoint.TopLeft)
                    {
                        clamped = new Point(Math.Min(m_rect.BottomRight.X - 10d, point.X),
                            Math.Min(m_rect.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left + offsetSize.X,
                            m_rectDrag.Top + offsetSize.Y,
                            m_rectDrag.Width - offsetSize.X,
                            m_rectDrag.Height - offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + offsetSize.X + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + offsetSize.Y + m_offsetRectInsidePoint.Y,
                                 m_rectInside.Width,
                                 m_rectInside.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.TopLeftChild)
                    {
                        clamped = new Point(Math.Min(m_rectInside.BottomRight.X - 10d, point.X),
                            Math.Min(m_rectInside.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left + offsetSize.X,
                            m_rectInsideDrag.Top + offsetSize.Y,
                            m_rectInsideDrag.Width - offsetSize.X,
                            m_rectInsideDrag.Height - offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rect = new Rect(
                                  m_rectDrag.Left + offsetSize.X - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top + offsetSize.Y - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;
                case AnchorPoint.TopRight:
                case AnchorPoint.TopRightChild:
                    if (m_dragAnchor == AnchorPoint.TopRight)
                    {
                        clamped = new Point(Math.Max(m_rect.BottomLeft.X - 10d, point.X),
                            Math.Min(m_rect.BottomLeft.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left,
                            m_rectDrag.Top + offsetSize.Y,
                            m_rectDrag.Width + offsetSize.X,
                            m_rectDrag.Height - offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + offsetSize.Y + m_offsetRectInsidePoint.Y,
                                 m_rectInside.Width,
                                 m_rectInside.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.TopRightChild)
                    {
                        clamped = new Point(Math.Max(m_rectInside.BottomLeft.X - 10d, point.X),
                           Math.Min(m_rectInside.BottomLeft.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left,
                            m_rectInsideDrag.Top + offsetSize.Y,
                            m_rectInsideDrag.Width + offsetSize.X,
                            m_rectInsideDrag.Height - offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rect = new Rect(
                                  m_rectDrag.Left - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top + offsetSize.Y - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;

                case AnchorPoint.BottomLeft:
                case AnchorPoint.BottomLeftChild:
                    if (m_dragAnchor == AnchorPoint.BottomLeft)
                    {
                        clamped = new Point(Math.Min(m_rect.TopRight.X - 10d, point.X),
                            Math.Max(m_rect.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left + offsetSize.X,
                            m_rectDrag.Top,
                            m_rectDrag.Width - offsetSize.X,
                            m_rectDrag.Height + offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + offsetSize.X + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + m_offsetRectInsidePoint.Y,
                                 m_rectInside.Width,
                                 m_rectInside.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.BottomLeftChild)
                    {
                        clamped = new Point(Math.Min(m_rectInside.TopRight.X - 10d, point.X),
                           Math.Max(m_rectInside.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left + offsetSize.X,
                            m_rectInsideDrag.Top,
                            m_rectInsideDrag.Width - offsetSize.X,
                            m_rectInsideDrag.Height + offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rect = new Rect(
                                  m_rectDrag.Left + offsetSize.X - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;

                case AnchorPoint.BottomRight:
                case AnchorPoint.BottomRightChild:
                    if (m_dragAnchor == AnchorPoint.BottomRight)
                    {
                        clamped = new Point(Math.Max(m_rect.TopLeft.X + 10d, point.X),
                            Math.Max(m_rect.TopLeft.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left,
                            m_rectDrag.Top,
                            m_rectDrag.Width + offsetSize.X,
                            m_rectDrag.Height + offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;

                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + m_offsetRectInsidePoint.Y,
                                 m_rectInsideDrag.Width,
                                 m_rectInsideDrag.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.BottomRightChild)
                    {
                        clamped = new Point(Math.Max(m_rectInside.TopLeft.X + 10d, point.X),
                            Math.Max(m_rectInside.TopLeft.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left,
                            m_rectInsideDrag.Top,
                            m_rectInsideDrag.Width + offsetSize.X,
                            m_rectInsideDrag.Height + offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;

                        m_rect = new Rect(
                                  m_rectDrag.Left - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;

                case AnchorPoint.MidTop:
                case AnchorPoint.MidTopChild:
                    if (m_dragAnchor == AnchorPoint.MidTop)
                    {
                        clamped = new Point(Math.Min(m_rect.BottomRight.X - 10d, point.X),
                            Math.Min(m_rect.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left,
                            m_rectDrag.Top + offsetSize.Y,
                            m_rectDrag.Width,
                            m_rectDrag.Height - offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + offsetSize.Y + m_offsetRectInsidePoint.Y,
                                 m_rectInside.Width,
                                 m_rectInside.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.MidTopChild)
                    {
                        clamped = new Point(Math.Min(m_rectInside.BottomRight.X - 10d, point.X),
                            Math.Min(m_rectInside.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left,
                            m_rectInsideDrag.Top + offsetSize.Y,
                            m_rectInsideDrag.Width,
                            m_rectInsideDrag.Height - offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rect = new Rect(
                                  m_rectDrag.Left - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top + offsetSize.Y - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;
                case AnchorPoint.MidBottom:
                case AnchorPoint.MidBottomChild:
                    if (m_dragAnchor == AnchorPoint.MidBottom)
                    {
                        clamped = new Point(Math.Min(m_rect.TopRight.X - 10d, point.X),
                            Math.Max(m_rect.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left,
                            m_rectDrag.Top,
                            m_rectDrag.Width,
                            m_rectDrag.Height + offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + m_offsetRectInsidePoint.Y,
                                 m_rectInside.Width,
                                 m_rectInside.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.MidBottomChild)
                    {
                        clamped = new Point(Math.Min(m_rectInside.TopRight.X - 10d, point.X),
                            Math.Max(m_rectInside.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left,
                            m_rectInsideDrag.Top,
                            m_rectInsideDrag.Width,
                            m_rectInsideDrag.Height + offsetSize.Y);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rect = new Rect(
                                  m_rectDrag.Left - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;
                case AnchorPoint.MidLeft:
                case AnchorPoint.MidLeftChild:
                    if (m_dragAnchor == AnchorPoint.MidLeft)
                    {
                        clamped = new Point(Math.Min(m_rect.TopRight.X - 10d, point.X),
                            Math.Max(m_rect.TopRight.Y, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left + offsetSize.X,
                            m_rectDrag.Top,
                            m_rectDrag.Width - offsetSize.X,
                            m_rectDrag.Height);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + offsetSize.X + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + m_offsetRectInsidePoint.Y,
                                 m_rectInside.Width,
                                 m_rectInside.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.MidLeftChild)
                    {
                        clamped = new Point(Math.Min(m_rectInside.TopRight.X - 10d, point.X),
                            Math.Max(m_rectInside.TopRight.Y, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left + offsetSize.X,
                            m_rectInsideDrag.Top,
                            m_rectInsideDrag.Width - offsetSize.X,
                            m_rectInsideDrag.Height);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rect = new Rect(
                                  m_rectDrag.Left + offsetSize.X - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;
                case AnchorPoint.MidRight:
                case AnchorPoint.MidRightChild:
                    if (m_dragAnchor == AnchorPoint.MidRight)
                    {
                        clamped = new Point(Math.Max(m_rect.TopLeft.X + 10d, point.X),
                            Math.Max(m_rect.TopLeft.Y, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rect = new Rect(
                            m_rectDrag.Left,
                            m_rectDrag.Top,
                            m_rectDrag.Width + offsetSize.X,
                            m_rectDrag.Height);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rectInside = new Rect(
                                  m_rectInsideDrag.Left + m_offsetRectInsidePoint.X,
                                  m_rectInsideDrag.Top + m_offsetRectInsidePoint.Y,
                                 m_rectInside.Width,
                                 m_rectInside.Height);
                    }
                    else if (m_dragAnchor == AnchorPoint.MidRightChild)
                    {
                        clamped = new Point(Math.Max(m_rectInside.TopLeft.X + 10d, point.X),
                            Math.Max(m_rectInside.TopLeft.Y, point.Y));
                        offsetSize = new Point(clamped.X - m_dragStartPoint.X, clamped.Y - m_dragStartPoint.Y);
                        m_rectInside = new Rect(
                            m_rectInsideDrag.Left,
                            m_rectInsideDrag.Top,
                            m_rectInsideDrag.Width + offsetSize.X,
                            m_rectInsideDrag.Height);

                        m_offsetRectInsidePoint.X = (m_rect.Width - m_rectInside.Width) / 2 - m_offsetXYInsidePoint.X;
                        m_offsetRectInsidePoint.Y = (m_rect.Height - m_rectInside.Height) / 2 - m_offsetXYInsidePoint.Y;
                        m_rect = new Rect(
                                  m_rectDrag.Left - m_offsetRectInsidePoint.X,
                                  m_rectDrag.Top - m_offsetRectInsidePoint.Y,
                                 m_rect.Width,
                                 m_rect.Height);
                    }
                    break;

                case AnchorPoint.Rotation:
                    //var vecX = (point.X);
                    //var vecY = (-point.Y);

                    var vecX = (point.X - m_centerPoint.X);
                    var vecY = (m_centerPoint.Y - point.Y);

                    var len = Math.Sqrt(vecX * vecX + vecY * vecY);

                    var normX = vecX / len;
                    var normY = vecY / len;

                    //In rectangles's space, 
                    //compute dot product between, 
                    //Up and mouse-position vector
                    var dotProduct = (0 * normX + 1 * normY);
                    var angle = Math.Acos(dotProduct);

                    if (vecX < 0)
                        angle = -angle;

                    // Add (delta-radians) to rotation as degrees
                    m_rectRotation += (float)((180 / Math.PI) * angle);
                    if (m_rectRotation > 360 || m_rectRotation < -360)
                        m_rectRotation = 0;

                    break;

                case AnchorPoint.Center:
                case AnchorPoint.CenterChild:
                    //move this in screen-space
                    m_offsetRectPoint = new Point(e.GetPosition(this).X - m_dragStartOffsetPoint.X,
                        e.GetPosition(this).Y - m_dragStartOffsetPoint.Y);

                    break;
                default:
                    //if (child.IsMouseCaptured)
                    //{
                    //    var tt = GetTranslateTransform(child);
                    //    Vector v = start - e.GetPosition(this);
                    //    tt.X = origin.X - v.X;
                    //    tt.Y = origin.Y - v.Y;
                    //}
                    break;
            }
            this.InvalidateVisual();
        }
        private void ImageEx_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!m_bEnableSelectRectROITool && !m_bEnableSelectPolygonROITool && !m_bEnableSelectCircleROITool
                && !m_bEnableLocatorTool && !m_bEnableMeasureSegLineTool && !m_bEnableMeasureCircleTool)
                return;

            // Measure Segment Line
            if (m_bEnableMeasureSegLineTool)
            {
                var pointCur = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

                if (m_startPoint_MeasureSegLineTool == new Point(0, 0) && m_startPoint_MeasureSegLineTool == new Point(0, 0))
                {
                    // Measuring
                    m_bMeasuringSegLine = true;

                    // Select point 1
                    m_bMeasureSegLineSelectedPt1 = true;

                    m_startPoint_MeasureSegLineTool = pointCur;
                    m_endPoint_MeasureSegLineTool = pointCur;

                    this.InvalidateVisual();
                    return;
                }

                m_bMeasureSegLineSelectedPt2 = true;
                this.InvalidateVisual();

                return;
            }

            // Measure Circle
            if (m_bEnableMeasureCircleTool)
            {
                var pointCur = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

                if (m_centerPoint_MeasureCircleTool == new Point(0, 0) && m_radiusPoint_MeasureCircleTool == new Point(0, 0))
                {
                    // Measuring
                    m_bMeasuringCircle = true;

                    // Select point 1
                    m_bMeasureCircleSelectedPt1 = true;

                    m_centerPoint_MeasureCircleTool = pointCur;
                    m_radiusPoint_MeasureCircleTool = pointCur;

                    this.InvalidateVisual();
                    return;
                }

                m_bMeasureCircleSelectedPt2 = true;
                this.InvalidateVisual();

                return;
            }

            // Select Polygon
            if (m_bEnableSelectPolygonROITool)
            {
                m_bSelectingPolygonROI = true;
                m_bDrag = false;

                var pointCur = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
                m_listPointsPolygon.Add(pointCur);

                this.InvalidateVisual();

                return;
            }

            // Select Circle
            if (m_bEnableSelectCircleROITool)
            {
                var pointCur = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

                if (m_centerPointCircleROI == new Point(0, 0) && m_radiusPointCircleROI == new Point(0, 0))
                {
                    // Measuring
                    m_bSelectingCircleROI = true;

                    // Select point 1
                    m_bSelectedPt1CircleROI = true;

                    m_centerPointCircleROI = pointCur;
                    m_radiusPointCircleROI = pointCur;

                    this.InvalidateVisual();
                    return;
                }

                m_bSelectedPt2CircleROI = true;
                this.InvalidateVisual();

                return;
            }

            // Compute a Screen to Rectangle transform 

            var mat = new Matrix();
            if (m_bEnableRotate == true)
                mat.RotateAt(m_rectRotation, m_centerPoint.X, m_centerPoint.Y);
            mat.Translate(m_offsetRectPoint.X, m_offsetRectPoint.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            var rectOutside = m_rect;
            var rectInside = m_rectInside;

            if (m_bEnableLocatorTool)
            {
                if (rectOutside.Contains(point))
                {
                    m_bEnableSelectRect = true;
                    m_bEnableSelectRectInside = false;
                    this.InvalidateVisual();
                }
                if (rectInside.Contains(point) && rectOutside.Contains(point))
                {
                    m_bEnableSelectRectInside = true;
                    m_bEnableSelectRect = false;
                    this.InvalidateVisual();
                }
                else if (!rectInside.Contains(point) && !rectOutside.Contains(point))
                {
                    m_bEnableSelectRect = false;
                    m_bEnableSelectRectInside = false;
                    this.InvalidateVisual();
                }
            }
            var rectTopLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectTopRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectBottomLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectBottomRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidTop = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidBottom = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
            var ellipse = new EllipseGeometry(new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top - 20), m_dComWidth * 0.7, m_dComWidth * 0.7);

            var rectTopLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectTopRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectBottomLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectBottomRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidTopChild = new Rect(m_rectInside.Left + m_rectInside.Width / 2 - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidBottomChild = new Rect(m_rectInside.Left + m_rectInside.Width / 2 - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top + m_rectInside.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
            var rectMidRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top + m_rectInside.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);

            if (!m_bDrag)
            {
                //We're in Rectangle space now, so its simple box-point intersection test
                if (rectTopLeft.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.TopLeft;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectTopLeftChild.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.TopLeftChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectTopRight.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.TopRight;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectTopRightChild.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.TopRightChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectBottomLeft.Contains(point))
                {

                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.BottomLeft;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectBottomLeftChild.Contains(point))
                {

                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.BottomLeftChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectBottomRight.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.BottomRight;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectBottomRightChild.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.BottomRightChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidTop.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidTop;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidTopChild.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidTopChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidBottom.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidBottom;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidBottomChild.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidBottomChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidLeft.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidLeft;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidLeftChild.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidLeftChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidRight.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidRight;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectMidRightChild.Contains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.MidRightChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (ellipse.FillContains(point))
                {
                    m_bDrag = true;
                    m_dragStartPoint = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.Rotation;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_bSelectingRectROI = true;
                }
                else if (rectOutside.Contains(point))
                {
                    m_bDrag = true;
                    //imageEx.DragStart = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.Center;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_dragStartOffsetPoint = new Point(e.GetPosition(this).X - m_offsetRectPoint.X, e.GetPosition(this).Y - m_offsetRectPoint.Y);
                    m_bSelectingRectROI = true;
                }
                else if (rectInside.Contains(point))
                {
                    m_bDrag = true;
                    //imageEx.DragStart = new Point(point.X, point.Y);
                    m_dragAnchor = AnchorPoint.CenterChild;
                    m_rectDrag = new Rect(m_rect.Left, m_rect.Top, m_rect.Width, m_rect.Height);
                    m_rectInsideDrag = new Rect(m_rectInside.Left, m_rectInside.Top, m_rectInside.Width, m_rectInside.Height);
                    m_dragStartOffsetPoint = new Point(e.GetPosition(this).X - m_offsetRectPoint.X, e.GetPosition(this).Y - m_offsetRectPoint.Y);
                    m_bSelectingRectROI = true;
                }
                else
                {
                    //var tt = GetTranslateTransform(this);
                    //start = e.GetPosition(this);
                    //origin = new Point(tt.X, tt.Y);
                    //this.Cursor = Cursors.Hand;
                    //this.CaptureMouse();
                    m_bSelectingRectROI = false;
                }
            }
        }
        private void ImageEx_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            this.Cursor = Cursors.Arrow;
        }
        #endregion

        #region Public Properties
        public bool EnableSelectRectInside
        {
            get => m_bEnableSelectRectInside;
            set => m_bEnableSelectRectInside = value;
        }
        public bool EnableSelectRect
        {
            get => m_bEnableSelectRect;
            set => m_bEnableSelectRect = value;
        }
        public bool EnableLocatorTool
        {
            get => m_bEnableLocatorTool;
            set
            {
                if (SetProperty(ref m_bEnableLocatorTool, value))
                {
                    if (m_bEnableLocatorTool)
                    {
                        m_bEnableSelectRectROITool = false;
                        m_bEnableSelectPolygonROITool = false;
                        m_bEnableMeasureSegLineTool = false;
                        m_bSelectingRectROI = false;
                        m_bMeasuringSegLine = false;
                        m_bEnableSelectRect = false;
                        m_bEnableRotate = false;
                        m_bDrag = false;

                        ToolType = emToolType.ToolType_LocatorTool;

                        this.InvalidateVisual();
                    }
                }
            }
        }
        public bool EnableSelectROIRectTool
        {
            get => m_bEnableSelectRectROITool;
            set
            {
                if (SetProperty(ref m_bEnableSelectRectROITool, value))
                {
                    if (m_bEnableSelectRectROITool)
                    {
                        m_bEnableLocatorTool = false;
                        m_bEnableSelectCircleROITool = false;
                        m_bEnableSelectPolygonROITool = false;
                        m_bEnableMeasureSegLineTool = false;
                        m_bEnableMeasureCircleTool = false;
                        m_bEnableInspectTool = false;

                        m_bEnableSelectRect = true;
                        m_bEnableRotate = false;
                        m_bDrag = true;

                        ToolType = emToolType.ToolType_SelectROITool;

                        this.InvalidateVisual();
                    }
                }
            }
        }
        public bool EnableSelectROIPolygonTool
        {
            get => m_bEnableSelectPolygonROITool;
            set
            {
                if (SetProperty(ref m_bEnableSelectPolygonROITool, value))
                {
                    if (m_bEnableSelectPolygonROITool)
                    {
                        m_bEnableLocatorTool = false;
                        m_bEnableSelectRectROITool = false;
                        m_bEnableSelectCircleROITool = false;
                        m_bEnableMeasureSegLineTool = false;
                        m_bEnableMeasureCircleTool = false;
                        m_bEnableInspectTool = false;

                        ToolType = emToolType.ToolType_SelectROITool;

                        this.InvalidateVisual();
                    }
                }
            }
        }
        public bool EnableSelectROICircleTool
        {
            get => m_bEnableSelectCircleROITool;
            set
            {
                if (SetProperty(ref m_bEnableSelectCircleROITool, value))
                {
                    if (m_bEnableSelectCircleROITool)
                    {
                        m_bEnableLocatorTool = false;
                        m_bEnableSelectRectROITool = false;
                        m_bEnableSelectPolygonROITool = false;
                        m_bEnableMeasureSegLineTool = false;
                        m_bEnableMeasureCircleTool = false;
                        m_bEnableInspectTool = false;

                        ToolType = emToolType.ToolType_SelectROITool;

                        this.InvalidateVisual();
                    }
                }
            }
        }
        public bool EnableInspectTool
        {
            get => m_bEnableInspectTool;
            set
            {
                if (SetProperty(ref m_bEnableInspectTool, value))
                {

                }
            }
        }
        public bool EnableMeasureSegLineTool
        {
            get => m_bEnableMeasureSegLineTool;
            set
            {
                if (SetProperty(ref m_bEnableMeasureSegLineTool, value))
                {
                    if (m_bEnableMeasureSegLineTool)
                    {
                        m_bEnableLocatorTool = false;
                        m_bEnableSelectRectROITool = false;
                        m_bEnableSelectPolygonROITool = false;
                        m_bEnableMeasureCircleTool = false;
                        m_bSelectingRectROI = false;
                        m_bEnableSelectRect = false;

                        ToolType = emToolType.ToolType_MeasurementTool;

                        this.InvalidateVisual();
                    }
                }
            }
        }
        public bool EnableMeasureCircleTool
        {
            get => m_bEnableMeasureCircleTool;
            set
            {
                if (SetProperty(ref m_bEnableMeasureCircleTool, value))
                {
                    if (m_bEnableMeasureCircleTool)
                    {
                        m_bEnableLocatorTool = false;
                        m_bEnableSelectRectROITool = false;
                        m_bEnableSelectPolygonROITool = false;
                        m_bEnableMeasureSegLineTool = false;
                        m_bSelectingRectROI = false;
                        m_bEnableSelectRect = false;

                        ToolType = emToolType.ToolType_MeasurementTool;

                        this.InvalidateVisual();
                    }
                }
            }
        }
        public bool EnableRotate
        {
            get => m_bEnableRotate;
            set => m_bEnableRotate = value;
        }
        public bool IsSelectingRectROI
        {
            get => m_bSelectingRectROI;
            set => m_bSelectingRectROI = value;
        }
        public bool IsSelectingPolygonROI
        {
            get => m_bSelectingPolygonROI;
            set => m_bSelectingPolygonROI = value;
        }
        public bool IsSelectingCircleROI
        {
            get => m_bSelectingCircleROI;
            set => m_bSelectingCircleROI = value;
        }
        public bool IsMeasuringSegLine
        {
            get => m_bMeasuringSegLine;
            set => m_bMeasuringSegLine = value;
        }
        public bool IsMeasuringCircle
        {
            get => m_bMeasuringCircle;
            set => m_bMeasuringCircle = value;
        }
        public bool Drag
        {
            get => m_bDrag;
            set => m_bDrag = value;
        }
        public bool CompletedSelectROI
        {
            get => m_bCompletedSelectROI;
            set => m_bCompletedSelectROI = value;
        }
        public double RadiusCircleROI
        {
            get => m_dRadiusCircleROI;
            set => m_dRadiusCircleROI = value;
        }
        public Rect DragRect
        {
            get => m_rectDrag;
            set => m_rectDrag = value;
        }
        public Rect DragRectInside
        {
            get => m_rectInsideDrag;
            set => m_rectInsideDrag = value;
        }
        public Rect Rect
        {
            get => m_rect;
            set => m_rect = value;
        }
        public Rect RectReal
        {
            get => m_rectReal;
            set => m_rectReal = value;
        }
        public Rect RectInside
        {
            get => m_rectInside;
            set => m_rectInside = value;
        }
        public Rect RectTransform
        {
            get => m_rectTransform;
            set => m_rectTransform = value;
        }
        public Point CenterPoint
        {
            get => m_centerPoint;
            set => m_centerPoint = value;
        }
        public Point CenterPointReal
        {
            get => m_centerPointReal;
            set => m_centerPointReal = value;
        }
        public Point CenterPointCircleROI
        {
            get => m_centerPointCircleROI;
            set => m_centerPointCircleROI = value;
        }
        public Point DragStart
        {
            get => m_dragStartPoint;
            set => m_dragStartPoint = value;
        }
        public Point DragStartOffset
        {
            get => m_dragStartOffsetPoint;
            set => m_dragStartOffsetPoint = value;
        }
        public Point OffsetRect
        {
            get => m_offsetRectPoint;
            set => m_offsetRectPoint = value;
        }
        public Point OffsetRectInside
        {
            get => m_offsetRectInsidePoint;
            set => m_offsetRectInsidePoint = value;
        }
        public Point OffsetXYInside
        {
            get => m_offsetXYInsidePoint;
            set => m_offsetXYInsidePoint = value;
        }
        public Single RectRotation
        {
            get => m_rectRotation;
            set => m_rectRotation = value;
        }
        public Size DragSize
        {
            get => m_szDragSize;
            set => m_szDragSize = value;
        }
        public AnchorPoint DragAnchor
        {
            get => m_dragAnchor;
            set => m_dragAnchor = value;
        }
        public emToolType ToolType
        {
            get => m_toolType;
            set
            {
                if (SetProperty(ref m_toolType, value))
                {
                    // change mode tool
                }
            }
        }
        public System.Drawing.Bitmap BMP
        {
            get => m_bmp;
            set
            {
                if (SetProperty(ref m_bmp, value))
                {

                }
            }
        }
        public List<Point> PointsPolygon
        {
            get => m_listPointsPolygon;
            set => m_listPointsPolygon = value;
        }
        #endregion

        private void DrawCenterPt(DrawingContext dc, Point cntPt)
        {
            int seg = 10;
            int squareEdge = (int)(seg * Math.Tan(Math.PI / 4));

            Point p1 = new Point(cntPt.X + seg, cntPt.Y - squareEdge);
            Point p2 = new Point(cntPt.X - seg, cntPt.Y + squareEdge);
            dc.DrawLine(new Pen(Brushes.LightSalmon, 1), p1, p2);

            Point p3 = new Point(cntPt.X - seg, cntPt.Y - squareEdge);
            Point p4 = new Point(cntPt.X + seg, cntPt.Y + squareEdge);
            dc.DrawLine(new Pen(Brushes.LightSalmon, 1), p3, p4);

            dc.DrawEllipse(Brushes.LightSalmon, new Pen(Brushes.LightSalmon, 1), cntPt, m_dComWidth * 0.1, m_dComWidth * 0.1);
        }
        private void DrawAxis(DrawingContext dc, Point p1, Point p2, float scale = 0.2f)
        {
            double angle = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
            double hypotenuse = Math.Sqrt(Math.Pow((p1.Y - p2.Y), 2) + Math.Pow((p1.X - p2.X), 2));

            // Here we lengthen the arrow by a factor of scale
            p2.X = (int)(p1.X - scale * hypotenuse * Math.Cos(angle));
            p2.Y = (int)(p1.Y - scale * hypotenuse * Math.Sin(angle));
            dc.DrawLine(new Pen(Brushes.LightCoral, 1), p1, p2);

            // create the arrow hooks
            p1.X = (int)(p2.X + 9 * Math.Cos(angle + Math.PI / 4));
            p1.Y = (int)(p2.Y + 9 * Math.Sin(angle + Math.PI / 4));
            dc.DrawLine(new Pen(Brushes.LightCoral, 1), p1, p2);

            p1.X = (int)(p2.X + 9 * Math.Cos(angle - Math.PI / 4));
            p1.Y = (int)(p2.Y + 9 * Math.Sin(angle - Math.PI / 4));
            dc.DrawLine(new Pen(Brushes.LightCoral, 1), p1, p2);
        }
        private void RenderSelectRectROITool(DrawingContext dc)
        {
            //calculate center point
            m_centerPoint = new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top + m_rect.Height / 2);
            m_centerPointReal.X = m_centerPoint.X + m_offsetRectPoint.X;
            m_centerPointReal.Y = m_centerPoint.Y + m_offsetRectPoint.Y;
            m_rectReal = new Rect(new Point(m_rect.X + m_offsetRectPoint.X, m_rect.Y + m_offsetRectPoint.Y), new Size(m_rect.Width, m_rect.Height));

            //string puttext = "Angle: " + _rectRotation + "\n" + "Offset X: " + _offsetRect.X
            //    + " , " + "Offset Y: " + _offsetRect.Y + "\n"
            //    + "Width: " + _rect.Width
            //    + " , " + "Height: " + _rect.Height + "\n"
            //    + "Center Point: " + "X = " + _centerPoint.X + " , " + "Y = " + _centerPoint.Y + "\n"
            //    + "Center Point Real: " + "X = " + _centerPointReal.X + " , " + "Y = " + _centerPointReal.Y;

            //FormattedText formattedText = new FormattedText(puttext, new System.Globalization.CultureInfo(2),
            //FlowDirection.LeftToRight, new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Bold, FontStretches.ExtraExpanded), 10, Brushes.DarkBlue, 0.8);
            //dc.DrawText(formattedText, new Point(10, 10));

            Matrix mat1 = new Matrix();
            if (m_bEnableRotate == true)
                mat1.RotateAt(m_rectRotation, m_centerPoint.X, m_centerPoint.Y);
            mat1.Translate(m_offsetRectPoint.X, m_offsetRectPoint.Y);

            MatrixTransform matrixTransform1 = new MatrixTransform(mat1);
            dc.PushTransform(matrixTransform1);
            dc.PushOpacity(0.35);


            if (!m_bEnableSelectRect)
            {
                dc.DrawRectangle(Brushes.GreenYellow, new Pen(Brushes.LightYellow, 2), m_rect);
            }
            else
            {
                // All out gizmo rectangles are defined in Rectangle Parent Space
                Rect rectTopLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectTopRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectBottomLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectBottomRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidTop = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidBottom = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectCenter = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);

                //Draw Rectangle Parent
                dc.DrawRectangle(colorBgRect, new Pen(colorPen, m_dThicknessPen), m_rect);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectTopLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectTopRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectBottomLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectBottomRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidTop);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidBottom);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectCenter);


                //draw center
                dc.DrawLine(new Pen(colorCrossLine, m_dThicknessPen), new Point(m_centerPoint.X - 10d,
                    m_centerPoint.Y), new Point(m_centerPoint.X + 10d, m_centerPoint.Y));
                dc.DrawLine(new Pen(colorCrossLine, m_dThicknessPen), new Point(m_centerPoint.X,
                    m_centerPoint.Y - 10d), new Point(m_centerPoint.X, m_centerPoint.Y + 10d));
                //dc.DrawEllipse(colorCrossLine, null, _centerPoint, 2d, 2d);
            }
            if (m_bEnableRotate == true)
            {
                //3 point draw line and ellipse
                Point pointLine1 = new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top - m_dComOffset);
                Point pointLine2 = new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top - 40);
                Point pointCenterEllipse = new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top - 40);

                //draw line rotate
                dc.DrawLine(new Pen(colorPen, m_dThicknessPen), pointLine1, pointLine2);
                //draw ellipse rotate
                dc.DrawEllipse(colorBgCorner, new Pen(colorPen, m_dThicknessPen), pointCenterEllipse, m_dComWidth * 0.6, m_dComWidth * 0.6);
            }

            dc.Pop();
        }
        private void RenderSelectRoiTool(DrawingContext dc)
        {
            RenderSelectRectROITool(dc);
        }
        private void RenderLocatorTool(DrawingContext dc)
        {
            //calculate center point
            m_centerPoint = new Point(m_rect.Left + m_rect.Width / 2, m_rect.Top + m_rect.Height / 2);
            m_centerPointReal.X = m_centerPoint.X + m_offsetRectPoint.X;
            m_centerPointReal.Y = m_centerPoint.Y + m_offsetRectPoint.Y;
            m_rectReal = new Rect(new Point(m_rect.X + m_offsetRectPoint.X, m_rect.Y + m_offsetRectPoint.Y), new Size(m_rect.Width, m_rect.Height));


            Matrix mat1 = new Matrix();
            mat1.Translate(m_offsetRectPoint.X, m_offsetRectPoint.Y);

            MatrixTransform matrixTransform1 = new MatrixTransform(mat1);
            dc.PushTransform(matrixTransform1);
            dc.PushOpacity(0.35);

            if (!m_bEnableSelectRect)
            {
                dc.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.LightYellow, 2), m_rect);
            }
            else
            {
                // All out gizmo rectangles are defined in Rectangle Parent Space
                Rect rectTopLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectTopRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectBottomLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectBottomRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidTop = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidBottom = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top + m_rect.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidLeft = new Rect(m_rect.Left - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidRight = new Rect(m_rect.Left + m_rect.Width - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectCenter = new Rect(m_rect.Left + m_rect.Width / 2 - m_dComOffset, m_rect.Top + m_rect.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);

                //Draw Rectangle Parent
                dc.DrawRectangle(colorBgRect, new Pen(colorPen, m_dThicknessPen), m_rect);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectTopLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectTopRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectBottomLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectBottomRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidTop);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidBottom);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectMidRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, m_dThicknessPen), rectCenter);


                //draw center
                dc.DrawLine(new Pen(colorCrossLine, m_dThicknessPen), new Point(m_centerPoint.X - 10d,
                    m_centerPoint.Y), new Point(m_centerPoint.X + 10d, m_centerPoint.Y));
                dc.DrawLine(new Pen(colorCrossLine, m_dThicknessPen), new Point(m_centerPoint.X,
                    m_centerPoint.Y - 10d), new Point(m_centerPoint.X, m_centerPoint.Y + 10d));
                //dc.DrawEllipse(Brushes.Red, null, _centerPoint, 2d, 2d);
            }

            if (!m_bEnableSelectRectInside)
            {
                dc.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.OrangeRed, 2), m_rectInside);
            }
            else
            {
                // All out gizmo rectangles are defined in Rectangle Child Space
                Rect rectTopLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectTopRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectBottomLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectBottomRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidTopChild = new Rect(m_rectInside.Left + m_rectInside.Width / 2 - m_dComOffset, m_rectInside.Top - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidBottomChild = new Rect(m_rectInside.Left + m_rectInside.Width / 2 - m_dComOffset, m_rectInside.Top + m_rectInside.Height - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidLeftChild = new Rect(m_rectInside.Left - m_dComOffset, m_rectInside.Top + m_rectInside.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
                Rect rectMidRightChild = new Rect(m_rectInside.Left + m_rectInside.Width - m_dComOffset, m_rectInside.Top + m_rectInside.Height / 2 - m_dComOffset, m_dComWidth, m_dComWidth);
                //var rectCenterChild = new Rect(_rectChild.Left + _rectChild.Width / 2 - 10f, _rectChild.Top + _rectChild.Height / 2 - 10f, 20f, 20f);

                //Draw Rectangle Child
                dc.DrawRectangle(Brushes.LightSlateGray, new Pen(Brushes.OrangeRed, m_dThicknessPen), m_rectInside);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectTopLeftChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectTopRightChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectBottomLeftChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectBottomRightChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectMidTopChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectMidBottomChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectMidLeftChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, m_dThicknessPen), rectMidRightChild);
                //dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Black, 1), rectCenterChild);
            }
            dc.Pop();
        }
        private void RenderSelectPolyROITool(DrawingContext dc)
        {
            if (BMP == null)
                return;

            if (m_listPointsPolygon.Count < 1)
                return;

            dc.PushOpacity(1.0);
            double dThicknessPen = 2.0;

            Pen penDash = new Pen(colorCrossLine, dThicknessPen);
            penDash.DashStyle = DashStyles.Dash;
            penDash.DashCap = PenLineCap.Flat;
            penDash.LineJoin = PenLineJoin.Miter;

            int nIdxPtEnd = m_listPointsPolygon.Count - 1;

            Point startPt = m_listPointsPolygon[nIdxPtEnd];

            if (!m_bCompletedSelectROI && m_bDrag)
            {
                dc.DrawLine(penDash, startPt, m_currentPoint);

                dc.DrawEllipse(Brushes.Red, null, startPt, 3d, 3d);
                dc.DrawEllipse(Brushes.Red, null, m_currentPoint, 3d, 3d);
            }
            else if (!m_bCompletedSelectROI && !m_bDrag)
            {
                if (m_listPointsPolygon.Count < 2)
                    return;

                Point startPoint = m_listPointsPolygon[0];
                Point nextPoint = m_listPointsPolygon[1];
                for (int i = 1; i < m_listPointsPolygon.Count; i++)
                {
                    nextPoint = m_listPointsPolygon[i];

                    dc.DrawLine(new Pen(Brushes.Blue, dThicknessPen), startPoint, nextPoint);
                    dc.DrawEllipse(Brushes.Red, null, startPoint, 3d, 3d);
                    dc.DrawEllipse(Brushes.Red, null, nextPoint, 3d, 3d);

                    startPoint = nextPoint;
                }
            }


            dc.Pop();
        }
        private void RenderSelectCircleROITool(DrawingContext dc)
        {
            if (BMP == null)
                return;

            dc.PushOpacity(1.0);
            double dThicknessPen = 2.0;

            Pen penDash = new Pen(colorCrossLine, dThicknessPen);
            penDash.DashStyle = DashStyles.Dash;
            penDash.DashCap = PenLineCap.Flat;
            penDash.LineJoin = PenLineJoin.Miter;

            double dDeltaX = Math.Round(Math.Abs(m_centerPointCircleROI.X - m_radiusPointCircleROI.X), 1);
            double dDeltaY = Math.Round(Math.Abs(m_centerPointCircleROI.Y - m_radiusPointCircleROI.Y), 1);
            double dRadius = Math.Round(Math.Sqrt(dDeltaX * dDeltaX + dDeltaY * dDeltaY), 1);

            if (m_bSelectedPt1CircleROI && !m_bSelectedPt2CircleROI)
            {
                // draw dash line center to radius point
                dc.DrawLine(penDash, m_centerPointCircleROI, m_radiusPointCircleROI);

                // draw Circle
                dc.DrawEllipse(null, new Pen(colorCrossLine, dThicknessPen), m_centerPointCircleROI, (int)dRadius, (int)dRadius);

                // draw center point
                dc.DrawEllipse(Brushes.Red, null, m_centerPointCircleROI, 3d, 3d);

                // draw radius point
                dc.DrawEllipse(Brushes.Red, null, m_radiusPointCircleROI, 3d, 3d);
            }

            if (m_bSelectedPt1CircleROI && m_bSelectedPt2CircleROI)
            {
                dc.PushOpacity(0.6);

                // draw Circle
                dc.DrawEllipse(colorBgRect, new Pen(Brushes.Cyan, dThicknessPen), m_centerPointCircleROI, (int)dRadius, (int)dRadius);
            }

            dc.Pop();
        }

        [Obsolete]
        private void RenderMeasureSegLineTool(DrawingContext dc)
        {
            if (BMP == null)
                return;

            if (m_startPoint_MeasureSegLineTool.X == 0 && m_endPoint_MeasureSegLineTool.X == 0 &&
                m_endPoint_MeasureSegLineTool.Y == 0 && m_endPoint_MeasureSegLineTool.Y == 0)
            {
                dc.PushOpacity(0.5);

                dc.DrawText(new FormattedText("MOUSE CLICK start to measure segment line", CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Segoe UI"), FontStyles.Oblique, FontWeights.Bold, FontStretches.Normal), 26, Brushes.OrangeRed), new Point(200, 200));
                dc.Pop();
                return;
            }

            dc.PushOpacity(1.0);
            double dThicknessPen = 2.0;

            Pen penDash = new Pen(colorCrossLine, dThicknessPen);
            penDash.DashStyle = DashStyles.Dash;
            penDash.DashCap = PenLineCap.Flat;
            penDash.LineJoin = PenLineJoin.Miter;

            // cross line 1
            dc.DrawLine(new Pen(Brushes.Cyan, dThicknessPen), new Point(m_startPoint_MeasureSegLineTool.X,
                0), new Point(m_startPoint_MeasureSegLineTool.X, BMP.Height));
            dc.DrawLine(new Pen(Brushes.Cyan, dThicknessPen), new Point(0, m_startPoint_MeasureSegLineTool.Y),
                new Point(BMP.Width, m_startPoint_MeasureSegLineTool.Y));
            dc.DrawEllipse(Brushes.Red, null, m_startPoint_MeasureSegLineTool, 3d, 3d);

            // cross line 2
            dc.DrawLine(new Pen(Brushes.Magenta, dThicknessPen), new Point(m_endPoint_MeasureSegLineTool.X,
                0), new Point(m_endPoint_MeasureSegLineTool.X, BMP.Height));
            dc.DrawLine(new Pen(Brushes.Magenta, dThicknessPen), new Point(0, m_endPoint_MeasureSegLineTool.Y),
                new Point(BMP.Width, m_endPoint_MeasureSegLineTool.Y));
            dc.DrawEllipse(Brushes.Red, null, m_endPoint_MeasureSegLineTool, 3d, 3d);

            // draw dash line distance
            dc.DrawLine(penDash, m_startPoint_MeasureSegLineTool, m_endPoint_MeasureSegLineTool);

            if (m_bMeasureSegLineSelectedPt1 && m_bMeasureSegLineSelectedPt2)
            {
                double deltaX = Math.Round(Math.Abs(m_endPoint_MeasureSegLineTool.X - m_startPoint_MeasureSegLineTool.X), 1);
                double deltaY = Math.Round(Math.Abs(m_endPoint_MeasureSegLineTool.Y - m_startPoint_MeasureSegLineTool.Y), 1);
                double hypotenuse = Math.Round((Math.Sqrt(deltaX * deltaX + deltaY * deltaY)), 1);

                Point pointPutText = new Point(0, 0);
                if (m_endPoint_MeasureSegLineTool.X > m_startPoint_MeasureSegLineTool.X)
                {
                    pointPutText = new Point(m_endPoint_MeasureSegLineTool.X + 10, m_endPoint_MeasureSegLineTool.Y + 10);
                }
                else
                {
                    pointPutText = new Point(m_endPoint_MeasureSegLineTool.X + deltaX + 10, m_endPoint_MeasureSegLineTool.Y + 10);
                }

                dc.DrawText(new FormattedText(string.Format("Start Point: X = {0} | Y = {1}", (int)m_startPoint_MeasureSegLineTool.X, (int)m_startPoint_MeasureSegLineTool.Y),
                    CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), pointPutText);
                dc.DrawText(new FormattedText(string.Format("End Point: X = {0} | Y = {1}", (int)m_endPoint_MeasureSegLineTool.X, (int)m_endPoint_MeasureSegLineTool.Y),
                   CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), new Point(pointPutText.X, pointPutText.Y + 20));
                dc.DrawText(new FormattedText(string.Format("Delta X = {0} pxl", deltaX),
                   CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), new Point(pointPutText.X, pointPutText.Y + 40));
                dc.DrawText(new FormattedText(string.Format("Delta Y = {0} pxl", deltaY),
                   CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), new Point(pointPutText.X, pointPutText.Y + 60));
                dc.DrawText(new FormattedText(string.Format("Hypotenuse = {0} pxl", hypotenuse),
                   CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), new Point(pointPutText.X, pointPutText.Y + 80));
            }

            dc.Pop();
        }
        [Obsolete]
        private void RenderMeasureCircleTool(DrawingContext dc)
        {
            if (BMP == null)
                return;

            if (m_centerPoint_MeasureCircleTool.X == 0 && m_centerPoint_MeasureCircleTool.X == 0 &&
                m_radiusPoint_MeasureCircleTool.Y == 0 && m_radiusPoint_MeasureCircleTool.Y == 0)
            {
                dc.PushOpacity(0.5);

                dc.DrawText(new FormattedText("MOUSE CLICK start to measure circle", CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Segoe UI"), FontStyles.Oblique, FontWeights.Bold, FontStretches.Normal), 26, Brushes.OrangeRed), new Point(200, 200));
                dc.Pop();
                return;
            }

            dc.PushOpacity(1.0);
            double dThicknessPen = 2.0;

            double dDeltaX = Math.Round(Math.Abs(m_centerPoint_MeasureCircleTool.X - m_radiusPoint_MeasureCircleTool.X), 1);
            double dDeltaY = Math.Round(Math.Abs(m_centerPoint_MeasureCircleTool.Y - m_radiusPoint_MeasureCircleTool.Y), 1);
            double dRadius = Math.Round(Math.Sqrt(dDeltaX * dDeltaX + dDeltaY * dDeltaY), 1);

            Pen penDash = new Pen(colorCrossLine, dThicknessPen);
            penDash.DashStyle = DashStyles.Dash;
            penDash.DashCap = PenLineCap.Flat;
            penDash.LineJoin = PenLineJoin.Miter;

            // cross line 1
            dc.DrawLine(penDash, new Point(0, m_centerPoint_MeasureCircleTool.Y + (int)dRadius),
                new Point(BMP.Width, m_centerPoint_MeasureCircleTool.Y + (int)dRadius));
            dc.DrawLine(penDash, new Point(0, m_centerPoint_MeasureCircleTool.Y - (int)dRadius),
                new Point(BMP.Width, m_centerPoint_MeasureCircleTool.Y - (int)dRadius));

            // cross line 2
            dc.DrawLine(penDash, new Point(m_centerPoint_MeasureCircleTool.X + (int)dRadius, 0),
                new Point(m_centerPoint_MeasureCircleTool.X + (int)dRadius, BMP.Height));
            dc.DrawLine(penDash, new Point(m_centerPoint_MeasureCircleTool.X - (int)dRadius, 0),
                new Point(m_centerPoint_MeasureCircleTool.X - (int)dRadius, BMP.Height));

            // draw dash line center to radius point
            dc.DrawLine(penDash, m_centerPoint_MeasureCircleTool, m_radiusPoint_MeasureCircleTool);

            // draw Circle
            dc.DrawEllipse(null, new Pen(Brushes.Cyan, dThicknessPen), m_centerPoint_MeasureCircleTool, (int)dRadius, (int)dRadius);

            // draw center point
            dc.DrawEllipse(Brushes.Red, null, m_centerPoint_MeasureCircleTool, 3d, 3d);

            // draw radius point
            dc.DrawEllipse(Brushes.Red, null, m_radiusPoint_MeasureCircleTool, 3d, 3d);

            if (m_bMeasureCircleSelectedPt1 && m_bMeasureCircleSelectedPt2)
            {
                Point pointPutText = new Point(0, 0);
                if (BMP.Width - m_centerPoint_MeasureCircleTool.X <= 350)
                {
                    pointPutText = new Point(m_centerPoint_MeasureCircleTool.X - (int)dRadius - 250, m_centerPoint_MeasureCircleTool.Y - (int)dRadius - 30);
                }
                else
                {
                    pointPutText = new Point(m_centerPoint_MeasureCircleTool.X + (int)dRadius + 10, m_centerPoint_MeasureCircleTool.Y - (int)dRadius - 30);
                }

                dc.DrawText(new FormattedText(string.Format("Center Point: X = {0} | Y = {1}", (int)m_centerPoint_MeasureCircleTool.X, (int)m_centerPoint_MeasureCircleTool.Y),
                    CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), new Point(pointPutText.X, pointPutText.Y - 40));
                dc.DrawText(new FormattedText(string.Format("End Point: X = {0} | Y = {1}", (int)m_radiusPoint_MeasureCircleTool.X, (int)m_radiusPoint_MeasureCircleTool.Y),
                    CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), new Point(pointPutText.X, pointPutText.Y - 20));
                dc.DrawText(new FormattedText(string.Format("Radius = {0} pxl", dRadius),
                    CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 16, Brushes.OrangeRed), pointPutText);
            }

            dc.Pop();
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (m_bEnableSelectRectROITool && !m_bCompletedSelectROI)
            {
                RenderSelectRoiTool(dc);
            }
            else if (m_bEnableLocatorTool && !m_bCompletedSelectROI)
            {
                RenderLocatorTool(dc);
            }
            else if (m_bEnableMeasureSegLineTool)
            {
                RenderMeasureSegLineTool(dc);
            }
            else if (m_bEnableMeasureCircleTool)
            {
                RenderMeasureCircleTool(dc);
            }
            else if (m_bEnableSelectPolygonROITool)
            {
                RenderSelectPolyROITool(dc);
            }
            else if(m_bEnableSelectCircleROITool)
            {
                RenderSelectCircleROITool(dc);
            }
        }
    }
    public enum emToolType
    {
        ToolType_Default,
        ToolType_SelectROITool,
        ToolType_LocatorTool,
        ToolType_MeasurementTool,
    }
}
