using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class OverFlowControl : ContextMenu
    {
        #region Fields
        public static readonly DependencyProperty CornerRadiusProperty;

        #endregion
        #region Constructor
        static OverFlowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverFlowControl), new FrameworkPropertyMetadata(typeof(OverFlowControl)));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(OverFlowControl), null);

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
        #endregion 

    }
}
