using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Koh.Wpf.Controls.Behaviors
{
    public class UserApplyStackPanelBehavior : Behavior<StackPanel>
    {
        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplyStackPanelBehavior));


        private StackPanel _element;




        protected override void OnAttached()
        {
            base.OnAttached();

            _element = this.AssociatedObject as StackPanel;

            if (_element != null)
            {
                _element.MouseLeftButtonUp += _element_Click;

                UpdateClick();
            }
        }

        

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_element != null)
            {
                _element.MouseLeftButtonUp -= _element_Click;
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
                UserApplyStackPanelClickRoutedEventArgs eventArgs = new UserApplyStackPanelClickRoutedEventArgs(UserApplyStackPanelBehavior.UserApplyClickEvent, _element, this.InputID);
                _element.RaiseEvent(eventArgs);
            }
        }

        public static readonly RoutedEvent UserApplyClickEvent = EventManager.RegisterRoutedEvent(
            "UserApplyClick", RoutingStrategy.Bubble, typeof(UserApplyStackPanelClickRoutedEventArgs), typeof(UserApplyStackPanelBehavior));

        #region Watch
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty =
            DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplyStackPanelBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UserApplyStackPanelBehavior view = dobj as UserApplyStackPanelBehavior;
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

            UpdateClick();
        }

        private void UpdateClick()
        {
            //if (this.WatchValue == null)
            //{
            //    _element.IsClick = null;
            //}
            //else if (this.WatchValue is bool)
            //{
            //    _element.IsClick = (bool)this.WatchValue;
            //}
            //else if (this.WatchValue is bool?)
            //{
            //    _element.IsClick = (bool?)this.WatchValue;
            //}
        }
        #endregion
    }

    public class UserApplyStackPanelClickRoutedEventArgs : RoutedEventArgs
    {
        //public bool? IsClick { get; set; }
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }

        public UserApplyStackPanelClickRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, string inputID)
            : base(routedEvent)
        {
            this.InputID = inputID;
            //this.IsClick = isClick;
            this.Element = element;
            this.DataContext = element?.DataContext;
        }
    }
}
