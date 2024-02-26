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
    public class OuterGlowBorder : ContentControl
    {
        #region Fields
        public static readonly DependencyProperty OuterGlowOpacityProperty = DependencyProperty.Register("OuterGlowOpacity", typeof(double),
                                                                                                                                          typeof(OuterGlowBorder), null);

        public static readonly DependencyProperty OuterGlowSizeProperty = DependencyProperty.Register("OuterGlowSize", typeof(double),
                                                                                                                                      typeof(OuterGlowBorder), null);

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
                                                                                                                                     typeof(OuterGlowBorder), null);

        public static readonly DependencyProperty ShadowCornerRadiusProperty = DependencyProperty.Register("ShadowCornerRadius", typeof(CornerRadius),
                                                                                                                                             typeof(OuterGlowBorder), null);

        public static readonly DependencyProperty OuterGlowColorProperty = DependencyProperty.Register("OuterGlowColor", typeof(Color),
                                                                                                                                        typeof(OuterGlowBorder),
                                                                                                                                        new PropertyMetadata(
                                                                                                                                            Colors.Black,
                                                                                                                                            OuterGlowColorChanged));
        public static readonly DependencyProperty ClipContentProperty = DependencyProperty.Register("ClipContent", typeof(bool), typeof(OuterGlowBorder),
                                                                                                                                    null);


        private Border _outerGlowBorder;
        private GradientStop _shadowHorizontal1;
        private GradientStop _shadowHorizontal2;
        private GradientStop _shadowOuterStop1;
        private GradientStop _shadowOuterStop2;
        private GradientStop _shadowVertical1;
        private GradientStop _shadowVertical2;
        #endregion
        #region Constructor
        public OuterGlowBorder()
        {
            DefaultStyleKey = typeof(OuterGlowBorder);
            SizeChanged += OuterGlowContentControlSizeChanged;
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
        public double OuterGlowOpacity
        {
            get
            {
                return (double)GetValue(OuterGlowOpacityProperty);
            }
            set
            {
                SetValue(OuterGlowOpacityProperty, value);
            }
        }

        public double OuterGlowSize
        {
            get
            {
                return (double)GetValue(OuterGlowSizeProperty);
            }

            set
            {
                SetValue(OuterGlowSizeProperty, value);
                UpdateGlowSize(OuterGlowSize);
                UpdateStops(new Size(ActualWidth, ActualHeight));
            }
        }

        public Color OuterGlowColor
        {
            get
            {
                return (Color)GetValue(OuterGlowColorProperty);
            }

            set
            {
                SetValue(OuterGlowColorProperty, value);
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

                ShadowCornerRadius = new CornerRadius(Math.Abs(value.TopLeft * 1.5), Math.Abs(value.TopRight * 1.5), Math.Abs(value.BottomRight * 1.5),
                                                                         Math.Abs(value.BottomLeft * 1.5));
            }
        }

        public CornerRadius ShadowCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ShadowCornerRadiusProperty);
            }

            set
            {
                SetValue(ShadowCornerRadiusProperty, value);
            }
        }
        #endregion
        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _shadowOuterStop1 = (GradientStop)GetTemplateChild("PART_ShadowOuterStop1");
            _shadowOuterStop2 = (GradientStop)GetTemplateChild("PART_ShadowOuterStop2");
            _shadowVertical1 = (GradientStop)GetTemplateChild("PART_ShadowVertical1");
            _shadowVertical2 = (GradientStop)GetTemplateChild("PART_ShadowVertical2");
            _shadowHorizontal1 = (GradientStop)GetTemplateChild("PART_ShadowHorizontal1");
            _shadowHorizontal2 = (GradientStop)GetTemplateChild("PART_ShadowHorizontal2");
            _outerGlowBorder = (Border)GetTemplateChild("PART_OuterGlowBorder");
            UpdateGlowSize(OuterGlowSize);
            UpdateGlowColor(OuterGlowColor);
        }

        internal void UpdateGlowSize(double size)
        {
            if (_outerGlowBorder != null)
            {
                _outerGlowBorder.Margin = new Thickness(-Math.Abs(size));
            }
        }

        internal void UpdateGlowColor(Color color)
        {
            if (_shadowVertical1 != null)
            {
                _shadowVertical1.Color = color;
            }

            if (_shadowVertical2 != null)
            {
                _shadowVertical2.Color = color;
            }

            if (_shadowOuterStop1 != null)
            {
                _shadowOuterStop1.Color = Color.FromArgb(0, color.R, color.G, color.B);
            }

            if (_shadowOuterStop2 != null)
            {
                _shadowOuterStop2.Color = Color.FromArgb(0, color.R, color.G, color.B);
            }
        }

        private static void OuterGlowColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.NewValue != null)
            {
                var outerGlowBorder = (OuterGlowBorder)dependencyObject;
                outerGlowBorder.UpdateGlowColor((Color)eventArgs.NewValue);
            }
        }

        private void OuterGlowContentControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateStops(e.NewSize);
        }

        private void UpdateStops(Size size)
        {
            if (size.Width > 0 && size.Height > 0)
            {
                if (_shadowHorizontal1 != null)
                {
                    _shadowHorizontal1.Offset = Math.Abs(OuterGlowSize) / (size.Width + Math.Abs(OuterGlowSize) + Math.Abs(OuterGlowSize));
                }

                if (_shadowHorizontal2 != null)
                {
                    _shadowHorizontal2.Offset = 1 - (Math.Abs(OuterGlowSize) / (size.Width + Math.Abs(OuterGlowSize) + Math.Abs(OuterGlowSize)));
                }

                if (_shadowVertical1 != null)
                {
                    _shadowVertical1.Offset = Math.Abs(OuterGlowSize) / (size.Height + Math.Abs(OuterGlowSize) + Math.Abs(OuterGlowSize));
                }

                if (_shadowVertical2 != null)
                {
                    _shadowVertical2.Offset = 1 - (Math.Abs(OuterGlowSize) / (size.Height + Math.Abs(OuterGlowSize) + Math.Abs(OuterGlowSize)));
                }
            }
        }
        #endregion
    }
}
