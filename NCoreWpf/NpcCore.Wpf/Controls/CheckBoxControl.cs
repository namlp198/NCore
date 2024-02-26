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
    public class CheckBoxControl : CheckBox
    {
        #region Fields
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty CheckedBackgroundProperty;
        public static readonly DependencyProperty CheckedIconColorProperty;
        #endregion
        #region Constructor
        static CheckBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxControl), new FrameworkPropertyMetadata(typeof(CheckBoxControl)));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CheckBoxControl), null);
            CheckedBackgroundProperty = DependencyProperty.Register("CheckedBackground", typeof(Brush), typeof(CheckBoxControl), null);
            CheckedIconColorProperty = DependencyProperty.Register("CheckedIconColor", typeof(Brush), typeof(CheckBoxControl), null);
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

        public Brush CheckedBackground
        {
            get
            {
                return (Brush)GetValue(CheckedBackgroundProperty);
            }
            set
            {
                SetValue(CheckedBackgroundProperty, value);
            }
        }

        public Brush CheckedIconColor
        {
            get
            {
                return (Brush)GetValue(CheckedIconColorProperty);
            }
            set
            {
                SetValue(CheckedIconColorProperty, value);
            }
        }

        #endregion
    }
}
