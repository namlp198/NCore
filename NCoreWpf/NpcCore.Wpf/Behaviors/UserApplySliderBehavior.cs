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
    public class UserApplySliderBehavior : Behavior<Slider>
    {
        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplySliderBehavior));


        private Slider _element;




        protected override void OnAttached()
        {
            base.OnAttached();

            _element = this.AssociatedObject as Slider;

            if (_element != null)
            {
                _element.PreviewKeyDown += _element_PreviewKeyDown;
                _element.PreviewMouseLeftButtonUp += _element_PreviewMouseLeftButtonUp;
                _element.ValueChanged += _element_ValueChanged;

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
                _element.ValueChanged -= _element_ValueChanged;
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
                UserApplySliderRoutedEventArgs eventArgs = new UserApplySliderRoutedEventArgs(UserApplySliderBehavior.UserApplySliderEvent, _element, _element.Value, this.InputID);
                _element.RaiseEvent(eventArgs);
            }
        }

        public static readonly RoutedEvent UserApplySliderEvent = EventManager.RegisterRoutedEvent(
            "UserApplySlider", RoutingStrategy.Bubble, typeof(UserApplySliderRoutedEventArgs), typeof(UserApplySliderBehavior));

        #region Watch
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty =
            DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplySliderBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UserApplySliderBehavior view = dobj as UserApplySliderBehavior;
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
                _element.Value = Convert.ToDouble(this.WatchValue);
            }
            catch
            {
                _element.Value = _element.Minimum;
            }
        }
        #endregion
    }

    public class UserApplySliderRoutedEventArgs : RoutedEventArgs
    {
        public double Value { get; set; }
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }

        public UserApplySliderRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, double value, string inputID)
            : base(routedEvent)
        {
            this.InputID = inputID;
            this.Value = value;
            this.Element = element;
            this.DataContext = element?.DataContext;
        }
    }
}
