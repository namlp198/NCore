using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class ItemCustomControl : ComboBoxItem
    {
        #region Fields
        public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(Uri),
            typeof(ItemCustomControl), new UIPropertyMetadata(null));
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

        public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register("IconHeight", typeof(double),
            typeof(ItemCustomControl), new PropertyMetadata(10.0));
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

        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double),
            typeof(ItemCustomControl), new PropertyMetadata(10.0));
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

        #region Constructor
        static ItemCustomControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemCustomControl), new FrameworkPropertyMetadata(typeof(ItemCustomControl)));
        }
        #endregion
    }
}
