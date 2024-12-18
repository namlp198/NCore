﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NpcCore.Wpf.Controls.Behaviors
{
    public class UserApplyCheckBoxBehavior : Behavior<CheckBox>
    {
        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplyCheckBoxBehavior));


        private CheckBox _element;




        protected override void OnAttached()
        {
            base.OnAttached();

            _element = this.AssociatedObject as CheckBox;

            if (_element != null)
            {
                _element.Click += _element_Click;

                UpdateChecked();
            }
        }

        

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_element != null)
            {
                _element.Click -= _element_Click;
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
                UserApplyCheckedRoutedEventArgs eventArgs = new UserApplyCheckedRoutedEventArgs(UserApplyCheckBoxBehavior.UserApplyCheckedEvent, _element, _element.IsChecked, this.InputID);
                _element.RaiseEvent(eventArgs);
            }
        }

        public static readonly RoutedEvent UserApplyCheckedEvent = EventManager.RegisterRoutedEvent(
            "UserApplyChecked", RoutingStrategy.Bubble, typeof(UserApplyCheckedRoutedEventArgs), typeof(UserApplyCheckBoxBehavior));

        #region Watch
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty =
            DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplyCheckBoxBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UserApplyCheckBoxBehavior view = dobj as UserApplyCheckBoxBehavior;
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
            if (this.WatchValue == null)
            {
                _element.IsChecked = null;
            }
            else if (this.WatchValue is bool)
            {
                _element.IsChecked = (bool)this.WatchValue;
            }
            else if (this.WatchValue is bool?)
            {
                _element.IsChecked = (bool?)this.WatchValue;
            }
        }
        #endregion
    }

    public class UserApplyCheckedRoutedEventArgs : RoutedEventArgs
    {
        public bool? IsChecked { get; set; }
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }

        public UserApplyCheckedRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, bool? isChecked, string inputID)
            : base(routedEvent)
        {
            this.InputID = inputID;
            this.IsChecked = isChecked;
            this.Element = element;
            this.DataContext = element?.DataContext;
        }
    }
}
