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
    public class InnerGlowBorder : ContentControl
    {
        #region Fields
        public static readonly DependencyProperty InnerGlowOpacityProperty = DependencyProperty.Register("InnerGlowOpacity", typeof(double),
                                                                                                                                          typeof(InnerGlowBorder), null);

        public static readonly DependencyProperty InnerGlowSizeProperty = DependencyProperty.Register("InnerGlowSize", typeof(Thickness),
                                                                                                                                      typeof(InnerGlowBorder),
                                                                                                                                      new PropertyMetadata(InnerGlowSizeChanged));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
                                                                                                                                     typeof(InnerGlowBorder), null);

        public static readonly DependencyProperty InnerGlowColorProperty = DependencyProperty.Register("InnerGlowColor", typeof(Color),
                                                                                                                                        typeof(InnerGlowBorder),
                                                                                                                                        new PropertyMetadata(
                                                                                                                                            Colors.Black,
                                                                                                                                            InnerGlowColorChanged));

        public static readonly DependencyProperty ClipContentProperty = DependencyProperty.Register("ClipContent", typeof(bool), typeof(InnerGlowBorder), null);


        public static readonly DependencyProperty ContentZIndexProperty = DependencyProperty.Register("ContentZIndex", typeof(int), typeof(InnerGlowBorder), null);


        private Border _bottomGlow;
        private GradientStop _bottomGlowStop0;
        private GradientStop _bottomGlowStop1;
        private Border _leftGlow;
        private GradientStop _leftGlowStop0;
        private GradientStop _leftGlowStop1;
        private Border _rightGlow;
        private GradientStop _rightGlowStop0;
        private GradientStop _rightGlowStop1;
        private Border _topGlow;
        private GradientStop _topGlowStop0;
        private GradientStop _topGlowStop1;
        #endregion
        #region Constructor
        public InnerGlowBorder()
        {
            DefaultStyleKey = typeof(InnerGlowBorder);
        }
        #endregion
        #region Properties
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

        public int ContentZIndex
        {
            get
            {
                return (int)GetValue(ContentZIndexProperty);
            }
            set
            {
                SetValue(ContentZIndexProperty, value);
            }
        }

        public double InnerGlowOpacity
        {
            get
            {
                return (double)GetValue(InnerGlowOpacityProperty);
            }
            set
            {
                SetValue(InnerGlowOpacityProperty, value);
            }
        }
        public Color InnerGlowColor
        {
            get
            {
                return (Color)GetValue(InnerGlowColorProperty);
            }

            set
            {
                SetValue(InnerGlowColorProperty, value);
            }
        }

        public Thickness InnerGlowSize
        {
            get
            {
                return (Thickness)GetValue(InnerGlowSizeProperty);
            }

            set
            {
                SetValue(InnerGlowSizeProperty, value);
                UpdateGlowSize(value);
            }
        }
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
        #endregion
        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _leftGlow = GetTemplateChild("PART_LeftGlow") as Border;
            _topGlow = GetTemplateChild("PART_TopGlow") as Border;
            _rightGlow = GetTemplateChild("PART_RightGlow") as Border;
            _bottomGlow = GetTemplateChild("PART_BottomGlow") as Border;

            _leftGlowStop0 = GetTemplateChild("PART_LeftGlowStop0") as GradientStop;
            _leftGlowStop1 = GetTemplateChild("PART_LeftGlowStop1") as GradientStop;
            _topGlowStop0 = GetTemplateChild("PART_TopGlowStop0") as GradientStop;
            _topGlowStop1 = GetTemplateChild("PART_TopGlowStop1") as GradientStop;
            _rightGlowStop0 = GetTemplateChild("PART_RightGlowStop0") as GradientStop;
            _rightGlowStop1 = GetTemplateChild("PART_RightGlowStop1") as GradientStop;
            _bottomGlowStop0 = GetTemplateChild("PART_BottomGlowStop0") as GradientStop;
            _bottomGlowStop1 = GetTemplateChild("PART_BottomGlowStop1") as GradientStop;

            UpdateGlowColor(InnerGlowColor);
            UpdateGlowSize(InnerGlowSize);
        }

        internal void UpdateGlowColor(Color color)
        {
            if (_leftGlowStop0 != null)
            {
                _leftGlowStop0.Color = color;
            }

            if (_leftGlowStop1 != null)
            {
                _leftGlowStop1.Color = Color.FromArgb(0, color.R, color.G, color.B);
            }

            if (_topGlowStop0 != null)
            {
                _topGlowStop0.Color = color;
            }

            if (_topGlowStop1 != null)
            {
                _topGlowStop1.Color = Color.FromArgb(0, color.R, color.G, color.B);
            }

            if (_rightGlowStop0 != null)
            {
                _rightGlowStop0.Color = color;
            }

            if (_rightGlowStop1 != null)
            {
                _rightGlowStop1.Color = Color.FromArgb(0, color.R, color.G, color.B);
            }

            if (_bottomGlowStop0 != null)
            {
                _bottomGlowStop0.Color = color;
            }

            if (_bottomGlowStop1 != null)
            {
                _bottomGlowStop1.Color = Color.FromArgb(0, color.R, color.G, color.B);
            }
        }

        internal void UpdateGlowSize(Thickness newGlowSize)
        {
            if (_leftGlow != null)
            {
                _leftGlow.Width = Math.Abs(newGlowSize.Left);
            }

            if (_topGlow != null)
            {
                _topGlow.Height = Math.Abs(newGlowSize.Top);
            }

            if (_rightGlow != null)
            {
                _rightGlow.Width = Math.Abs(newGlowSize.Right);
            }

            if (_bottomGlow != null)
            {
                _bottomGlow.Height = Math.Abs(newGlowSize.Bottom);
            }
        }

        private static void InnerGlowColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.NewValue != null)
            {
                var innerGlowBorder = (InnerGlowBorder)dependencyObject;
                innerGlowBorder.UpdateGlowColor((Color)eventArgs.NewValue);
            }
        }
        private static void InnerGlowSizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            // [NCS-2695] CID 171141 Dereference null return (stat)
            //var innerGlowBorder = (InnerGlowBorder)dependencyObject;
            //innerGlowBorder.UpdateGlowSize((Thickness)eventArgs.NewValue);
            if (dependencyObject is InnerGlowBorder innerGlowBorder && eventArgs.NewValue is Thickness thickness)
            {
                innerGlowBorder.UpdateGlowSize(thickness);
            }
        }
        #endregion
    }
}
