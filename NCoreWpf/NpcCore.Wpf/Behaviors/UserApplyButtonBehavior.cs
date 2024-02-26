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
    /// <summary>
    /// Button Behavior
    /// [ NCS-703 : Edit/Delete Ignore AutoHole Object List ]
    /// </summary>
    public class UserApplyButtonBehavior : Behavior<Button>
    {
        /// <summary>
        /// Button Element
        /// </summary>
        private Button _element;

        /// <summary>
        /// InputID
        /// </summary>
        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty = DependencyProperty.Register("InputID", typeof(string), typeof(UserApplyButtonBehavior));

        /// <summary>
        /// Attached
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this._element = this.AssociatedObject as Button;

            if (this._element != null)
            {
                this._element.Click += this._element_Click;
            }
        }

        /// <summary>
        /// Detaching
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this._element != null)
            {
                this._element.Click -= this._element_Click;
            }

            this._element = null;
        }

        /// <summary>
        /// Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _element_Click(object sender, RoutedEventArgs e)
        {
            if (this._element != null)
            {
                this.OnApply();
            }
        }

        /// <summary>
        /// Apply
        /// </summary>
        private void OnApply()
        {
            if (this._element != null)
            {
                var eventArgs = new UserApplyButtonClickRoutedEventArgs(UserApplyButtonBehavior.UserApplyButtonClickEvent, this._element, this.InputID, this.WatchValue);
                this._element.RaiseEvent(eventArgs);
            }
        }

        /// <summary>
        /// RoutedEvent
        /// </summary>
        public static readonly RoutedEvent UserApplyButtonClickEvent = EventManager.RegisterRoutedEvent("UserApplyButtonClick", RoutingStrategy.Bubble, typeof(UserApplyButtonClickRoutedEventArgs), typeof(UserApplyButtonBehavior));

        #region Watch
        /// <summary>
        /// Watch Value
        /// </summary>
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty = DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplyButtonBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UserApplyButtonBehavior view = dobj as UserApplyButtonBehavior;
            if (view != null)
            {
                view.OnWatchValueChanged();
            }
        }

        private void OnWatchValueChanged()
        {
            if (this._element == null)
            {
                return;
            }
        }
        #endregion
    }

    public class UserApplyButtonClickRoutedEventArgs : RoutedEventArgs
    {
        public string InputID { get; set; }

        public FrameworkElement Element { get; set; }

        public object DataContext { get; set; }

        public object Value { get; set; }

        public UserApplyButtonClickRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, string inputID, object value)
            : base(routedEvent)
        {
            this.Element = element;
            this.InputID = inputID;
            this.DataContext = element?.DataContext;
            this.Value = value;
        }
    }
}
