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
    public class UserApplyComboBoxBehavior : Behavior<ComboBox>
    {
        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplyComboBoxBehavior));


        private ComboBox _element;




        protected override void OnAttached()
        {
            base.OnAttached();

            _element = this.AssociatedObject as ComboBox;

            if (_element != null)
            {
                _element.SelectionChanged += _element_SelectionChanged;

                UpdateSelection();
            }
        }

        
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_element != null)
            {
                _element.SelectionChanged -= _element_SelectionChanged;
            }

            _element = null;
        }




        private void _element_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnApply();
        }


        private void OnApply()
        {
            if (_element != null && _ignoreApply == false)
            {
                UserApplySelectedItemRoutedEventArgs eventArgs = new UserApplySelectedItemRoutedEventArgs(UserApplyComboBoxBehavior.UserApplySelectedItemEvent, _element, _element.SelectedItem, this.InputID);
                _element.RaiseEvent(eventArgs);
            }
        }

        public static readonly RoutedEvent UserApplySelectedItemEvent = EventManager.RegisterRoutedEvent(
            "UserApplySelectedItem", RoutingStrategy.Bubble, typeof(UserApplySelectedItemRoutedEventArgs), typeof(UserApplyComboBoxBehavior));

        #region Watch
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty =
            DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplyComboBoxBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UserApplyComboBoxBehavior view = dobj as UserApplyComboBoxBehavior;
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

            UpdateSelection();
        }

        bool _ignoreApply;
        private void UpdateSelection()
        {
            _ignoreApply = true;
            _element.SelectedItem = this.WatchValue;
            if(_element.SelectedItem !=null)
            {
                string _stringDirection = _element.SelectedItem.ToString() as string;
                if ((_stringDirection != null) && (_stringDirection.Contains("Direction")))
                { _element.IsDropDownOpen = false; }
            }
            
            _ignoreApply = false;
        }
        #endregion
    }

    public class UserApplySelectedItemRoutedEventArgs : RoutedEventArgs
    {
        public object SelectedItem { get; set; }
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }

        public UserApplySelectedItemRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, object selectedItem, string inputID)
            : base(routedEvent)
        {
            this.InputID = inputID;
            this.SelectedItem = selectedItem;
            this.Element = element;
            this.DataContext = element?.DataContext;
        }
    }
}
