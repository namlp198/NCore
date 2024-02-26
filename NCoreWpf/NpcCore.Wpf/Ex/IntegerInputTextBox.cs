using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NpcCore.Wpf.Ex
{
    public class IntegerInputTextBox : TextBox
    {
        public static readonly RoutedEvent ApplyValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ApplyValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int?>), typeof(IntegerInputTextBox));

        public event RoutedPropertyChangedEventHandler<int?> ApplyValueChanged
        {
            add { AddHandler(ApplyValueChangedEvent, value); }
            remove { RemoveHandler(ApplyValueChangedEvent, value); }
        }


        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(IntegerInputTextBox), new PropertyMetadata(int.MinValue));


        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(IntegerInputTextBox), new PropertyMetadata(int.MinValue));



        public int? Value
        {
            get { return (int?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int?), typeof(IntegerInputTextBox), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject dobj, DependencyPropertyChangedEventArgs e)
        {
            IntegerInputTextBox view = dobj as IntegerInputTextBox;
            if (view != null && e.NewValue is int?)
            {
                view.OnValueChanged(e.OldValue as int?, e.NewValue as int?);
            }
        }

        public int IsContainsValue(int? value)
        {
            if (value != null)
            {
                int v = (int)value;
                if (v < this.Minimum)
                {
                    return -1;
                }
                else if (v > this.Maximum)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void OnValueChanged(int? oldValue, int? value)
        {
            Vaildation(value == null ? "" : value.ToString());
        }

        protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            ApplyValue();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyValue();
            }
        }

        private void ApplyValue()
        {
            Vaildation(this.Text);
            
            var applyValuechangedEventArgs = new RoutedPropertyChangedEventArgs<int?>(null, this.Value, IntegerInputTextBox.ApplyValueChangedEvent);
            RaiseEvent(applyValuechangedEventArgs);

            
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Vaildation(this.Text);
        }

        private void Vaildation(string text)
        {
            int value;
            if (string.IsNullOrWhiteSpace(text) == true)
            {
                this.Text = string.Empty;
                this.SetValue(IntegerInputTextBox.ValueProperty, null);
            }
            else if (int.TryParse(text, out value) == true)
            {
                var compare = IsContainsValue(value);
                if (compare > 0)
                {
                    value = this.Maximum;
                }
                else if (compare < 0)
                {
                    value = this.Minimum;
                }
                this.Text = value.ToString();
                this.SetValue(IntegerInputTextBox.ValueProperty, value);
            }
            else
            {
                this.Text = string.Empty;
                this.SetValue(IntegerInputTextBox.ValueProperty, null);
            }
        }
    }
}
