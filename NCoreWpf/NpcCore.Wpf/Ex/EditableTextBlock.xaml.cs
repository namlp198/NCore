using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NpcCore.Wpf.Ex
{
    /// <summary>
    /// EditableTextBlock.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditableTextBlock : UserControl
    {
        #region [ =============== Field =============== ]
        /// <summary>
        /// Old Text
        /// </summary>
        private string oldText;
        #endregion // [ =============== Field =============== ]


        #region [ =============== Property =============== ]
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnTextChangedCallBack)) { BindsTwoWayByDefault = true });

        /// <summary>
        /// Sub Text
        /// </summary>
        public string SubText
        {
            get { return (string)GetValue(SubTextProperty); }
            set { SetValue(SubTextProperty, value); }
        }
        public static readonly DependencyProperty SubTextProperty = DependencyProperty.Register("SubText", typeof(string), typeof(EditableTextBlock), new FrameworkPropertyMetadata("") );

        /// <summary>
        /// Is Editable
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditableTextBlock), new FrameworkPropertyMetadata(true) { BindsTwoWayByDefault = true });

        /// <summary>
        /// Is InEditMode
        /// </summary>
        public bool IsInEditMode
        {
            get 
            {
                if (IsEditable)
                {
                    return (bool)GetValue(IsInEditModeProperty);
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (IsEditable)
                {
                    if (value)
                    {
                        oldText = Text;
                    }
                    SetValue(IsInEditModeProperty, value);
                }
            }
        }
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTextBlock), new FrameworkPropertyMetadata(false) { BindsTwoWayByDefault = true });

        /// <summary>
        /// Text Format
        /// </summary>
        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set
            {
                if (value == "")
                {
                    value = "{0}";
                }
                SetValue(TextFormatProperty, value);
            }
        }
        public static readonly DependencyProperty TextFormatProperty = DependencyProperty.Register("TextFormat", typeof(string), typeof(EditableTextBlock), new PropertyMetadata("{0}"));

        /// <summary>
        /// Formatted Text
        /// </summary>
        public string FormattedText
        {
            get { return String.Format(TextFormat.Replace("[[", "{").Replace("]]", "}"), Text, SubText); }
        }
        #endregion // [ =============== Property =============== ]


        #region [ =============== Constructor / Initialization =============== ]
        /// <summary>
        /// Constructor
        /// </summary>
        public EditableTextBlock()
        {
            InitializeComponent();

            base.Focusable = true;
            base.FocusVisualStyle = null;
        }
        #endregion // [ =============== Constructor / Initialization =============== ]


        #region [ =============== Event Action =============== ]
        /// <summary>
        /// TextBox Loaded Event Action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.Focus();
            txt.SelectAll();
        }

        /// <summary>
        /// TextBox LostFocus Event Action (exit edit mode)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsInEditMode = false;
            if (this.oldText != this.Text)
            {
                OnTextChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// TextBox KeyDown Event Action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.IsInEditMode = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.IsInEditMode = false;
                Text = oldText;
                e.Handled = true;
            }
        }

        private static void OnTextChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EditableTextBlock obj = sender as EditableTextBlock;
            if (obj != null)
            {
                //obj.OnTextChanged(sender, new EventArgs());
            }
        }

        public event TextChangedHandler TextChanged;
        public delegate void TextChangedHandler (object sender, EventArgs e);

        protected virtual void OnTextChanged(DependencyObject sender, EventArgs e)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(sender, e);
            }
        }
        #endregion // [ =============== Event Action =============== ]
    }
}
