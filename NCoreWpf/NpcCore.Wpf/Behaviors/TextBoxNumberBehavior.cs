using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NpcCore.Wpf.Controls.Behaviors
{
    /// <summary>
    /// Input Mode
    /// </summary>
    public enum InputModes
    {
        Int,
        Double,
    }

    /// <summary>
    /// TextBox Input Behavior
    /// </summary>
    public class TextBoxNumberBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Input Mode
        /// </summary>
        public InputModes InputMode { get; set; }

        /// <summary>
        /// Old Value
        /// </summary>
        private string _oldValue = string.Empty;

        /// <summary>
        /// 소수점 자릿수
        /// </summary>
        public int DecimalPointMaxCount
        {
            get { return (int)GetValue(DecimalPointMaxCountProperty); }
            set { SetValue(DecimalPointMaxCountProperty, value); }
        }
        public static readonly DependencyProperty DecimalPointMaxCountProperty = DependencyProperty.Register("DecimalPointMaxCount", typeof(int), typeof(TextBoxNumberBehavior), new FrameworkPropertyMetadata(3));

        /// <summary>
        /// Empty Value
        /// </summary>
        public string EmptyValue
        {
            get { return (string)GetValue(EmptyValueProperty); }
            set { SetValue(EmptyValueProperty, value); }
        }
        public static readonly DependencyProperty EmptyValueProperty = DependencyProperty.Register("EmptyValue", typeof(string), typeof(TextBoxNumberBehavior), null);

        /// <summary>
        /// Null Value
        /// </summary>
        public string NullValue
        {
            get { return (string)GetValue(NullValueProperty); }
            set { SetValue(NullValueProperty, value); }
        }
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register("NullValue", typeof(string), typeof(TextBoxNumberBehavior), new FrameworkPropertyMetadata("-"));

        /// <summary>
        /// Constructor
        /// </summary>
        public TextBoxNumberBehavior()
        {
            this.InputMode = InputModes.Double;
        }

        /// <summary>
        ///     Attach our behaviour. Add event handlers
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;
            DataObject.AddPastingHandler(AssociatedObject, PastingHandler);
        }

        /// <summary>
        /// TextChanged Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._oldValue == this.AssociatedObject.Text) { return; }

            if (double.TryParse(this.AssociatedObject.Text, out double d))
            {
                string result = string.Empty;

                if (d.Equals(double.NaN))
                {
                    this.AssociatedObject.Text = NullValue;
                }
                else if (!this.ValidateText(this.AssociatedObject.Text, out result))
                {
                    this.AssociatedObject.Text = result;
                }
            }
            else
            {
                this.AssociatedObject.Text = NullValue;
            }

            this._oldValue = this.AssociatedObject.Text;
        }

        /// <summary>
        ///     Deattach our behaviour. remove event handlers
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;
            DataObject.RemovePastingHandler(AssociatedObject, PastingHandler);
        }

        /// <summary>
        /// PreviewTextInput Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string text;
            if (this.AssociatedObject.Text.Length < this.AssociatedObject.CaretIndex)
            {
                text = this.AssociatedObject.Text;
            }
            else
            {
                //  Remaining text after removing selected text.
                if (TreatSelectedText(out string remainingTextAfterRemoveSelection))
                {
                    text = remainingTextAfterRemoveSelection.Insert(AssociatedObject.SelectionStart, e.Text);
                }
                else
                {
                    text = AssociatedObject.Text.Insert(this.AssociatedObject.CaretIndex, e.Text);
                }
            }
            
            string resultText = string.Empty;
            e.Handled = !ValidateText(text, out resultText);
        }

        /// <summary>
        /// PreviewKeyDown Event Handler
        /// </summary>
        void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(this.EmptyValue))
                return;

            string text = null;

            // Handle the Backspace key
            if (e.Key == Key.Back)
            {
                if (!this.TreatSelectedText(out text))
                {
                    if (AssociatedObject.SelectionStart > 0)
                        text = this.AssociatedObject.Text.Remove(AssociatedObject.SelectionStart - 1, 1);
                }
            }
            // Handle the Delete key
            else if (e.Key == Key.Delete)
            {
                // If text was selected, delete it
                if (!this.TreatSelectedText(out text) && this.AssociatedObject.Text.Length > AssociatedObject.SelectionStart)
                {
                    // Otherwise delete next symbol
                    text = this.AssociatedObject.Text.Remove(AssociatedObject.SelectionStart, 1);
                }
            }

            if (text == string.Empty)
            {
                this.AssociatedObject.Text = this.EmptyValue;
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    AssociatedObject.SelectionStart++;
                }

                AssociatedObject.SelectAll();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Pasting Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text));

                string resultText = string.Empty;
                if (!ValidateText(text, out resultText))
                {
                    if (string.IsNullOrEmpty(resultText))
                    {
                        e.CancelCommand();
                    }
                    else
                    {
                        AssociatedObject.Text = resultText;
                        e.CancelCommand();
                    }
                }
            }
            else
                e.CancelCommand();
        }

        /// <summary>
        /// Validate Text
        /// </summary>
        /// <param name="text">Text for validation</param>
        /// <returns> True - valid, False - invalid </returns>
        private bool ValidateText(string text, out string result)
        {
            result = string.Empty;

            switch (this.InputMode)
            {
                case InputModes.Int:
                    {
                        if (int.TryParse(text, out int i))
                        {
                            // bool isMatched = (new Regex(@"^$^|^[0-9]$|^[1-9][0-9]*$", RegexOptions.IgnoreCase)).IsMatch(text);
                            bool isMatched = (new Regex(@"^$|^[-|0-9]$|^[-|1-9][0-9]*$", RegexOptions.IgnoreCase)).IsMatch(text);

                            return isMatched;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case InputModes.Double:
                    {
                        if (double.TryParse(text, out double d))
                        {
                            bool isMatched = (new Regex(@"^-?(\d{0,}([.]\d{0," + DecimalPointMaxCount + @"})?)?$", RegexOptions.IgnoreCase)).IsMatch(text);
                            //bool isMatched = (new Regex(@"^-?(\d{0,}([.]\d{0,})?)?$", RegexOptions.IgnoreCase)).IsMatch(text);
                            if (!isMatched && !d.Equals(double.NaN))
                            {
                                var arr = text.Split('.');
                                if (arr.Length > 1 && arr[1].Length >= DecimalPointMaxCount)
                                {
                                    result = string.Concat(arr[0], ".", arr[1].Substring(0, DecimalPointMaxCount));
                                }
                                //result = text;
                            }

                            return isMatched;
                        }
                        else
                        {
                            return false;
                        }
                    }
            }

            return true;
        }

        /// <summary>
        ///     Handle text selection
        /// </summary>
        /// <returns>true if the character was successfully removed; otherwise, false. </returns>
        private bool TreatSelectedText(out string text)
        {
            text = null;
            if (AssociatedObject.SelectionLength <= 0)
                return false;

            var length = this.AssociatedObject.Text.Length;
            if (AssociatedObject.SelectionStart >= length)
                return true;

            if (AssociatedObject.SelectionStart + AssociatedObject.SelectionLength >= length)
                AssociatedObject.SelectionLength = length - AssociatedObject.SelectionStart;

            text = this.AssociatedObject.Text.Remove(AssociatedObject.SelectionStart, AssociatedObject.SelectionLength);
            return true;
        }
    }
}
