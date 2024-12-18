﻿using Npc.Foundation.Helper;
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
    public class UserApplyTextBoxBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// PreviewKeyUp Event 를 발생할지 여부
        /// [NCS-3035] : bugfix : DB Test 버튼이 없어 연결 상태를 확인할 수 있는 길이 없음
        /// </summary>
        public bool IsApplyWhenPreviewKeyUp
        {
            get { return (bool)GetValue(IsApplyWhenPreviewKeyUpProperty); }
            set { SetValue(IsApplyWhenPreviewKeyUpProperty, value); }
        }
        public static readonly DependencyProperty IsApplyWhenPreviewKeyUpProperty =
            DependencyProperty.Register("IsApplyWhenPreviewKeyUp", typeof(bool), typeof(UserApplyTextBoxBehavior), new PropertyMetadata(false));

        #region NULL 일 경우 Event를 밖으로 보낼것인지 여부
        public bool AllowNullProcess
        {
            get { return (bool)GetValue(AllowNullProcessProperty); }
            set { SetValue(AllowNullProcessProperty, value); }
        }
        public static readonly DependencyProperty AllowNullProcessProperty =
            DependencyProperty.Register("AllowNullProcess", typeof(bool), typeof(UserApplyTextBoxBehavior));
        #endregion

        public IValueConverter WatchValueConverter
        {
            get { return (IValueConverter)GetValue(WatchValueConverterProperty); }
            set { SetValue(WatchValueConverterProperty, value); }
        }
        public static readonly DependencyProperty WatchValueConverterProperty =
            DependencyProperty.Register("WatchValueConverter", typeof(IValueConverter), typeof(UserApplyTextBoxBehavior));

        public string InputID
        {
            get { return (string)GetValue(UserTokenProperty); }
            set { SetValue(UserTokenProperty, value); }
        }
        public static readonly DependencyProperty UserTokenProperty =
            DependencyProperty.Register("InputID", typeof(string), typeof(UserApplyTextBoxBehavior));

        public string Validator { get; set; }
        public string ValidatorOption { get; set; }

        public object WatchValueConverterParameter { get; set; }

        public string _watchValueText;
        private TextBox _element;
        private Window _outside;

        #region LifeCycle
        protected override void OnAttached()
        {
            base.OnAttached();

            _element = AssociatedObject as TextBox;
            if (_element != null)
            {
                _element.PreviewGotKeyboardFocus += _element_PreviewGotKeyboardFocus;
                _element.PreviewLostKeyboardFocus += _element_PreviewLostKeyboardFocus;
                _element.PreviewKeyDown += _element_PreviewKeyDown;

                // [NCS-3035] : bugfix : DB Test 버튼이 없어 연결 상태를 확인할 수 있는 길이 없음
                if (IsApplyWhenPreviewKeyUp == true)
                {
                    _element.PreviewKeyUp += _element_PreviewKeyUp;
                }

                this.Dispatcher.BeginInvoke(new Action(OnLoadedElement), DispatcherPriority.Background);
            }
        }

        private void _element_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_element != null)
            {
                if (e.Key == Key.Enter && _element.AcceptsReturn == false)
                {
                    OnApply();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// PreviewKeyUp Event 처리
        /// [NCS-3035] : bugfix : DB Test 버튼이 없어 연결 상태를 확인할 수 있는 길이 없음
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _element_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (_element != null && IsApplyWhenPreviewKeyUp == true)
            {
                OnApply();
                e.Handled = true;
            }
        }

        public void OnLoadedElement()
        {
            _outside = VisualTreeHelperEx.FindAncestorByType<Window>(_element);
            if (_outside != null)
            {
                _outside.PreviewMouseDown += _outside_PreviewMouseDown;
            }

            if (_element != null)
            {
                OnUpdateText();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_element != null)
            {
                _element.PreviewGotKeyboardFocus -= _element_PreviewGotKeyboardFocus;
                _element.PreviewLostKeyboardFocus -= _element_PreviewLostKeyboardFocus;
                _element.PreviewKeyDown -= _element_PreviewKeyDown;
            }
            _element = null;

            if (_outside != null)
            {
                _outside.PreviewMouseDown -= _outside_PreviewMouseDown;
            }
            _outside = null;
        }
        #endregion

        #region Outside
        private void _outside_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_element == null)
            {
                return;
            }

            var fe = e.OriginalSource as TextBox;
            var textbox = VisualTreeHelperEx.FindAncestorByType<TextBox>(e.OriginalSource as FrameworkElement);
            if (textbox != _element && _element.IsFocused == true)
            {
                _preProcessedLostFocus = true;
                OnApply();
            }
        }
        #endregion

        #region Focus
        private bool _preProcessedLostFocus = false;
        private void _element_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _preProcessedLostFocus = false;
        }

        private void _element_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_preProcessedLostFocus == false)
            {
                OnApply();
            }
        }
        #endregion

        #region Apply
        private void OnApply()
        {
            if (_element != null)
            {
                object value = null;
                var validator = ValidatorFactory.Create(this.Validator);
                if (validator != null)
                {
                    if (validator.Vaild(_element.Text, out value, this.ValidatorOption) == false)
                    {
                        OnUpdateText();
                        _element.SelectAll();
                        return;
                    }
                }
                else
                {
                    value = _element.Text;
                }

                if (IsChanged() == true)
                {
                    UserApplyTextRoutedEventArgs eventArgs = new UserApplyTextRoutedEventArgs(UserApplyTextBoxBehavior.UserApplyTextEvent, _element, _element.Text, this.InputID) { ValidationValue = value };
                    _element.RaiseEvent(eventArgs);
                }                

                // Allow Null Process
                if (this.AllowNullProcess && string.IsNullOrEmpty(value as string))
                {
                    UserApplyTextRoutedEventArgs eventArgs = new UserApplyTextRoutedEventArgs(UserApplyTextBoxBehavior.UserApplyTextEvent, _element, _element.Text, this.InputID) { ValidationValue = value };
                    _element.RaiseEvent(eventArgs);
                }
            }
        }
        public static readonly RoutedEvent UserApplyTextEvent = EventManager.RegisterRoutedEvent(
            "UserApplyText", RoutingStrategy.Bubble, typeof(UserApplyTextRoutedEventArgs), typeof(UserApplyTextBoxBehavior));
        #endregion

        #region Watch
        public object WatchValue
        {
            get { return (object)GetValue(WatchValueProperty); }
            set { SetValue(WatchValueProperty, value); }
        }
        public static readonly DependencyProperty WatchValueProperty =
            DependencyProperty.Register("WatchValue", typeof(object), typeof(UserApplyTextBoxBehavior), new PropertyMetadata(new PropertyChangedCallback(OnWatchValueChanged)));

        private static void OnWatchValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            UserApplyTextBoxBehavior view = dobj as UserApplyTextBoxBehavior;
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

            OnUpdateText();
        }

        private void OnUpdateText()
        {
            if (_element == null)
            {
                return;
            }

            if (WatchValueConverter == null)
            {
                _watchValueText = (this.WatchValue ?? "").ToString();
            }
            else
            {
                _watchValueText = (WatchValueConverter.Convert(this.WatchValue, null, this.WatchValueConverterParameter, null) ?? "").ToString();

            }
            _element.Text = _watchValueText;
        }
        #endregion

        #region Util
        private bool IsChanged()
        {
            if (_element == null)
            {
                return false;
            }
            return _element.Text != _watchValueText
                && _element.Text != (this.WatchValue ?? "").ToString();
        }
        #endregion


        #region Validators
        public interface IApplyTextValidator
        {
            bool Vaild(string text, out object value, string parameter);
        }

        public static class ValidatorFactory
        {
            public static IApplyTextValidator Create(string text)
            {
                switch (text)
                {
                    case "Double":
                        return new DoubleValidator();
                    case "DoubleAutoRange":
                        return new DoubleAutoRangeValidator();
                    case "Int":
                        return new IntValidator();
                    case "IntAutoRange":
                        return new IntAutoRangeValidator();
                    case "Angle":
                        return new AngleValidator();
                }

                return null;
            }
        }

        /// <summary>
        /// Angle Validator
        /// </summary>
        public class AngleValidator : DoubleValidator
        {
            public override bool Vaild(string text, out object value, string parameter)
            {
                var isValid = base.Vaild(text, out value, parameter);
                if (isValid == true)
                {
                    var token = (parameter ?? "").Trim().Split(',');
                    if (token.Length == 2)
                    {
                        try
                        {
                            var min = double.Parse(token[0]);
                            var max = double.Parse(token[1]);

                            var v = (double)value;

                            v = Math.Max(v, min);
                            v = Math.Min(v, max);

                            value = v;
                            return true;
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        value = null;
                        return false;
                    }
                }


                return isValid;
            }
        }

        public class DoubleValidator : IApplyTextValidator
        {
            public virtual bool Vaild(string text, out object value, string parameter)
            {
                double v = 0d;
                if (double.TryParse(text, out v) == true)
                {
                    value = v;
                    return true;
                }

                value = null;
                return false;
            }
        }

        public class DoubleAutoRangeValidator : DoubleValidator
        {
            public override bool Vaild(string text, out object value, string parameter)
            {
                var isValid = base.Vaild(text, out value, parameter);
                if (isValid == true)
                {
                    var token = (parameter ?? "").Trim().Split(',');
                    if (token.Length == 2)
                    {
                        try
                        {
                            var min = double.Parse(token[0]);
                            var max = double.MaxValue;

                            if ("Max".Equals(token[1]) == false)
                            {
                                max = double.Parse(token[1]);
                            }

                            var v = (double)value;

                            v = Math.Max(v, min);
                            v = Math.Min(v, max);

                            value = v;
                            return true;
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        value = null;
                        return false;
                    }
                }


                return isValid;
            }
        }

        public class IntValidator : IApplyTextValidator
        {
            public virtual bool Vaild(string text, out object value, string parameter)
            {
                int v = 0;
                if (int.TryParse(text, out v) == true)
                {
                    value = v;
                    return true;
                }

                value = null;
                return false;
            }
        }

        public class IntAutoRangeValidator : IntValidator
        {
            public override bool Vaild(string text, out object value, string parameter)
            {
                var isValid = base.Vaild(text, out value, parameter);
                if (isValid == true)
                {
                    var token = (parameter ?? "").Trim().Split(',');
                    if (token.Length == 2)
                    {
                        try
                        {
                            var min = int.Parse(token[0]);
                            var max = int.Parse(token[1]);

                            var v = (int)value;

                            v = Math.Max(v, min);
                            v = Math.Min(v, max);

                            value = v;
                            return true;
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        value = null;
                        return false;
                    }
                }


                return isValid;
            }
        } 
        #endregion
    }

    public class UserApplyTextRoutedEventArgs : RoutedEventArgs
    {
        public string Text { get; set; }
        public string InputID { get; set; }
        public FrameworkElement Element { get; set; }
        public object DataContext { get; set; }
        public object ValidationValue { get; set; }

        public UserApplyTextRoutedEventArgs(RoutedEvent routedEvent, FrameworkElement element, string text, string inputID)
            : base(routedEvent)
        {
            this.Text = text;
            this.InputID = inputID;
            this.Element = element;
            this.DataContext = element?.DataContext;
        }
    }

    
}
