using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class ButtonControl : ToggleButton
    {
        #region Fields
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty IconSourceProperty;
        public static readonly DependencyProperty IconHeightProperty;
        public static readonly DependencyProperty IconWidthProperty;
        public static readonly DependencyProperty TextColorChangedProperty;
        #endregion
        #region Constructor
        static ButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonControl), new FrameworkPropertyMetadata(typeof(ButtonControl)));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonControl), null);
            IconSourceProperty = DependencyProperty.Register("IconSource", typeof(Uri), typeof(ButtonControl), new UIPropertyMetadata(null));
            IconHeightProperty = DependencyProperty.Register("IconHeight", typeof(double), typeof(ButtonControl), new UIPropertyMetadata(10d));
            IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double), typeof(ButtonControl), new UIPropertyMetadata(10d));
            TextColorChangedProperty = DependencyProperty.Register("TextColorChanged", typeof(Brush),
                typeof(ButtonControl), new UIPropertyMetadata(null));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion

        #region Control Properties
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

        public Uri IconSource
        {
            get
            {
                return (Uri)GetValue(IconSourceProperty);
            }
            set
            {
                SetValue(IconSourceProperty, value);
            }
        }

        public Brush TextColorChanged
        {
            get
            {
                return (Brush)GetValue(TextColorChangedProperty);
            }
            set
            {
                SetValue(TextColorChangedProperty, value);
            }
        }

        public double IconHeight
        {
            get
            {
                return (double)GetValue(IconHeightProperty);
            }
            set
            {
                SetValue(IconHeightProperty, value);
            }
        }

        public double IconWidth
        {
            get
            {
                return (double)GetValue(IconWidthProperty);
            }
            set
            {
                SetValue(IconWidthProperty, value);
            }
        }

        #endregion
    }
}
