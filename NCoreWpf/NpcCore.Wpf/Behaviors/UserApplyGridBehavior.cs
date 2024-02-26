using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NpcCore.Wpf.Controls.Behaviors
{
    public class UserApplyGridBehavior : Behavior<Grid>
    {
        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplyGridBehavior));


        private Grid _element;




        protected override void OnAttached()
        {
            base.OnAttached();

            _element = this.AssociatedObject as Grid;

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
            // [ NCS-703 : Edit/Delete Ignore AutoHole Object List ] : Control Key 체크
            //if (_element != null)
            //{
            //    OnApply();
            //}

            this.IsCtrlKey = false;
            if (_element != null)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    this.IsCtrlKey = true;
                }

                OnApply();
                
            }
        }

        private void OnApply()
        {
            if (_element != null)
            {
                // [ NCS-703 : Edit/Delete Ignore AutoHole Object List ] : Argument 에 WatchValue/IsCtrlKey 추가
                UserApplyGridClickRoutedEventArgs eventArgs = new UserApplyGridClickRoutedEventArgs(UserApplyGridBehavior.UserApplyClickEvent, _element, this.InputID, this.WatchValue, this.IsCtrlKey);
                _element.RaiseEvent(eventArgs);
            }
        }

        public static readonly RoutedEvent UserApplyClickEvent = EventManager.RegisterRoutedEvent(
            "UserApplyClick", RoutingStrategy.Bubble, typeof(UserApplyGridClickRoutedEventArgs), typeof(UserApplyGridBehavior));

        /// <summary>
        /// Ctrl Key 여부
        /// [ NCS-703 : Edit/Delete Ignore AutoHole Object List ]
        /// </summary>
        public bool IsCtrlKey { get; set; }

        #region Watch
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty =
            DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplyGridBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UserApplyGridBehavior view = dobj as UserApplyGridBehavior;
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

    public class UserApplyGridClickRoutedEventArgs : RoutedEventArgs
    {
        //public bool? IsClick { get; set; }
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }

        /// <summary>
        /// WatchValue
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Ctrl Key
        /// </summary>
        public bool IsCtrlKey { get; set; }

        public UserApplyGridClickRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, string inputID, object value, bool isCtrlKey)
            : base(routedEvent)
        {
            this.Element = element;
            this.InputID = inputID;
            this.DataContext = element?.DataContext;
            this.Value = value;
            this.IsCtrlKey = isCtrlKey;
        }
    }
}
