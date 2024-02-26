using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace NpcCore.Wpf.Ex
{
    public class NumberInputTextBox : TextBox
    {
        public string NumberFormat
        {
            get { return (string)GetValue(NumberFormatProperty); }
            set { SetValue(NumberFormatProperty, value); }
        }
        public static readonly DependencyProperty NumberFormatProperty =
            DependencyProperty.Register("NumberFormat", typeof(string), typeof(NumberInputTextBox), new PropertyMetadata("0.##"));


        public double? TruncateValue
        {
            get { return (double?)GetValue(TruncateValueProperty); }
            set { SetValue(TruncateValueProperty, value); }
        }
        public static readonly DependencyProperty TruncateValueProperty =
            DependencyProperty.Register("TruncateValue", typeof(double?), typeof(NumberInputTextBox));


        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(NumberInputTextBox), new PropertyMetadata(double.MaxValue));



        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(NumberInputTextBox), new PropertyMetadata(double.MinValue));


        public static readonly RoutedEvent ApplyValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ApplyValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double?>), typeof(NumberInputTextBox));

        public event RoutedPropertyChangedEventHandler<double?> ApplyValueChanged
        {
            add { AddHandler(ApplyValueChangedEvent, value); }
            remove { RemoveHandler(ApplyValueChangedEvent, value); }
        }


        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(NumberInputTextBox), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            NumberInputTextBox view = dobj as NumberInputTextBox;
            if (view != null)
            {
                view.OnValueChanged();
            }
        }


        public bool IsInteger
        {
            get { return (bool)GetValue(IsIntegerProperty); }
            set { SetValue(IsIntegerProperty, value); }
        }
        public static readonly DependencyProperty IsIntegerProperty =
            DependencyProperty.Register("IsInteger", typeof(bool), typeof(NumberInputTextBox));

        public NumberInputTextBox()
        {
        }

        private void OnValueChanged()
        {
            var cindex = this.SelectionStart;

            double number = 0;
            if (double.TryParse((this.Value).ToString(), out number) == true)
            {
                double eval = Truncate(number);
                this.ReText(eval.ToString(NumberFormat));
            }
            else
            {
                this.Text = (this.Value).ToString();
            }


            this.SelectionStart = cindex;
        }

        private double Truncate(double number)
        {
            if (IsInteger == true)
            {
                return Math.Truncate(number);
            }
            else
            {
                return this.TruncateValue != null ? Math.Truncate(number * TruncateValue.Value) / TruncateValue.Value : number;
            }
        }
        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    ApplyText(true);
        //}
        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            ApplyText(true);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyText();
            }
            _beforeText = this.Text;
        }
        string _beforeText = "";



        public void ApplyText(bool isOutFocus = false)
        {
            if (this.Text.Trim() == string.Empty)
            {
                OnValueChanged();
                return;
            }

            double number = 0;

            if (double.TryParse(this.Text, out number) == true)
            {
                var eval = this.TruncateValue != null ? Math.Truncate(number * TruncateValue.Value) / TruncateValue.Value : number;

                if (eval > this.Maximum)
                {
                    eval = this.Maximum;
                }
                else if (eval < this.Minimum)
                {
                    eval = this.Minimum;
                }

                this.ReText(eval.ToString(NumberFormat));

                double? oldValue = GetDoubleValue();

                if (IsInteger == true)
                {
                    this.SetValue(NumberInputTextBox.ValueProperty, Math.Truncate(eval));
                }
                else
                {
                    this.SetValue(NumberInputTextBox.ValueProperty, eval);
                }

                var applyValuechangedEventArgs = new RoutedPropertyChangedEventArgs<double?>(oldValue, GetDoubleValue(), NumberInputTextBox.ApplyValueChangedEvent);
                RaiseEvent(applyValuechangedEventArgs);
            }
            else
            {
                //OnValueChanged();

                //if (isOutFocus == false)
                //{
                //    //this.SelectAll();
                //}
            }
        }

        public RoutedPropertyChangedEventArgs<double?> Test()
        {
            var applyValuechangedEventArgs = new RoutedPropertyChangedEventArgs<double?>(GetDoubleValue(), GetDoubleValue(), NumberInputTextBox.ApplyValueChangedEvent);
            RaiseEvent(applyValuechangedEventArgs);

            return applyValuechangedEventArgs;
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Vaildation();
        }

        private void Vaildation()
        {
            double number = 0;
            if (double.TryParse(this.Text, out number) || Regex.IsMatch(this.Text, @"^(?:-?|[1-9]\d*|0)?(?:\.\d+)?$"))
            {
            }
            else
            {
                var cindex = this.SelectionStart;
                this.ReText(_beforeText);
                this.SelectionStart = cindex;
            }
        }

        public double? GetDoubleValue()
        {
            return this.Value as double?;
        }

        private void ReText(string text)
        {
            var cindex = this.SelectionStart;
            this.Text = text;
            this.SelectionStart = cindex;
        }

    }
}
