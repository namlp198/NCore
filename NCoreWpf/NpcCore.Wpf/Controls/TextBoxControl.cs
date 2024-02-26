using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class TextBoxControl : TextBox
    {
        #region Fields
        public static readonly DependencyProperty IsRequiredFieldProperty;
        public static readonly DependencyProperty CornerRadiusProperty;
        public static readonly DependencyProperty TagTemplateProperty;
        private Border _borderTextBox;
        private ScrollViewer _ScrollViewerTextBox;
        #endregion
        #region Constructor
        static TextBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxControl), new FrameworkPropertyMetadata(typeof(TextBoxControl)));

            IsRequiredFieldProperty = DependencyProperty.Register("IsRequiredField", typeof(bool), typeof(TextBoxControl), new UIPropertyMetadata(false));
            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(double), typeof(TextBoxControl), new UIPropertyMetadata(0d));
            TagTemplateProperty = DependencyProperty.Register("TagTemplate", typeof(DataTemplate),
                                                               typeof(TextBoxControl),
                                                               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _borderTextBox = this.GetTemplateChild("Border") as Border;
            if (_borderTextBox != null)
            {
                _borderTextBox.CornerRadius = new CornerRadius(CornerRadius);
            }
            if (IsRequiredField && !String.IsNullOrEmpty(Tag.ToString()))
            {
                this.Tag += " *";
            }

            // Initialize stuff added for the first time
            exceedOffset = 3.0;
            _internalEnabled = true;
            LayoutUpdated += new EventHandler(TextBoxWithEllipsis_LayoutUpdated);
            _ScrollViewerTextBox = GetTemplateChild("PART_ContentHost") as ScrollViewer;
        }

        private void TextBoxWithEllipsis_LayoutUpdated(object sender, EventArgs e)
        {
            if (_internalEnabled)
            {
                if ((this.ViewportWidth + exceedOffset < this.ExtentWidth) || (this.ViewportHeight + exceedOffset < this.ExtentHeight))
                {
                    // The current Text (whose length without ellipsis is _curLen) is too long.
                    _lastLongLen = _curLen;
                }
                else
                {
                    // The current Text is not too long.
                    _lastFitLen = _curLen;
                }

                // Try a new substring whose length is halfway between the last length
                // known to fit and the last length known to be too long.
                int newLen = (_lastFitLen + _lastLongLen) / 2;

                if (_curLen == newLen)
                {
                    if (this.Text == _longText)
                    {

                        ToolTip = null;
                    }
                    else
                    {
                        AddToolTip();
                    }

                }
                else
                {
                    _curLen = newLen;

                    // This sets the Text property without raising the TextChanged event.
                    // However it does raise the LayoutUpdated event again, though
                    // not recursively.
                    CalcText();
                }
            }
            else if (ViewportWidth + exceedOffset < ExtentWidth)
            {
                // The current Text is too long.
                AddToolTip();
            }
            else
            {
                // The current Text is not too long.
                ToolTip = null;
            }


        }

        private void AddToolTip()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.Content = _longText;
            toolTip.FontSize = this.FontSize;
            toolTip.Background = this.Background;
            toolTip.Foreground = this.Foreground;
            ToolTip = toolTip;
        }

        private void PrepareForLayout()
        {
            _lastFitLen = 0;
            _lastLongLen = _longText.Length;
            _curLen = _longText.Length;

            // This raises the LayoutUpdated event, whose
            // handler does the ellipsis.
            SetText(_longText);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {

            if (_externalChange)
            {
                _longText = this.Text ?? "";
                //AddToolTip();
                PrepareForLayout();
                base.OnTextChanged(e);
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            _internalEnabled = true;
            PrepareForLayout();
            base.OnLostKeyboardFocus(e);

            // [NCS-2695] CID 171168 Dereference null return (stat)
            //this.Text = this.Text.Trim();
            this.Text = this.Text?.Trim();

        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            _internalEnabled = false;
            SetText(_longText);
            base.OnGotKeyboardFocus(e);
        }
        private void CalcText()
        {
            SetText(_longText.Substring(0, _curLen) + "\u2026");
        }
        private void SetText(string text)
        {
            if (this.Text != text)
            {
                _externalChange = false;
                this.Text = text; // Will trigger Layout event.
                _externalChange = true;
            }

        }
        #endregion
        #region Control Properties
        public double CornerRadius
        {
            get
            {
                return (double)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public bool IsRequiredField
        {
            get
            {
                return (bool)GetValue(IsRequiredFieldProperty);
            }
            set
            {
                SetValue(IsRequiredFieldProperty, value);
            }
        }

        public DataTemplate TagTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TagTemplateProperty);
            }
            set
            {
                SetValue(TagTemplateProperty, value);
            }
        }
        #endregion

        #region Variable for TipTool and Ellipsis
        public double exceedOffset;

        // Last length of substring of LongText known to be too long.
        // Used while calculating the correct length to fit.
        private int _lastLongLen;

        // Last length of substring of LongText known to fit.
        // Used while calculating the correct length to fit.
        private int _lastFitLen = 0;
        // Backer for LongText.
        private string _longText = "";

        // Length of substring of LongText currently assigned to the Text property.
        // Used while calculating the correct length to fit.
        private int _curLen;

        // Used to detect whether the OnTextChanged event occurs due to an
        // external change vs. an internal one.
        private bool _externalChange = true;

        // Used to disable ellipsis internally (primarily while
        // the control has the focus).
        private bool _internalEnabled = true;
        #endregion
    }
}
