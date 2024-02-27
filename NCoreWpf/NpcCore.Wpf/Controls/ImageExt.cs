﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NpcCore.Wpf.Controls
{
    public class ImageExt : Image
    {
        AnchorPoint _dragAnchor = AnchorPoint.None;
        HitType _mouseHitType = HitType.None;

        #region Solidbrush
        SolidColorBrush colorBgRect = (SolidColorBrush)new BrushConverter().ConvertFrom("#3d424d");
        SolidColorBrush colorBgCorner = (SolidColorBrush)new BrushConverter().ConvertFrom("#ce3b3f");
        SolidColorBrush colorPen = (SolidColorBrush)new BrushConverter().ConvertFrom("#F3E9DF");
        SolidColorBrush colorCrossLine = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFD764");
        double _thicknessPen = 0.5;
        #endregion

        #region Routed Event
        public static readonly RoutedEvent GetROIEvent = EventManager.RegisterRoutedEvent(
            "GetROI",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public static readonly RoutedEvent SaveImageEvent = EventManager.RegisterRoutedEvent(
            "SaveImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public static readonly RoutedEvent FitEvent = EventManager.RegisterRoutedEvent(
            "Fit",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public event RoutedEventHandler GetROI
        {
            add
            {
                base.AddHandler(GetROIEvent, value);
            }
            remove
            {
                base.RemoveHandler(GetROIEvent, value);
            }
        }
        public event RoutedEventHandler SaveImage
        {
            add
            {
                base.AddHandler(SaveImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(SaveImageEvent, value);
            }
        }
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
        private bool _completedGetRoi;
        private bool _enableGetRoiTool;
        private bool _isSelectingRoi;
        private bool _enableRotate;
        private bool _enableLocatorTool;
        private bool _enableSelectRect;
        private bool _enableSelectRectInside;
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

        private double _comWidth = 6;
        private double _comOffset = 3;
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

        #region Constructor
        public ImageExt()
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

            this.MouseDown += ImageEx_MouseDown;
            this.MouseMove += ImageEx_MouseMove;
            this.MouseUp += ImageEx_MouseUp;
            this.MouseWheel += ImageEx_MouseWheel;

            this.MouseLeftButtonUp += ImageEx_MouseLeftButtonUp;
            this.PreviewMouseRightButtonDown += ImageEx_PreviewMouseRightButtonDown;
        }
        #endregion

        #region Handle Event
        private void ImageEx_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_enableGetRoiTool && !_enableLocatorTool)
                return;

            var mat = new Matrix();
            if (_enableRotate == true)
                mat.RotateAt(_rectRotation, _centerPoint.X, _centerPoint.Y);
            mat.Translate(_offsetRect.X, _offsetRect.Y);
            mat.Invert();

            // Mouse point in Rectangle's space. 
            var point = mat.Transform(new Point(e.GetPosition(this).X, e.GetPosition(this).Y));

            if (_rect.Contains(point))
            {
                //Create contextmenu get ROI
                ContextMenu context = new ContextMenu();

                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Get ROI";
                menuItem.Name = "mnuGetROI";
                menuItem.Click += mnuGetROI_Click;
                menuItem.FontFamily = new FontFamily("Georgia");
                menuItem.FontWeight = FontWeights.Regular;
                menuItem.FontSize = 12;

                context.Items.Add(menuItem);
                context.PlacementTarget = this;
                context.IsOpen = true;

            }
            else
            {
                ContextMenu context = new ContextMenu();

                MenuItem saveImageItem = new MenuItem();
                saveImageItem.Header = "Save Image";
                saveImageItem.Name = "mnuSaveImage";
                saveImageItem.Click += mnuSaveImage_Click;
                saveImageItem.FontFamily = new FontFamily("Georgia");
                saveImageItem.FontWeight = FontWeights.Regular;
                saveImageItem.FontSize = 12;
                MenuItem fitItem = new MenuItem();
                fitItem.Header = "Fit";
                fitItem.Name = "mnuFit";
                fitItem.Click += mnuFit_Click;
                fitItem.FontFamily = new FontFamily("Georgia");
                fitItem.FontWeight = FontWeights.Regular;
                fitItem.FontSize = 12;

                context.Items.Add(saveImageItem);
                context.Items.Add(fitItem);
                context.PlacementTarget = this;
                context.IsOpen = true;
            }
        }

        private void mnuFit_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(FitEvent, this));
        }

        private void mnuSaveImage_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveImageEvent, this));
        }

        private void mnuGetROI_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(GetROIEvent, this));
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
            if (!_enableGetRoiTool && !_enableLocatorTool)
                return;
            _drag = false;

            _offsetXYInside.X = (_rect.Width - _rectInside.Width) / 2;
            _offsetXYInside.Y = (_rect.Height - _rectInside.Height) / 2;
        }

        private void ImageEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_enableGetRoiTool && !_enableLocatorTool)
                return;
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
            if (!_enableGetRoiTool && !_enableLocatorTool)
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
            set => _enableLocatorTool = value;
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
        public bool EnableGetRoiTool
        {
            get => _enableGetRoiTool;
            set => _enableGetRoiTool = value;
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
        public bool CompletedGetRoi
        {
            get => _completedGetRoi;
            set => _completedGetRoi = value;
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
        #endregion

        private void RenderGetRoiTool(DrawingContext dc)
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
            dc.PushOpacity(0.6);


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
                Point pointLine2 = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 20);
                Point pointCenterEllipse = new Point(_rect.Left + _rect.Width / 2, _rect.Top - 20);

                //draw line rotate
                dc.DrawLine(new Pen(colorPen, _thicknessPen), pointLine1, pointLine2);
                //draw ellipse rotate
                dc.DrawEllipse(colorBgCorner, new Pen(colorPen, _thicknessPen), pointCenterEllipse, _comWidth * 0.6, _comWidth * 0.6);
            }

            dc.Pop();
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
            dc.PushOpacity(0.55);

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
                //dc.DrawEllipse(Brushes.Red, null, _centerPoint, 2d, 2d);
            }

            if (!_enableSelectRectInside)
            {
                dc.DrawRectangle(Brushes.LightYellow, new Pen(Brushes.OrangeRed, 2), _rectInside);
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
                dc.DrawRectangle(Brushes.LightYellow, new Pen(Brushes.OrangeRed, 2), _rectInside);
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

            if (_enableGetRoiTool && !_completedGetRoi)
            {
                RenderGetRoiTool(dc);
            }
            else if (_enableLocatorTool && !_completedGetRoi)
            {
                RenderLocatorTool(dc);
            }
        }
    }
    public enum HitType
    {
        None,
        Body,
        Rotate,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        MidTop,
        MidBottom,
        MidLeft,
        MidRight,
        NoneChild,
        BodyChild,
        RotateChild,
        TopLeftChild,
        TopRightChild,
        BottomLeftChild,
        BottomRightChild,
        MidTopChild,
        MidBottomChild,
        MidLeftChild,
        MidRightChild
    }
    public enum AnchorPoint
    {
        None,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        MidTop,
        MidBottom,
        MidLeft,
        MidRight,
        Rotation,
        Center,
        NoneChild,
        TopLeftChild,
        TopRightChild,
        BottomLeftChild,
        BottomRightChild,
        MidTopChild,
        MidBottomChild,
        MidLeftChild,
        MidRightChild,
        RotationChild,
        CenterChild
    }
    public enum ModeTool
    {
        None,
        GetRoiTool,
        OneLocatorTool,
        FindTemplateTool
    }
}