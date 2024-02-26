using Npc.Foundation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Koh.Wpf.Controls.Behaviors
{
    public class UserApplySpinButtonBehavior : Behavior<FrameworkElement>
    {
        public double TargetValue
        {
            get { return (double)GetValue(UserValueTokenProperty); }
            set { SetValue(UserValueTokenProperty, value); }
        }
        public static readonly DependencyProperty UserValueTokenProperty =
            DependencyProperty.Register("TargetValue", typeof(double), typeof(UserApplySpinButtonBehavior), new PropertyMetadata(0d));

        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplySpinButtonBehavior));


        public double DefaultTick
        {
            get { return (double)GetValue(DefaultTickProperty); }
            set { SetValue(DefaultTickProperty, value); }
        }
        public static readonly DependencyProperty DefaultTickProperty =
            DependencyProperty.Register("DefaultTick", typeof(double), typeof(UserApplySpinButtonBehavior), new PropertyMetadata(0d));


        public double DetailTick
        {
            get { return (double)GetValue(DetailTickProperty); }
            set { SetValue(DetailTickProperty, value); }
        }
        public static readonly DependencyProperty DetailTickProperty =
            DependencyProperty.Register("DetailTick", typeof(double), typeof(UserApplySpinButtonBehavior), new PropertyMetadata(0d));



        private FrameworkElement _element;

        protected override void OnAttached()
        {
            base.OnAttached();

            _element = this.AssociatedObject as FrameworkElement;
            if (_element != null)
            {
                _element.AddHandler(Button.ClickEvent, new RoutedEventHandler(OnButtonClick));
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_element != null)
            {
                _element.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(OnButtonClick));
            }
            _element = null;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as FrameworkElement;
            if (button != null)
            {
                var tag = (button.Tag ?? "").ToString();

                if (tag == "SpinButton.Up")
                {
                    e.Handled = true;
                    double tick = Keyboard.Modifiers != ModifierKeys.Shift ? this.DefaultTick : this.DetailTick;
                    UserApplySpinButtonRoutedEventArgs args = new UserApplySpinButtonRoutedEventArgs(UserApplySpinButtonBehavior.UserSpinButtonEvent, _element, true, tick, InputID);

                    if(TargetValue <= double.MaxValue && double.IsNaN(TargetValue) is false)
                        TargetValue += tick;

                    _element.RaiseEvent(args);
                }
                else if(tag == "SpinButton.Down")
                {
                    e.Handled = true;
                    double tick = Keyboard.Modifiers != ModifierKeys.Shift ? this.DefaultTick : this.DetailTick;
                    UserApplySpinButtonRoutedEventArgs args = new UserApplySpinButtonRoutedEventArgs(UserApplySpinButtonBehavior.UserSpinButtonEvent, _element, false, -tick, InputID);

                    if (TargetValue >= double.MinValue && double.IsNaN(TargetValue) is false)
                        TargetValue -= tick;
                    
                    _element.RaiseEvent(args);
                }
            }
        }

        public static readonly RoutedEvent UserSpinButtonEvent = EventManager.RegisterRoutedEvent(
            "UserSpinButtonEvent", RoutingStrategy.Bubble, typeof(UserApplyTextRoutedEventArgs), typeof(UserApplySpinButtonBehavior));
    }

    public class UserApplySpinButtonRoutedEventArgs : RoutedEventArgs
    {
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }
        public double Value { get; set; }
        public bool IsUp { get; private set; }

        public UserApplySpinButtonRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, bool isUp, double value, string inputID)
            : base(routedEvent)
        {
            this.InputID = inputID;
            this.Element = element;
            this.DataContext = element?.DataContext;

            this.IsUp = isUp;
            this.Value = value;
        }
    }
}
