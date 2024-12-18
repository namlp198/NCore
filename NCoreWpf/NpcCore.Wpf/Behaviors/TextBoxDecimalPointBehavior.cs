﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace NpcCore.Wpf.Controls.Behaviors
{
    public enum TextBoxInputMode
    {
        None,
        DecimalInput,
        DigitInput,
        PercentInput
    }

    public class TextBoxInputBehavior : Behavior<TextBox>
    {
        const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;

        public TextBoxInputBehavior()
        {
            this.InputMode = TextBoxInputMode.None;
            this.JustPositivDecimalInput = false;
            this.MaxVorkommastellen = null;
        }

        public TextBoxInputMode InputMode { get; set; }

        public ushort? MaxVorkommastellen { get; set; }

        public static readonly DependencyProperty JustPositivDecimalInputProperty =
            DependencyProperty.Register("JustPositivDecimalInput", typeof(bool),
                typeof(TextBoxInputBehavior), new FrameworkPropertyMetadata(false));

        public bool JustPositivDecimalInput
        {
            get { return (bool)GetValue(JustPositivDecimalInputProperty); }
            set { SetValue(JustPositivDecimalInputProperty, value); }
        }

        public static readonly DependencyProperty DecimalPointMaxCountProperty =
            DependencyProperty.Register("DecimalPointMaxCount", typeof(int),
                typeof(TextBoxInputBehavior), new FrameworkPropertyMetadata(3));

        public int DecimalPointMaxCount
        {
            get { return (int)GetValue(DecimalPointMaxCountProperty); }
            set { SetValue(DecimalPointMaxCountProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;

            DataObject.AddPastingHandler(AssociatedObject, Pasting);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;

            DataObject.RemovePastingHandler(AssociatedObject, Pasting);
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));

                if (!this.IsValidInput(this.GetText(pastedText)))
                {
                    //System.Media.SystemSounds.Beep.Play();
                    e.CancelCommand();
                }
            }
            else
            {
                //System.Media.SystemSounds.Beep.Play();
                e.CancelCommand();
            }
        }

        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!this.IsValidInput(this.GetText(" ")))
                {
                    //System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Back)
            {
                if (this.AssociatedObject.SelectionLength > 0)
                {
                    if (!this.IsValidInput(this.GetText("")))
                    {
                        //System.Media.SystemSounds.Beep.Play();
                        e.Handled = true;
                    }
                }
                else if (this.AssociatedObject.CaretIndex > 0)
                {
                    var txt = this.AssociatedObject.Text;
                    var backspace = txt.Remove(this.AssociatedObject.CaretIndex - 1, 1);

                    if (!this.IsValidInput(backspace))
                    {
                        //System.Media.SystemSounds.Beep.Play();
                        e.Handled = true;
                    }
                }
            }

            if (e.Key == Key.Delete)
            {
                if (this.AssociatedObject.SelectionLength > 0)
                {
                    if (!this.IsValidInput(this.GetText("")))
                    {
                        //System.Media.SystemSounds.Beep.Play();
                        e.Handled = true;
                    }
                }
                else if (this.AssociatedObject.CaretIndex < this.AssociatedObject.Text.Length)
                {
                    var txt = this.AssociatedObject.Text;
                    var entf = txt.Remove(this.AssociatedObject.CaretIndex, 1);

                    if (!this.IsValidInput(entf))
                    {
                        //System.Media.SystemSounds.Beep.Play();
                        e.Handled = true;
                    }
                }
            }
        }

        private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!this.IsValidInput(this.GetText(e.Text)))
            {
                //System.Media.SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        private string GetText(string input)
        {
            var txt = this.AssociatedObject;

            int selectionStart = txt.SelectionStart;
            if (txt.Text.Length < selectionStart)
                selectionStart = txt.Text.Length;

            int selectionLength = txt.SelectionLength;
            if (txt.Text.Length < selectionStart + selectionLength)
                selectionLength = txt.Text.Length - selectionStart;

            var realtext = txt.Text.Remove(selectionStart, selectionLength);

            int caretIndex = txt.CaretIndex;
            if (realtext.Length < caretIndex)
                caretIndex = realtext.Length;

            var newtext = realtext.Insert(caretIndex, input);

            return newtext;
        }

        private bool IsValidInput(string input)
        {
            if (input.Length == 0)
                return true;

            switch (InputMode)
            {
                case TextBoxInputMode.None:
                    return true;
                case TextBoxInputMode.DigitInput:
                    return CheckIsDigit(input);

                case TextBoxInputMode.DecimalInput:
                    decimal d;
                    if (input.ToCharArray().Where(x => x == ',').Count() > 1)
                        return false;

                    if (input.Contains("-"))
                    {
                        if (this.JustPositivDecimalInput)
                            return false;


                        if (input.IndexOf("-", StringComparison.Ordinal) > 0)
                            return false;

                        if (input.ToCharArray().Count(x => x == '-') > 1)
                            return false;

                        if (input.Length == 1)
                            return true;
                    }

                    var result = decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out d);

                    //소숫점 자리수 확인
                    if (result)
                    {
                        string strResult = d.ToString();
                        int pointIndex = strResult.LastIndexOf(".");
                        if (pointIndex >= 0)
                        {
                            strResult = strResult.Substring(pointIndex + 1, strResult.Length - pointIndex);
                            if (strResult.Length > this.DecimalPointMaxCount)
                                return false;
                        }
                    }

                    return result;

                case TextBoxInputMode.PercentInput:
                    float f;

                    if (input.Contains("-"))
                        return false;

                    if (input.ToCharArray().Where(x => x == ',').Count() > 1)
                        return false;

                    var percentResult = float.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out f);

                    if (MaxVorkommastellen.HasValue)
                    {
                        var vorkomma = Math.Truncate(f);
                        if (vorkomma.ToString(CultureInfo.CurrentCulture).Length > MaxVorkommastellen.Value)
                            return false;
                    }

                    return percentResult;

                default: throw new ArgumentException("Unknown TextBoxInputMode");

            }
        }

        private bool CheckIsDigit(string wert)
        {
            return wert.ToCharArray().All(Char.IsDigit);
        }
    }

}
