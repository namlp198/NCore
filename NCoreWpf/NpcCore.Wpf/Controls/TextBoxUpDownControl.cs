using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public partial class TextBoxUpDownControl : TextBox
    {
        #region Fields
        private Button _upButton;
        private Button _downButton;
        #endregion
        #region Dependency Property
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(TextBoxUpDownControl), new UIPropertyMetadata(double.MaxValue));
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(TextBoxUpDownControl), new UIPropertyMetadata(double.MinValue));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(TextBoxUpDownControl), new UIPropertyMetadata(0.0));
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(double), typeof(TextBoxUpDownControl), new UIPropertyMetadata(1.0));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TextBoxUpDownControl), new UIPropertyMetadata(null));
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

        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }
        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        public double Step
        {
            get
            {
                return (double)GetValue(StepProperty);
            }
            set
            {
                SetValue(StepProperty, value);
            }
        }
        #endregion
        #region Constructor
        static TextBoxUpDownControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxUpDownControl), new FrameworkPropertyMetadata(typeof(TextBoxUpDownControl)));
        }
        public override void OnApplyTemplate()
        {
            _upButton = GetTemplateChild("PART_UpButton") as Button;
            _downButton = GetTemplateChild("PART_DownButton") as Button;
            if (_upButton != null && _downButton != null)
            {
                _upButton.Click += buttonUpClick;
                _downButton.Click += buttonDownClick;
            }
            base.OnApplyTemplate();
        }
        #endregion
        #region Methods
        private void buttonUpClick(object sender, RoutedEventArgs e)
        {
            if (Value < Maximum)
            {
                Value += Step;
                if (Value > Maximum)
                    Value = Maximum;
            }
        }

        private void buttonDownClick(object sender, RoutedEventArgs e)
        {
            if (Value > Minimum)
            {
                Value -= Step;
                if (Value < Minimum)
                    Value = Minimum;
            }
        }
        #endregion
        #region Events
        public event EventHandler<DependencyPropertyChangedEventArgs> ValueChanged;
        private void RaiseValueChangedEvent(DependencyPropertyChangedEventArgs sender)
        {
            ValueChanged?.Invoke(this, sender);
        }
        #endregion
    }
}
