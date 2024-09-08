using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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

        AnchorPoint _dragAnchor = AnchorPoint.None;
        HitType _mouseHitType = HitType.None;

        #region Solidbrush
        SolidColorBrush colorBgRect = (SolidColorBrush)new BrushConverter().ConvertFrom("#3d424d");
        SolidColorBrush colorBgCorner = (SolidColorBrush)new BrushConverter().ConvertFrom("#ce3b3f");
        SolidColorBrush colorPen = (SolidColorBrush)new BrushConverter().ConvertFrom("#F3E9DF");
        SolidColorBrush colorCrossLine = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD764");
        double _thicknessPen = 0.5;
        #endregion

        #region Event
        public delegate void MouseMove_Handler(int nX, int nY, int r, int g, int b);
        public event MouseMove_Handler MouseMoveEndEvent;

        public static readonly RoutedEvent SelectedROIEvent = EventManager.RegisterRoutedEvent(
            "SelectedROI",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler SelectedROI
        {
            add
            {
                base.AddHandler(SelectedROIEvent, value);
            }
            remove
            {
                base.RemoveHandler(SelectedROIEvent, value);
            }
        }

        public static readonly RoutedEvent SaveFullImageEvent = EventManager.RegisterRoutedEvent(
            "SaveFullImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler SaveFullImage
        {
            add
            {
                base.AddHandler(SaveFullImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(SaveFullImageEvent, value);
            }
        }

        public static readonly RoutedEvent SaveROIImageEvent = EventManager.RegisterRoutedEvent(
            "SaveROIImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler SaveROIImage
        {
            add
            {
                base.AddHandler(SaveROIImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(SaveROIImageEvent, value);
            }
        }

        public static readonly RoutedEvent TrainLocatorEvent = EventManager.RegisterRoutedEvent(
            "TrainLocator",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler TrainLocator
        {
            add
            {
                base.AddHandler(TrainLocatorEvent, value);
            }
            remove
            {
                base.RemoveHandler(TrainLocatorEvent, value);
            }
        }

        public static readonly RoutedEvent FitEvent = EventManager.RegisterRoutedEvent(
            "Fit",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt_Basic));
        public event RoutedEventHandler Fit
        {
            add
            {
                base.AddHandler(FitEvent, value);
            }
            remove
            {
                base.RemoveHandler(FitEvent, value);
            }
        }
        #endregion

        #region Member Variables
        private bool _drag;
        private bool _completedSelectRoi;
        private bool _enableSelectRoiTool;
        private bool _isSelectingRoi;
        private bool _enableRotate;
        private bool _enableLocatorTool;
        private bool _enableSelectRect;
        private bool _enableSelectRectInside;
        private bool _enableSelectPoly;
        private Size _dragSize;
        private Point _dragStart;
        private Point _dragStartOffset;
        private Rect _dragRect;
        private Rect _dragRectInside;
        private Rect _rect;
        private Rect _rectReal;
        private Rect _rectInside;
        private Rect _rectTransform;
        private Point _centerPoint;
        private Point _centerPointReal;
        private Point _offsetRect;
        private Point _offsetRectInside;
        private Point _offsetXYInside;
        private Single _rectRotation;

        private EnToolType _toolType = EnToolType.ToolType_Default;
        private EnGrabMode _grabMode = EnGrabMode.GrabMode_SingleGrab;
        private EnViewMode _viewMode = EnViewMode.ViewMode_CreateRecipe;

        private double _comWidth = 20;
        private double _comOffset = 10;

        private System.Drawing.Bitmap m_bmp;
        #endregion

        #region Methods
        public void SetHitType(MouseEventArgs e)
        {
            // Compute a Screen to Rectangle transform 

            var mat = new Matrix();
            mat.RotateAt(_rectRotation, _centerPoint.X, _centerPoint.Y);
            mat.Translate(_offsetRect.X, _offsetRect.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            var rect = _rect;
            var rectTopLeft = new Rect(_rect.Left - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
            var rectTopRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
            var rectBottomLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
            var rectBottomRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
            var rectMidTop = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
            var rectMidBottom = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
            var rectMidLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
            var rectMidRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
            var ellipse = new EllipseGeometry(new Point(_rect.Left + _rect.Width / 2, _rect.Top - 20), _comWidth * 0.7, _comWidth * 0.7);

            Rect rectChild = new Rect();
            Rect rectTopLeftChild = new Rect();
            Rect rectTopRightChild = new Rect();
            Rect rectBottomLeftChild = new Rect();
            Rect rectBottomRightChild = new Rect();
            Rect rectMidTopChild = new Rect();
            Rect rectMidBottomChild = new Rect();
            Rect rectMidLeftChild = new Rect();
            Rect rectMidRightChild = new Rect();
            if (_enableLocatorTool)
            {
                rectChild = _rectInside;
                rectTopLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
                rectTopRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
                rectBottomLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
                rectBottomRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
                rectMidTopChild = new Rect(_rectInside.Left + _rectInside.Width / 2 - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
                rectMidBottomChild = new Rect(_rectInside.Left + _rectInside.Width / 2 - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
                rectMidLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top + _rectInside.Height / 2 - _comOffset, _comWidth, _comWidth);
                rectMidRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top + _rectInside.Height / 2 - _comOffset, _comWidth, _comWidth);
            }

            if (rectTopLeft.Contains(point) || rectTopLeftChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.TopLeft;
                _mouseHitType = HitType.TopLeftChild;
                SetMouseCusor();
            }
            else if (rectTopRight.Contains(point) || rectTopRightChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.TopRight;
                _mouseHitType = HitType.TopRightChild;
                SetMouseCusor();
            }
            else if (rectBottomLeft.Contains(point) || rectBottomLeftChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.BottomLeft;
                _mouseHitType = HitType.BottomLeftChild;
                SetMouseCusor();
            }
            else if (rectBottomRight.Contains(point) || rectBottomRightChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.BottomRight;
                _mouseHitType = HitType.BottomRightChild;
                SetMouseCusor();
            }
            else if (rectMidTop.Contains(point) || rectMidTopChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.MidTop;
                _mouseHitType = HitType.MidTopChild;
                SetMouseCusor();
            }
            else if (rectMidBottom.Contains(point) || rectMidBottomChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.MidBottom;
                _mouseHitType = HitType.MidBottomChild;
                SetMouseCusor();
            }
            else if (rectMidLeft.Contains(point) || rectMidLeftChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.MidLeft;
                _mouseHitType = HitType.MidLeftChild;
                SetMouseCusor();
            }
            else if (rectMidRight.Contains(point) || rectMidRightChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.MidRight;
                _mouseHitType = HitType.MidRightChild;
                SetMouseCusor();
            }
            else if (ellipse.FillContains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.Rotate;
                SetMouseCusor();
            }
            else if (rect.Contains(point) || rectChild.Contains(point))
            {
                _isSelectingRoi = true;
                _mouseHitType = HitType.Body;
                _mouseHitType = HitType.BodyChild;
                SetMouseCusor();
            }

            else
            {
                _isSelectingRoi = false;
                _mouseHitType = HitType.None;
                _mouseHitType = HitType.NoneChild;
                SetMouseCusor();
            }
        }
        public void SetMouseCusor()
        {
            Cursor desired_cursor = Cursors.Arrow;
            switch (_mouseHitType)
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
            _rect = new Rect(new Point(100, 100), new Size(220, 160));
            _rectTransform = _rect;
            _offsetRect = new Point(0, 0);
            _rectRotation = 0;
            _centerPoint = new Point(_rect.Left + _rect.Width / 2, _rect.Top + _rect.Height / 2);
            _centerPointReal = new Point(_centerPoint.X + _offsetRect.X, _centerPoint.Y + _offsetRect.Y);
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
        #endregion

        #region Draw Polygon

        #endregion

        #region Constructor
        public ImageExt_Basic()
        {
            _rect = new Rect(new Point(100, 100), new Size(220, 160));
            _rectReal = new Rect(new Point(_rect.X + _offsetRect.X, _rect.Y + _offsetRect.Y), new Size(_rect.Width, _rect.Height));
            _rectInside = new Rect(new Point(_rect.X + 60, _rect.Y + 40), new Size(100, 80));
            _rectTransform = _rect;
            _offsetXYInside = new Point((_rect.Width - _rectInside.Width) / 2, (_rect.Height - _rectInside.Height) / 2);
            _offsetRect = new Point(0, 0);
            _rectRotation = 0;
            _centerPoint = new Point(_rect.Left + _rect.Width / 2, _rect.Top + _rect.Height / 2);
            _centerPointReal = new Point(_centerPoint.X + _offsetRect.X, _centerPoint.Y + _offsetRect.Y);


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
        private ContextMenu _ctxMnuDefault;

        // context menu in Select Roi tool mode and locator tool mode
        private ContextMenu _ctxMnuRoiMode;

        // context menu in Locator tool
        private ContextMenu _ctxMnuLocator;

        // Get roi and save image
        private MenuItem _getROIItem;
        private MenuItem _saveFullImageItem;
        private MenuItem _saveROIImageItem;

        // default mode
        private MenuItem _fitItem;
        private MenuItem _zoomInItem;
        private MenuItem _zoomOutItem;
        private MenuItem _measureItem;
        private MenuItem _selectModeTool;
        private MenuItem _selectRoiMode;
        private MenuItem _selectRectItem;
        private MenuItem _selectPolyItem;
        private MenuItem _locatorMode;

        // the chooses in locator contextmenu
        private MenuItem _trainItem;

        // method
        private void InitContextMenuRoiMode()
        {
            _ctxMnuRoiMode = new ContextMenu();

            _getROIItem = new MenuItem();
            _getROIItem.Header = "Get ROI";
            _getROIItem.Name = "mnuGetROI";
            _getROIItem.Click += mnuGetROI_Click;
            _getROIItem.FontFamily = new FontFamily("Georgia");
            _getROIItem.FontWeight = FontWeights.Regular;
            _getROIItem.FontSize = 12;

            _saveFullImageItem = new MenuItem();
            _saveFullImageItem.Header = "Save Full Image";
            _saveFullImageItem.Name = "mnuSaveFullImage";
            _saveFullImageItem.Click += mnuSaveFullImage_Click;
            _saveFullImageItem.FontFamily = new FontFamily("Georgia");
            _saveFullImageItem.FontWeight = FontWeights.Regular;
            _saveFullImageItem.FontSize = 12;

            _saveROIImageItem = new MenuItem();
            _saveROIImageItem.Header = "Save ROI Image";
            _saveROIImageItem.Name = "mnuSaveROIImage";
            _saveROIImageItem.Click += mnuSaveROIImage_Click;
            _saveROIImageItem.FontFamily = new FontFamily("Georgia");
            _saveROIImageItem.FontWeight = FontWeights.Regular;
            _saveROIImageItem.FontSize = 12;

            _ctxMnuRoiMode.Items.Add(_getROIItem);
            _ctxMnuRoiMode.Items.Add(_saveFullImageItem);
            _ctxMnuRoiMode.Items.Add(_saveROIImageItem);
            _ctxMnuRoiMode.PlacementTarget = this;
            _ctxMnuRoiMode.IsOpen = false;
        }
        private void InitContextMenuDefault()
        {
            _ctxMnuDefault = new ContextMenu();

            _fitItem = new MenuItem();
            _fitItem.Header = "Fit";
            _fitItem.Name = "mnuFit";
            _fitItem.Click += mnuFit_Click;
            _fitItem.FontFamily = new FontFamily("Georgia");
            _fitItem.FontWeight = FontWeights.Regular;
            _fitItem.FontSize = 12;

            _zoomInItem = new MenuItem();
            _zoomInItem.Header = "Zoom in";
            _zoomInItem.Name = "mnuZoomIn";
            _zoomInItem.Click += mnuZoomIn_Click;
            _zoomInItem.FontFamily = new FontFamily("Georgia");
            _zoomInItem.FontWeight = FontWeights.Regular;
            _zoomInItem.FontSize = 12;

            _zoomOutItem = new MenuItem();
            _zoomOutItem.Header = "Zoom out";
            _zoomOutItem.Name = "mnuZoomOut";
            _zoomOutItem.Click += mnuZoomOut_Click;
            _zoomOutItem.FontFamily = new FontFamily("Georgia");
            _zoomOutItem.FontWeight = FontWeights.Regular;
            _zoomOutItem.FontSize = 12;

            // measure item
            _measureItem = new MenuItem();
            _measureItem.Header = "Measure";
            _measureItem.Name = "mnuMeasure";
            _measureItem.Click += mnuMeasure_Click;
            _measureItem.FontFamily = new FontFamily("Georgia");
            _measureItem.FontWeight = FontWeights.Regular;
            _measureItem.FontSize = 12;

            // select mode tool item
            _selectModeTool = new MenuItem();
            _selectModeTool = new MenuItem();
            _selectModeTool.Header = "Mode Tool";
            _selectModeTool.Name = "mnuModeTool";
            _selectModeTool.FontFamily = new FontFamily("Georgia");
            _selectModeTool.FontWeight = FontWeights.Regular;
            _selectModeTool.FontSize = 12;

            // select Roi mode
            _selectRoiMode = new MenuItem();
            _selectRoiMode = new MenuItem();
            _selectRoiMode.Header = "Select ROI";
            _selectRoiMode.Name = "mnuSelectRoi";
            _selectRoiMode.FontFamily = new FontFamily("Georgia");
            _selectRoiMode.FontWeight = FontWeights.Regular;
            _selectRoiMode.FontSize = 12;

            // select rectangle
            _selectRectItem = new MenuItem();
            _selectRectItem.Header = "Rectangle";
            _selectRectItem.Name = "mnuSelectRect";
            _selectRectItem.Click += mnuSelectRect_Click;
            _selectRectItem.FontFamily = new FontFamily("Georgia");
            _selectRectItem.FontWeight = FontWeights.Regular;
            _selectRectItem.FontSize = 12;
            _selectRectItem.IsCheckable = true;

            // select polygon
            _selectPolyItem = new MenuItem();
            _selectPolyItem.Header = "Polygon";
            _selectPolyItem.Name = "mnuSelectPolygon";
            _selectPolyItem.Click += mnuSelectPolygon_Click;
            _selectPolyItem.FontFamily = new FontFamily("Georgia");
            _selectPolyItem.FontWeight = FontWeights.Regular;
            _selectPolyItem.FontSize = 12;
            _selectPolyItem.IsCheckable = true;

            // add 2 item select rectangle and polygon into menu Roi mode
            _selectRoiMode.Items.Add(_selectRectItem);
            _selectRoiMode.Items.Add(_selectPolyItem);


            // locator mode
            _locatorMode = new MenuItem();
            _locatorMode.Header = "Locator";
            _locatorMode.Name = "mnuLocator";
            _locatorMode.Click += mnuLocator_Click;
            _locatorMode.FontFamily = new FontFamily("Georgia");
            _locatorMode.FontWeight = FontWeights.Regular;
            _locatorMode.FontSize = 12;

            // add 2 mode: select Roi and locator into select mode tool item
            _selectModeTool.Items.Add(_selectRoiMode);
            _selectModeTool.Items.Add(_locatorMode);


            _ctxMnuDefault.Items.Add(_fitItem);
            _ctxMnuDefault.Items.Add(_zoomInItem);
            _ctxMnuDefault.Items.Add(_zoomOutItem);
            _ctxMnuDefault.Items.Add(_measureItem);
            _ctxMnuDefault.Items.Add(_selectModeTool);
            _ctxMnuDefault.PlacementTarget = this;
            _ctxMnuDefault.IsOpen = false;

        }
        private void InitContextMenuLocator()
        {
            _ctxMnuLocator = new ContextMenu();

            _trainItem = new MenuItem();
            _trainItem = new MenuItem();
            _trainItem.Header = "Train";
            _trainItem.Name = "mnuTrain";
            _trainItem.Click += mnuTrain_Click;
            _trainItem.FontFamily = new FontFamily("Georgia");
            _trainItem.FontWeight = FontWeights.Regular;
            _trainItem.FontSize = 12;

            _ctxMnuLocator.Items.Add(_trainItem);
            _ctxMnuLocator.PlacementTarget = this;
            _ctxMnuLocator.IsOpen = false;
        }

        private void mnuFit_Click(object sender, RoutedEventArgs e)
        {
            //this.Reset();
            //this.InvalidateVisual();
            RaiseEvent(new RoutedEventArgs(FitEvent, this));
        }
        private void mnuZoomOut_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Just press Zoom out");
        }
        private void mnuZoomIn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Just press Zoom in");
        }
        private void mnuMeasure_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Just press measure");
        }

        private void mnuLocator_Click(object sender, RoutedEventArgs e)
        {
            EnableLocatorTool = true;
        }
        private void mnuTrain_Click(object sender, RoutedEventArgs e)
        {
            EnableLocatorTool = false;
            RaiseEvent(new RoutedEventArgs(TrainLocatorEvent, this));
        }

        private void mnuSelectRect_Click(object sender, RoutedEventArgs e)
        {
            _selectPolyItem.IsChecked = false;
            EnableSelectRoiTool = true;
        }
        private void mnuSelectPolygon_Click(object sender, RoutedEventArgs e)
        {
            _selectRectItem.IsChecked = false;
        }

        private void mnuGetROI_Click(object sender, RoutedEventArgs e)
        {
            EnableSelectRoiTool = false;
            RaiseEvent(new RoutedEventArgs(SelectedROIEvent, this));
        }
        private void mnuSaveFullImage_Click(object sender, RoutedEventArgs e)
        {
            EnableSelectRoiTool = false;
            RaiseEvent(new RoutedEventArgs(SaveFullImageEvent, this));
        }
        private void mnuSaveROIImage_Click(object sender, RoutedEventArgs e)
        {
            EnableSelectRoiTool = false;
            RaiseEvent(new RoutedEventArgs(SaveROIImageEvent, this));
        }

        #endregion

        #region Handle Event
        private void ImageEx_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_enableSelectRoiTool && !_enableLocatorTool)
            {
                _ctxMnuDefault.IsOpen = true;
                return;
            }

            var mat = new Matrix();
            if (_enableRotate == true)
                mat.RotateAt(_rectRotation, _centerPoint.X, _centerPoint.Y);
            mat.Translate(_offsetRect.X, _offsetRect.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            if (_rect.Contains(point) && _enableSelectRoiTool && !_enableLocatorTool)
            {
                _ctxMnuRoiMode.IsOpen = true;
            }
            else if (_rect.Contains(point) && !_enableSelectRoiTool && _enableLocatorTool)
            {
                _ctxMnuLocator.IsOpen = true;
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
            if (!_enableSelectRoiTool && !_enableLocatorTool)
                return;
            _drag = false;

            _offsetXYInside.X = (_rect.Width - _rectInside.Width) / 2;
            _offsetXYInside.Y = (_rect.Height - _rectInside.Height) / 2;
        }
        private void ImageEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_enableSelectRoiTool && !_enableLocatorTool)
            {
                var pointEnd = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);

                int nX = (int)(Math.Round(pointEnd.X, MidpointRounding.AwayFromZero));
                int nY = (int)(Math.Round(pointEnd.Y, MidpointRounding.AwayFromZero));

                if (m_bmp == null)
                    return;

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
                return;
            }
            SetHitType(e);
            if (!_drag)
                return;

            var mat = new Matrix();
            if (_enableRotate == true)
                mat.RotateAt(_rectRotation, _centerPoint.X, _centerPoint.Y);
            mat.Translate(_offsetRect.X, _offsetRect.Y);
            mat.Invert();

            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            Point offsetSize;
            Point clamped;

            switch (_dragAnchor)
            {
                case AnchorPoint.TopLeft:
                case AnchorPoint.TopLeftChild:
                    if (_dragAnchor == AnchorPoint.TopLeft)
                    {
                        clamped = new Point(Math.Min(_rect.BottomRight.X - 10d, point.X),
                            Math.Min(_rect.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left + offsetSize.X,
                            _dragRect.Top + offsetSize.Y,
                            _dragRect.Width - offsetSize.X,
                            _dragRect.Height - offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rectInside = new Rect(
                                  _dragRectInside.Left + offsetSize.X + _offsetRectInside.X,
                                  _dragRectInside.Top + offsetSize.Y + _offsetRectInside.Y,
                                 _rectInside.Width,
                                 _rectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.TopLeftChild)
                    {
                        clamped = new Point(Math.Min(_rectInside.BottomRight.X - 10d, point.X),
                            Math.Min(_rectInside.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left + offsetSize.X,
                            _dragRectInside.Top + offsetSize.Y,
                            _dragRectInside.Width - offsetSize.X,
                            _dragRectInside.Height - offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rect = new Rect(
                                  _dragRect.Left + offsetSize.X - _offsetRectInside.X,
                                  _dragRect.Top + offsetSize.Y - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;
                case AnchorPoint.TopRight:
                case AnchorPoint.TopRightChild:
                    if (_dragAnchor == AnchorPoint.TopRight)
                    {
                        clamped = new Point(Math.Max(_rect.BottomLeft.X - 10d, point.X),
                            Math.Min(_rect.BottomLeft.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left,
                            _dragRect.Top + offsetSize.Y,
                            _dragRect.Width + offsetSize.X,
                            _dragRect.Height - offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rectInside = new Rect(
                                  _dragRectInside.Left + _offsetRectInside.X,
                                  _dragRectInside.Top + offsetSize.Y + _offsetRectInside.Y,
                                 _rectInside.Width,
                                 _rectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.TopRightChild)
                    {
                        clamped = new Point(Math.Max(_rectInside.BottomLeft.X - 10d, point.X),
                           Math.Min(_rectInside.BottomLeft.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left,
                            _dragRectInside.Top + offsetSize.Y,
                            _dragRectInside.Width + offsetSize.X,
                            _dragRectInside.Height - offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rect = new Rect(
                                  _dragRect.Left - _offsetRectInside.X,
                                  _dragRect.Top + offsetSize.Y - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;

                case AnchorPoint.BottomLeft:
                case AnchorPoint.BottomLeftChild:
                    if (_dragAnchor == AnchorPoint.BottomLeft)
                    {
                        clamped = new Point(Math.Min(_rect.TopRight.X - 10d, point.X),
                            Math.Max(_rect.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left + offsetSize.X,
                            _dragRect.Top,
                            _dragRect.Width - offsetSize.X,
                            _dragRect.Height + offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rectInside = new Rect(
                                  _dragRectInside.Left + offsetSize.X + _offsetRectInside.X,
                                  _dragRectInside.Top + _offsetRectInside.Y,
                                 _rectInside.Width,
                                 _rectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.BottomLeftChild)
                    {
                        clamped = new Point(Math.Min(_rectInside.TopRight.X - 10d, point.X),
                           Math.Max(_rectInside.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left + offsetSize.X,
                            _dragRectInside.Top,
                            _dragRectInside.Width - offsetSize.X,
                            _dragRectInside.Height + offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rect = new Rect(
                                  _dragRect.Left + offsetSize.X - _offsetRectInside.X,
                                  _dragRect.Top - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;

                case AnchorPoint.BottomRight:
                case AnchorPoint.BottomRightChild:
                    if (_dragAnchor == AnchorPoint.BottomRight)
                    {
                        clamped = new Point(Math.Max(_rect.TopLeft.X + 10d, point.X),
                            Math.Max(_rect.TopLeft.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left,
                            _dragRect.Top,
                            _dragRect.Width + offsetSize.X,
                            _dragRect.Height + offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;

                        _rectInside = new Rect(
                                  _dragRectInside.Left + _offsetRectInside.X,
                                  _dragRectInside.Top + _offsetRectInside.Y,
                                 _dragRectInside.Width,
                                 _dragRectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.BottomRightChild)
                    {
                        clamped = new Point(Math.Max(_rectInside.TopLeft.X + 10d, point.X),
                            Math.Max(_rectInside.TopLeft.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left,
                            _dragRectInside.Top,
                            _dragRectInside.Width + offsetSize.X,
                            _dragRectInside.Height + offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;

                        _rect = new Rect(
                                  _dragRect.Left - _offsetRectInside.X,
                                  _dragRect.Top - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;

                case AnchorPoint.MidTop:
                case AnchorPoint.MidTopChild:
                    if (_dragAnchor == AnchorPoint.MidTop)
                    {
                        clamped = new Point(Math.Min(_rect.BottomRight.X - 10d, point.X),
                            Math.Min(_rect.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left,
                            _dragRect.Top + offsetSize.Y,
                            _dragRect.Width,
                            _dragRect.Height - offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rectInside = new Rect(
                                  _dragRectInside.Left + _offsetRectInside.X,
                                  _dragRectInside.Top + offsetSize.Y + _offsetRectInside.Y,
                                 _rectInside.Width,
                                 _rectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.MidTopChild)
                    {
                        clamped = new Point(Math.Min(_rectInside.BottomRight.X - 10d, point.X),
                            Math.Min(_rectInside.BottomRight.Y - 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left,
                            _dragRectInside.Top + offsetSize.Y,
                            _dragRectInside.Width,
                            _dragRectInside.Height - offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rect = new Rect(
                                  _dragRect.Left - _offsetRectInside.X,
                                  _dragRect.Top + offsetSize.Y - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;
                case AnchorPoint.MidBottom:
                case AnchorPoint.MidBottomChild:
                    if (_dragAnchor == AnchorPoint.MidBottom)
                    {
                        clamped = new Point(Math.Min(_rect.TopRight.X - 10d, point.X),
                            Math.Max(_rect.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left,
                            _dragRect.Top,
                            _dragRect.Width,
                            _dragRect.Height + offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rectInside = new Rect(
                                  _dragRectInside.Left + _offsetRectInside.X,
                                  _dragRectInside.Top + _offsetRectInside.Y,
                                 _rectInside.Width,
                                 _rectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.MidBottomChild)
                    {
                        clamped = new Point(Math.Min(_rectInside.TopRight.X - 10d, point.X),
                            Math.Max(_rectInside.TopRight.Y + 10d, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left,
                            _dragRectInside.Top,
                            _dragRectInside.Width,
                            _dragRectInside.Height + offsetSize.Y);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rect = new Rect(
                                  _dragRect.Left - _offsetRectInside.X,
                                  _dragRect.Top - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;
                case AnchorPoint.MidLeft:
                case AnchorPoint.MidLeftChild:
                    if (_dragAnchor == AnchorPoint.MidLeft)
                    {
                        clamped = new Point(Math.Min(_rect.TopRight.X - 10d, point.X),
                            Math.Max(_rect.TopRight.Y, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left + offsetSize.X,
                            _dragRect.Top,
                            _dragRect.Width - offsetSize.X,
                            _dragRect.Height);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rectInside = new Rect(
                                  _dragRectInside.Left + offsetSize.X + _offsetRectInside.X,
                                  _dragRectInside.Top + _offsetRectInside.Y,
                                 _rectInside.Width,
                                 _rectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.MidLeftChild)
                    {
                        clamped = new Point(Math.Min(_rectInside.TopRight.X - 10d, point.X),
                            Math.Max(_rectInside.TopRight.Y, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left + offsetSize.X,
                            _dragRectInside.Top,
                            _dragRectInside.Width - offsetSize.X,
                            _dragRectInside.Height);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rect = new Rect(
                                  _dragRect.Left + offsetSize.X - _offsetRectInside.X,
                                  _dragRect.Top - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;
                case AnchorPoint.MidRight:
                case AnchorPoint.MidRightChild:
                    if (_dragAnchor == AnchorPoint.MidRight)
                    {
                        clamped = new Point(Math.Max(_rect.TopLeft.X + 10d, point.X),
                            Math.Max(_rect.TopLeft.Y, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rect = new Rect(
                            _dragRect.Left,
                            _dragRect.Top,
                            _dragRect.Width + offsetSize.X,
                            _dragRect.Height);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rectInside = new Rect(
                                  _dragRectInside.Left + _offsetRectInside.X,
                                  _dragRectInside.Top + _offsetRectInside.Y,
                                 _rectInside.Width,
                                 _rectInside.Height);
                    }
                    else if (_dragAnchor == AnchorPoint.MidRightChild)
                    {
                        clamped = new Point(Math.Max(_rectInside.TopLeft.X + 10d, point.X),
                            Math.Max(_rectInside.TopLeft.Y, point.Y));
                        offsetSize = new Point(clamped.X - _dragStart.X, clamped.Y - _dragStart.Y);
                        _rectInside = new Rect(
                            _dragRectInside.Left,
                            _dragRectInside.Top,
                            _dragRectInside.Width + offsetSize.X,
                            _dragRectInside.Height);

                        _offsetRectInside.X = (_rect.Width - _rectInside.Width) / 2 - _offsetXYInside.X;
                        _offsetRectInside.Y = (_rect.Height - _rectInside.Height) / 2 - _offsetXYInside.Y;
                        _rect = new Rect(
                                  _dragRect.Left - _offsetRectInside.X,
                                  _dragRect.Top - _offsetRectInside.Y,
                                 _rect.Width,
                                 _rect.Height);
                    }
                    break;

                case AnchorPoint.Rotation:
                    //var vecX = (point.X);
                    //var vecY = (-point.Y);

                    var vecX = (point.X - _centerPoint.X);
                    var vecY = (_centerPoint.Y - point.Y);

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
                    _rectRotation += (float)((180 / Math.PI) * angle);
                    if (_rectRotation > 360 || _rectRotation < -360)
                        _rectRotation = 0;

                    break;

                case AnchorPoint.Center:
                case AnchorPoint.CenterChild:
                    //move this in screen-space
                    _offsetRect = new Point(e.GetPosition(this).X - _dragStartOffset.X,
                        e.GetPosition(this).Y - _dragStartOffset.Y);

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
            if (!_enableSelectRoiTool && !_enableLocatorTool)
                return;

            // Compute a Screen to Rectangle transform 

            var mat = new Matrix();
            if (_enableRotate == true)
                mat.RotateAt(_rectRotation, _centerPoint.X, _centerPoint.Y);
            mat.Translate(_offsetRect.X, _offsetRect.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            var rectOutside = _rect;
            var rectInside = _rectInside;

            if (_enableLocatorTool)
            {
                if (rectOutside.Contains(point))
                {
                    _enableSelectRect = true;
                    _enableSelectRectInside = false;
                    this.InvalidateVisual();
                }
                if (rectInside.Contains(point) && rectOutside.Contains(point))
                {
                    _enableSelectRectInside = true;
                    _enableSelectRect = false;
                    this.InvalidateVisual();
                }
                else if (!rectInside.Contains(point) && !rectOutside.Contains(point))
                {
                    _enableSelectRect = false;
                    _enableSelectRectInside = false;
                    this.InvalidateVisual();
                }
            }
            var rectTopLeft = new Rect(_rect.Left - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
            var rectTopRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
            var rectBottomLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
            var rectBottomRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
            var rectMidTop = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
            var rectMidBottom = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
            var rectMidLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
            var rectMidRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
            var ellipse = new EllipseGeometry(new Point(_rect.Left + _rect.Width / 2, _rect.Top - 20), _comWidth * 0.7, _comWidth * 0.7);

            var rectTopLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
            var rectTopRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
            var rectBottomLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
            var rectBottomRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
            var rectMidTopChild = new Rect(_rectInside.Left + _rectInside.Width / 2 - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
            var rectMidBottomChild = new Rect(_rectInside.Left + _rectInside.Width / 2 - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
            var rectMidLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top + _rectInside.Height / 2 - _comOffset, _comWidth, _comWidth);
            var rectMidRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top + _rectInside.Height / 2 - _comOffset, _comWidth, _comWidth);

            if (!_drag)
            {
                //We're in Rectangle space now, so its simple box-point intersection test
                if (rectTopLeft.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.TopLeft;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectTopLeftChild.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.TopLeftChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectTopRight.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.TopRight;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectTopRightChild.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.TopRightChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectBottomLeft.Contains(point))
                {

                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.BottomLeft;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectBottomLeftChild.Contains(point))
                {

                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.BottomLeftChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectBottomRight.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.BottomRight;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectBottomRightChild.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.BottomRightChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidTop.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidTop;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidTopChild.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidTopChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidBottom.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidBottom;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidBottomChild.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidBottomChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidLeft.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidLeft;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidLeftChild.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidLeftChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidRight.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidRight;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectMidRightChild.Contains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.MidRightChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (ellipse.FillContains(point))
                {
                    _drag = true;
                    _dragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.Rotation;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _isSelectingRoi = true;
                }
                else if (rectOutside.Contains(point))
                {
                    _drag = true;
                    //imageEx.DragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.Center;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _dragStartOffset = new Point(e.GetPosition(this).X - _offsetRect.X, e.GetPosition(this).Y - _offsetRect.Y);
                    _isSelectingRoi = true;
                }
                else if (rectInside.Contains(point))
                {
                    _drag = true;
                    //imageEx.DragStart = new Point(point.X, point.Y);
                    _dragAnchor = AnchorPoint.CenterChild;
                    _dragRect = new Rect(_rect.Left, _rect.Top, _rect.Width, _rect.Height);
                    _dragRectInside = new Rect(_rectInside.Left, _rectInside.Top, _rectInside.Width, _rectInside.Height);
                    _dragStartOffset = new Point(e.GetPosition(this).X - _offsetRect.X, e.GetPosition(this).Y - _offsetRect.Y);
                    _isSelectingRoi = true;
                }
                else
                {
                    //var tt = GetTranslateTransform(this);
                    //start = e.GetPosition(this);
                    //origin = new Point(tt.X, tt.Y);
                    //this.Cursor = Cursors.Hand;
                    //this.CaptureMouse();
                    _isSelectingRoi = false;
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
            get => _enableSelectRectInside;
            set => _enableSelectRectInside = value;
        }
        public bool EnableSelectRect
        {
            get => _enableSelectRect;
            set => _enableSelectRect = value;
        }
        public bool EnableLocatorTool
        {
            get => _enableLocatorTool;
            set
            {
                if (SetProperty(ref _enableLocatorTool, value))
                {
                    _enableSelectRoiTool = false;
                    _isSelectingRoi = false;
                    _enableSelectRect = false;
                    _enableRotate = false;
                    _drag = false;

                    ToolType = EnToolType.ToolType_LocatorTool;

                    this.InvalidateVisual();
                }
            }
        }
        public bool EnableSelectRoiTool
        {
            get => _enableSelectRoiTool;
            set
            {
                if (SetProperty(ref _enableSelectRoiTool, value))
                {
                    _enableLocatorTool = false;
                    _isSelectingRoi = true;
                    _enableSelectRect = true;
                    //_enableRotate = true;
                    _drag = true;

                    ToolType = EnToolType.ToolType_SelectRoiTool;

                    this.InvalidateVisual();
                }
            }
        }
        public bool EnableRotate
        {
            get => _enableRotate;
            set => _enableRotate = value;
        }
        public bool IsSelectingRoi
        {
            get => _isSelectingRoi;
            set => _isSelectingRoi = value;
        }
        public bool EnableSelectPoly
        {
            get => _enableSelectPoly;
            set => _enableSelectPoly = value;
        }
        public Point CenterPoint
        {
            get => _centerPoint;
            set => _centerPoint = value;
        }
        public Point CenterPointReal
        {
            get => _centerPointReal;
            set => _centerPointReal = value;
        }
        public bool Drag
        {
            get => _drag;
            set => _drag = value;
        }
        public bool CompletedSelectRoi
        {
            get => _completedSelectRoi;
            set => _completedSelectRoi = value;
        }
        public Size DragSize
        {
            get => _dragSize;
            set => _dragSize = value;
        }
        public Point DragStart
        {
            get => _dragStart;
            set => _dragStart = value;
        }
        public Point DragStartOffset
        {
            get => _dragStartOffset;
            set => _dragStartOffset = value;
        }
        public Rect DragRect
        {
            get => _dragRect;
            set => _dragRect = value;
        }
        public Rect DragRectInside
        {
            get => _dragRectInside;
            set => _dragRectInside = value;
        }
        public AnchorPoint DragAnchor
        {
            get => _dragAnchor;
            set => _dragAnchor = value;
        }
        public Rect Rect
        {
            get => _rect;
            set => _rect = value;
        }
        public Rect RectReal
        {
            get => _rectReal;
            set => _rectReal = value;
        }
        public Rect RectInside
        {
            get => _rectInside;
            set => _rectInside = value;
        }
        public Rect RectTransform
        {
            get => _rectTransform;
            set => _rectTransform = value;
        }
        public Point OffsetRect
        {
            get => _offsetRect;
            set => _offsetRect = value;
        }
        public Point OffsetRectInside
        {
            get => _offsetRectInside;
            set => _offsetRectInside = value;
        }
        public Point OffsetXYInside
        {
            get => _offsetXYInside;
            set => _offsetXYInside = value;
        }
        public Single RectRotation
        {
            get => _rectRotation;
            set => _rectRotation = value;
        }
        public EnToolType ToolType
        {
            get => _toolType;
            set
            {
                if (SetProperty(ref _toolType, value))
                {
                    // change mode tool
                }
            }
        }
        public EnGrabMode GrabMode
        {
            get => _grabMode;
            set
            {
                if (SetProperty(ref _grabMode, value))
                {
                    // change mode grab
                    if (_grabMode == EnGrabMode.GrabMode_ContinuousGrab)
                    {
                        EnableSelectRoiTool = false;
                        EnableLocatorTool = false;
                    }
                }
            }
        }
        public EnViewMode ViewMode
        {
            get => _viewMode;
            set
            {
                if (SetProperty(ref _viewMode, value))
                {
                    if (_viewMode == EnViewMode.ViewMode_ViewResult)
                    {
                        EnableSelectRoiTool = false;
                        EnableLocatorTool = false;
                    }
                    this.InvalidateVisual();
                }
            }
        }
        public System.Drawing.Bitmap BMP
        {
            get => m_bmp;
            set
            {
                if(SetProperty(ref m_bmp, value))
                {

                }
            }
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

            dc.DrawEllipse(Brushes.LightSalmon, new Pen(Brushes.LightSalmon, 1), cntPt, _comWidth * 0.1, _comWidth * 0.1);
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
        private void RenderSelectRectTool(DrawingContext dc)
        {
            //calculate center point
            _centerPoint = new Point(_rect.Left + _rect.Width / 2, _rect.Top + _rect.Height / 2);
            _centerPointReal.X = _centerPoint.X + _offsetRect.X;
            _centerPointReal.Y = _centerPoint.Y + _offsetRect.Y;
            _rectReal = new Rect(new Point(_rect.X + _offsetRect.X, _rect.Y + _offsetRect.Y), new Size(_rect.Width, _rect.Height));

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
            if (_enableRotate == true)
                mat1.RotateAt(_rectRotation, _centerPoint.X, _centerPoint.Y);
            mat1.Translate(_offsetRect.X, _offsetRect.Y);

            MatrixTransform matrixTransform1 = new MatrixTransform(mat1);
            dc.PushTransform(matrixTransform1);
            dc.PushOpacity(0.35);


            if (!_enableSelectRect)
            {
                dc.DrawRectangle(Brushes.GreenYellow, new Pen(Brushes.LightYellow, 2), _rect);
            }
            else
            {
                // All out gizmo rectangles are defined in Rectangle Parent Space
                Rect rectTopLeft = new Rect(_rect.Left - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
                Rect rectTopRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
                Rect rectBottomLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
                Rect rectBottomRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
                Rect rectMidTop = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
                Rect rectMidBottom = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
                Rect rectMidLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
                Rect rectMidRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
                Rect rectCenter = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);

                //Draw Rectangle Parent
                dc.DrawRectangle(colorBgRect, new Pen(colorPen, _thicknessPen), _rect);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectTopLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectTopRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectBottomLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectBottomRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidTop);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidBottom);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectCenter);


                //draw center
                dc.DrawLine(new Pen(colorCrossLine, _thicknessPen), new Point(_centerPoint.X - 10d,
                    _centerPoint.Y), new Point(_centerPoint.X + 10d, _centerPoint.Y));
                dc.DrawLine(new Pen(colorCrossLine, _thicknessPen), new Point(_centerPoint.X,
                    _centerPoint.Y - 10d), new Point(_centerPoint.X, _centerPoint.Y + 10d));
                //dc.DrawEllipse(colorCrossLine, null, _centerPoint, 2d, 2d);
            }
            if (_enableRotate == true)
            {
                //3 point draw line and ellipse
                Point pointLine1 = new Point(_rect.Left + _rect.Width / 2, _rect.Top - _comOffset);
                Point pointLine2 = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 40);
                Point pointCenterEllipse = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 40);

                //draw line rotate
                dc.DrawLine(new Pen(colorPen, _thicknessPen), pointLine1, pointLine2);
                //draw ellipse rotate
                dc.DrawEllipse(colorBgCorner, new Pen(colorPen, _thicknessPen), pointCenterEllipse, _comWidth * 0.6, _comWidth * 0.6);
            }

            dc.Pop();
        }
        private void RenderSelectRoiTool(DrawingContext dc)
        {
            RenderSelectRectTool(dc);
        }
        private void RenderLocatorTool(DrawingContext dc)
        {
            //calculate center point
            _centerPoint = new Point(_rect.Left + _rect.Width / 2, _rect.Top + _rect.Height / 2);
            _centerPointReal.X = _centerPoint.X + _offsetRect.X;
            _centerPointReal.Y = _centerPoint.Y + _offsetRect.Y;
            _rectReal = new Rect(new Point(_rect.X + _offsetRect.X, _rect.Y + _offsetRect.Y), new Size(_rect.Width, _rect.Height));


            Matrix mat1 = new Matrix();
            mat1.Translate(_offsetRect.X, _offsetRect.Y);

            MatrixTransform matrixTransform1 = new MatrixTransform(mat1);
            dc.PushTransform(matrixTransform1);
            dc.PushOpacity(0.35);

            if (!_enableSelectRect)
            {
                dc.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.LightYellow, 2), _rect);
            }
            else
            {
                // All out gizmo rectangles are defined in Rectangle Parent Space
                Rect rectTopLeft = new Rect(_rect.Left - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
                Rect rectTopRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
                Rect rectBottomLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
                Rect rectBottomRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
                Rect rectMidTop = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top - _comOffset, _comWidth, _comWidth);
                Rect rectMidBottom = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top + _rect.Height - _comOffset, _comWidth, _comWidth);
                Rect rectMidLeft = new Rect(_rect.Left - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
                Rect rectMidRight = new Rect(_rect.Left + _rect.Width - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);
                Rect rectCenter = new Rect(_rect.Left + _rect.Width / 2 - _comOffset, _rect.Top + _rect.Height / 2 - _comOffset, _comWidth, _comWidth);

                //Draw Rectangle Parent
                dc.DrawRectangle(colorBgRect, new Pen(colorPen, _thicknessPen), _rect);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectTopLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectTopRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectBottomLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectBottomRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidTop);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidBottom);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidLeft);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectMidRight);
                dc.DrawRectangle(colorBgCorner, new Pen(colorPen, _thicknessPen), rectCenter);


                //draw center
                dc.DrawLine(new Pen(colorCrossLine, _thicknessPen), new Point(_centerPoint.X - 10d,
                    _centerPoint.Y), new Point(_centerPoint.X + 10d, _centerPoint.Y));
                dc.DrawLine(new Pen(colorCrossLine, _thicknessPen), new Point(_centerPoint.X,
                    _centerPoint.Y - 10d), new Point(_centerPoint.X, _centerPoint.Y + 10d));
                //dc.DrawEllipse(Brushes.Red, null, _centerPoint, 2d, 2d);
            }

            if (!_enableSelectRectInside)
            {
                dc.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.OrangeRed, 2), _rectInside);
            }
            else
            {
                // All out gizmo rectangles are defined in Rectangle Child Space
                Rect rectTopLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
                Rect rectTopRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
                Rect rectBottomLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
                Rect rectBottomRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
                Rect rectMidTopChild = new Rect(_rectInside.Left + _rectInside.Width / 2 - _comOffset, _rectInside.Top - _comOffset, _comWidth, _comWidth);
                Rect rectMidBottomChild = new Rect(_rectInside.Left + _rectInside.Width / 2 - _comOffset, _rectInside.Top + _rectInside.Height - _comOffset, _comWidth, _comWidth);
                Rect rectMidLeftChild = new Rect(_rectInside.Left - _comOffset, _rectInside.Top + _rectInside.Height / 2 - _comOffset, _comWidth, _comWidth);
                Rect rectMidRightChild = new Rect(_rectInside.Left + _rectInside.Width - _comOffset, _rectInside.Top + _rectInside.Height / 2 - _comOffset, _comWidth, _comWidth);
                //var rectCenterChild = new Rect(_rectChild.Left + _rectChild.Width / 2 - 10f, _rectChild.Top + _rectChild.Height / 2 - 10f, 20f, 20f);

                //Draw Rectangle Child
                dc.DrawRectangle(Brushes.LightSlateGray, new Pen(Brushes.OrangeRed, _thicknessPen), _rectInside);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectTopLeftChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectTopRightChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectBottomLeftChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectBottomRightChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectMidTopChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectMidBottomChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectMidLeftChild);
                dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Blue, _thicknessPen), rectMidRightChild);
                //dc.DrawRectangle(Brushes.WhiteSmoke, new Pen(Brushes.Black, 1), rectCenterChild);
            }
            dc.Pop();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_viewMode == EnViewMode.ViewMode_CreateRecipe)
            {
                if (_enableSelectRoiTool && !_completedSelectRoi)
                {
                    RenderSelectRoiTool(dc);
                }
                else if (_enableLocatorTool && !_completedSelectRoi)
                {
                    RenderLocatorTool(dc);
                }
            }
            else if (_viewMode == EnViewMode.ViewMode_ViewResult)
            {
                
            }
        }
    }
    public enum EnToolType
    {
        ToolType_Default,
        ToolType_SelectRoiTool,
        ToolType_LocatorTool
    }
    public enum EnGrabMode
    {
        GrabMode_SingleGrab,
        GrabMode_ContinuousGrab,
    }
    public enum EnViewMode
    {
        ViewMode_CreateRecipe,
        ViewMode_ViewResult
    }
}
