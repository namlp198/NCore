using NpcCore.Wpf.Ex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NpcCore.Wpf.Controls.Behaviors
{
    public class UserApplyDualRangeSliderBehavior : Behavior<DualRangeSlider>
    {
        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplyDualRangeSliderBehavior));

        public string Tag { get; set; }

        private DualRangeSlider _element;




        protected override void OnAttached()
        {
            base.OnAttached();

            _element = this.AssociatedObject as DualRangeSlider;

            if (_element != null)
            {
                _element.PreviewKeyDown += _element_PreviewKeyDown;
                _element.PreviewMouseLeftButtonUp += _element_PreviewMouseLeftButtonUp;
                //_element.ValueChanged += _element_ValueChanged;

                UpdateChecked();
            }
        }

        private void _element_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void _element_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void _element_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OnApply();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_element != null)
            {
                _element.PreviewKeyDown -= _element_PreviewKeyDown;
                _element.PreviewMouseLeftButtonUp -= _element_PreviewMouseLeftButtonUp;
                //_element.ValueChanged -= _element_ValueChanged;
            }

            _element = null;
        }




        private void _element_Click(object sender, RoutedEventArgs e)
        {
            if (_element != null)
            {
                OnApply();
            }
        }

        private void OnApply()
        {
            if (_element != null)
            {
                double Value = ("Value1".Equals(this.Tag)) ? (double)_element.Value1 : (double)_element.Value2;
                UserApplyDualRangeSliderRoutedEventArgs eventArgs = new UserApplyDualRangeSliderRoutedEventArgs(UserApplyDualRangeSliderBehavior.UserApplyDualRangeSliderEvent, _element, Value, this.InputID);
                _element.RaiseEvent(eventArgs);
            }
        }

        public static readonly RoutedEvent UserApplyDualRangeSliderEvent = EventManager.RegisterRoutedEvent(
            "UserApplyDualRangeSlider", RoutingStrategy.Bubble, typeof(UserApplyDualRangeSliderRoutedEventArgs), typeof(UserApplyDualRangeSliderBehavior));

        #region Watch
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty =
            DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplyDualRangeSliderBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            if (e.NewValue.Equals(e.OldValue))
                return;

            UserApplyDualRangeSliderBehavior view = dobj as UserApplyDualRangeSliderBehavior;
            if (view != null)
            {
                view.OnWatchValueChanged();
            }
        }

        private void OnWatchValueChanged()
        {
            if (_element == null)
            {
                return;
            }

            UpdateChecked();
        }

        private void UpdateChecked()
        { 
            try
            {
                double val = Convert.ToDouble(this.WatchValue);
                if (("Value1".Equals(this.Tag)))
                {
                    if(_element.Value1 != val)
                        _element.Value1 = val;
                }
                else
                {
                    if (_element.Value2 != val)
                        _element.Value2 = val;
                } 
            }
            catch
            { 
                if (("Value1".Equals(this.Tag)))
                {
                    if (_element.Value1 != _element.Minimum)
                        _element.Value1 = _element.Minimum;
                }
                else
                {
                    if (_element.Value2 != _element.Minimum)
                        _element.Value2 = _element.Minimum;
                } 
            }
        }
        #endregion
    }

    public class UserApplyDualRangeSliderRoutedEventArgs : RoutedEventArgs
    {
        public double Value { get; set; }
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }

        public UserApplyDualRangeSliderRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, double value, string inputID)
            : base(routedEvent)
        {
            this.InputID = inputID;
            this.Value = value;
            this.Element = element;
            this.DataContext = element?.DataContext;
        }
    }
}
