using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class ClippingBorder : ContentControl
    {
        #region Fields
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
                                                                                                                                     typeof(ClippingBorder),
                                                                                                                                     new PropertyMetadata(CornerRadiusChanged));
        public static readonly DependencyProperty ClipContentProperty = DependencyProperty.Register("ClipContent", typeof(bool), typeof(ClippingBorder),
                                                                                                                                    new PropertyMetadata(ClipContentChanged));
        private Border _border;
        private RectangleGeometry _bottomLeftClip;
        private ContentControl _bottomLeftContentControl;
        private RectangleGeometry _bottomRightClip;
        private ContentControl _bottomRightContentControl;
        private RectangleGeometry _topLeftClip;
        private ContentControl _topLeftContentControl;
        private RectangleGeometry _topRightClip;
        private ContentControl _topRightContentControl;
        #endregion
        #region Constructor
        public ClippingBorder()
        {
            DefaultStyleKey = typeof(ClippingBorder);
            SizeChanged += ClippingBorderSizeChanged;
        }
        #endregion
        #region Properties
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }
        public bool ClipContent
        {
            get
            {
                return (bool)GetValue(ClipContentProperty);
            }
            set
            {
                SetValue(ClipContentProperty, value);
            }
        }
        #endregion
        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _border = GetTemplateChild("PART_Border") as Border;
            _topLeftContentControl = GetTemplateChild("PART_TopLeftContentControl") as ContentControl;
            _topRightContentControl = GetTemplateChild("PART_TopRightContentControl") as ContentControl;
            _bottomRightContentControl = GetTemplateChild("PART_BottomRightContentControl") as ContentControl;
            _bottomLeftContentControl = GetTemplateChild("PART_BottomLeftContentControl") as ContentControl;

            if (_topLeftContentControl != null)
            {
                _topLeftContentControl.SizeChanged += ContentControlSizeChanged;
            }

            _topLeftClip = GetTemplateChild("PART_TopLeftClip") as RectangleGeometry;
            _topRightClip = GetTemplateChild("PART_TopRightClip") as RectangleGeometry;
            _bottomRightClip = GetTemplateChild("PART_BottomRightClip") as RectangleGeometry;
            _bottomLeftClip = GetTemplateChild("PART_BottomLeftClip") as RectangleGeometry;

            UpdateClipContent(ClipContent);

            UpdateCornerRadius(CornerRadius);
        }
        internal void UpdateCornerRadius(CornerRadius newCornerRadius)
        {
            if (_border != null && newCornerRadius != null)
            {
                if (!Double.IsNaN(newCornerRadius.TopLeft) &&
                    !Double.IsNaN(newCornerRadius.TopRight) &&
                    !Double.IsNaN(newCornerRadius.BottomLeft) &&
                    !Double.IsNaN(newCornerRadius.BottomRight))
                    _border.CornerRadius = newCornerRadius;
            }

            if (_topLeftClip != null)
            {
                _topLeftClip.RadiusX = _topLeftClip.RadiusY = newCornerRadius.TopLeft - (Math.Min(BorderThickness.Left, BorderThickness.Top) / 2);
            }

            if (_topRightClip != null)
            {
                _topRightClip.RadiusX = _topRightClip.RadiusY = newCornerRadius.TopRight - (Math.Min(BorderThickness.Top, BorderThickness.Right) / 2);
            }

            if (_bottomRightClip != null)
            {
                _bottomRightClip.RadiusX = _bottomRightClip.RadiusY = newCornerRadius.BottomRight - (Math.Min(BorderThickness.Right, BorderThickness.Bottom) / 2);
            }

            if (_bottomLeftClip != null)
            {
                _bottomLeftClip.RadiusX = _bottomLeftClip.RadiusY = newCornerRadius.BottomLeft - (Math.Min(BorderThickness.Bottom, BorderThickness.Left) / 2);
            }

            UpdateClipSize(new Size(ActualWidth, ActualHeight));
        }
        internal void UpdateClipContent(bool clipContent)
        {
            if (clipContent)
            {
                if (_topLeftContentControl != null)
                {
                    _topLeftContentControl.Clip = _topLeftClip;
                }

                if (_topRightContentControl != null)
                {
                    _topRightContentControl.Clip = _topRightClip;
                }

                if (_bottomRightContentControl != null)
                {
                    _bottomRightContentControl.Clip = _bottomRightClip;
                }

                if (_bottomLeftContentControl != null)
                {
                    _bottomLeftContentControl.Clip = _bottomLeftClip;
                }

                UpdateClipSize(new Size(ActualWidth, ActualHeight));
            }
            else
            {
                if (_topLeftContentControl != null)
                {
                    _topLeftContentControl.Clip = null;
                }

                if (_topRightContentControl != null)
                {
                    _topRightContentControl.Clip = null;
                }

                if (_bottomRightContentControl != null)
                {
                    _bottomRightContentControl.Clip = null;
                }

                if (_bottomLeftContentControl != null)
                {
                    _bottomLeftContentControl.Clip = null;
                }
            }
        }

        private static void CornerRadiusChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            // [NCS-2695] CID 171166 Dereference null return (stat)
            //var clippingBorder = (ClippingBorder)dependencyObject;
            //clippingBorder.UpdateCornerRadius((CornerRadius)eventArgs.NewValue);
            if (dependencyObject is ClippingBorder clippingBorder && eventArgs.NewValue is CornerRadius cornerRadius)
            {
                clippingBorder.UpdateCornerRadius(cornerRadius);
            }
        }

        private static void ClipContentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            // // [NCS-2695] CID 171160 Dereference null return (stat)
            //var clippingBorder = (ClippingBorder)dependencyObject;
            //clippingBorder.UpdateClipContent((bool)eventArgs.NewValue);
            if (dependencyObject is ClippingBorder clippingBorder && eventArgs.NewValue is bool newValue)
            {
                clippingBorder.UpdateClipContent(newValue);
            }
        }
        private void ClippingBorderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ClipContent)
            {
                UpdateClipSize(e.NewSize);
            }
        }

        private void ContentControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ClipContent)
            {
                UpdateClipSize(new Size(ActualWidth, ActualHeight));
            }
        }
        private void UpdateClipSize(Size size)
        {
            if (size.Width > 0 || size.Height > 0)
            {
                double contentWidth = Math.Max(0, size.Width - BorderThickness.Left - BorderThickness.Right);
                double contentHeight = Math.Max(0, size.Height - BorderThickness.Top - BorderThickness.Bottom);

                if (_topLeftClip != null)
                {
                    _topLeftClip.Rect = new Rect(0, 0, contentWidth + (CornerRadius.TopLeft * 2), contentHeight + (CornerRadius.TopLeft * 2));
                }

                if (_topRightClip != null)
                {
                    _topRightClip.Rect = new Rect(0 - CornerRadius.TopRight, 0, contentWidth + CornerRadius.TopRight, contentHeight + CornerRadius.TopRight);
                }

                if (_bottomRightClip != null)
                {
                    _bottomRightClip.Rect = new Rect(0 - CornerRadius.BottomRight, 0 - CornerRadius.BottomRight, contentWidth + CornerRadius.BottomRight,
                                                                contentHeight + CornerRadius.BottomRight);
                }

                if (_bottomLeftClip != null)
                {
                    _bottomLeftClip.Rect = new Rect(0, 0 - CornerRadius.BottomLeft, contentWidth + CornerRadius.BottomLeft, contentHeight + CornerRadius.BottomLeft);
                }
            }
        }
        #endregion
    }
}
