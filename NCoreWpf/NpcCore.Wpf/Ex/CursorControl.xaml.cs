using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NpcCore.Wpf.Ex
{ 
    /// <summary>
    /// CursorControl.xaml에 대한 상호 작용 논리
    /// [ NCS-478 : Custom Mouse Cursor ]
    /// </summary>
    public partial class CursorControl : UserControl
    {
        public CursorInfoModel CursorInfo
        {
            get { return (CursorInfoModel)GetValue(CursorInfoProperty); }
            set { SetValue(CursorInfoProperty, value); }
        }
        public static readonly DependencyProperty CursorInfoProperty = DependencyProperty.Register("CursorInfo", typeof(CursorInfoModel), typeof(CursorControl));

        public CursorControl(CursorInfoModel cursorInfo)
        {
            this.SetCursorInfo(cursorInfo);

            InitializeComponent();
        }

        private void SetCursorInfo(CursorInfoModel cursorInfo)
        {
            this.CursorInfo = cursorInfo;

            var hMargin = 0d;
            if (cursorInfo.ImageSource != null)
            {
                hMargin = cursorInfo.ImageSource.Width / 2d;
            }
            var vMargin = cursorInfo.TextFontSize * -1.2;

            switch (cursorInfo.TextHAlign)
            {
                case HorizontalAlignment.Left:
                    {
                        this.CursorInfo.TextHAlign = HorizontalAlignment.Right;
                        this.CursorInfo.TextMargin = new Thickness(0, 0, hMargin, 0);
                    }
                    break;
                case HorizontalAlignment.Right:
                    {
                        this.CursorInfo.TextHAlign = HorizontalAlignment.Left;
                        this.CursorInfo.TextMargin = new Thickness(hMargin, 0, 0, 0);
                    }
                    break;
            }

            switch (cursorInfo.TextVAlign)
            {
                case VerticalAlignment.Top:
                    {
                        this.CursorInfo.TextRowIndex = 0;
                    }
                    break;
                case VerticalAlignment.Bottom:
                    {
                        this.CursorInfo.TextRowIndex = 2;
                    }
                    break;
            }
        }

        public void UpdateText(string text, Brush background = null)
        {
            if (background != null) { this.CursorInfo.Background = background; }

            this.CursorInfo.Text = text;
            
            this.UpdateLayout();
        }
    }

    public enum CursorTypes
    {
        // Window Default Cursor
        None        = 0,
        ScrollSW    = 1,
        ScrollNE    = 2,
        ScrollNW    = 3,
        ScrollE     = 4,
        ScrollW     = 5,
        ScrollS     = 6,
        ScrollN     = 7,
        ScrollAll   = 8,
        ScrollWE    = 9,
        ScrollNS    = 10,
        Pen         = 11,
        Hand        = 12,
        Wait        = 13,
        UpArrow     = 14,
        SizeWE      = 15,
        SizeNWSE    = 16,
        SizeNS      = 17,
        SizeNESW    = 18,
        SizeAll     = 19,
        IBeam       = 20,
        Help        = 21,
        Cross       = 22,
        AppStarting = 23,
        Arrow       = 24,
        No          = 25,
        ScrollSE    = 26,
        ArrowCD     = 27,

        // RealTime Custom Cursor (50 ~ 99)
        ColorPicker = 50,
        ColorPicker_Mouse=51,

        // Static Custom Cursor (100 ~ ...)
        IgnoreExclude  = 100,
        IgnoreInclude  = 101,
        CoatingExclude = 102,
        CoatingInclude = 103,
        Eraser         = 104,
        Rotate         = 105,
        CrossWhite     = 106,
        CrossBlack     = 107,
        RulerDistance  = 108,
        RulerRectangle = 109,
        Move           = 110,
    };

    public class CursorInfoModel : INotifyPropertyChanged
    {
        private CursorTypes _cursorType;
        /// <summary>
        /// CursorType
        /// </summary>
        public CursorTypes CursorType
        {
            get { return this._cursorType; }
            set
            {
                this._cursorType = value;
                OnPropertyChanged("CursorType");
            }
        }

        private ImageSource _imageSource;
        /// <summary>
        /// Cursor ImageSource
        /// </summary>
        public ImageSource ImageSource
        {
            get { return this._imageSource; }
            set
            {
                this._imageSource = value;
                this.OnPropertyChanged("ImageSource");
            }
        }

        private Brush _background = Brushes.White;
        /// <summary>
        /// Background
        /// </summary>
        public Brush Background
            {
            get { return this._background; }
            set
            {
                this._background = value;
                this.OnPropertyChanged("Background");
            }
        }

        private string _text;
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get { return this._text; }
            set
            {
                this._text = value;
                this.OnPropertyChanged("Text");
            }
        }

        private double _textFontSize = 15d;
        /// <summary>
        /// Text FontSize
        /// </summary>
        public double TextFontSize
        {
            get { return this._textFontSize; }
            set
            {
                this._textFontSize = value;
                this.OnPropertyChanged("TextFontSize");
            }
        }

        private Brush _textForeground = Brushes.Black;
        /// <summary>
        /// Text Foreground
        /// </summary>
        public Brush TextForeground
            {
            get { return this._textForeground; }
            set
            {
                this._textForeground = value;
                this.OnPropertyChanged("TextForeground");
            }
        }

        private HorizontalAlignment _textHAlign = HorizontalAlignment.Right;
        /// <summary>
        /// Text HorizontalAlignment
        /// </summary>
        public HorizontalAlignment TextHAlign
            {
            get { return this._textHAlign; }
            set
            {
                this._textHAlign = value;
                this.OnPropertyChanged("TextHAlign");
            }
        }

        private VerticalAlignment _textVAlign = VerticalAlignment.Bottom;
        /// <summary>
        /// Text VerticalAlignment
        /// </summary>
        public VerticalAlignment TextVAlign
            {
            get { return this._textVAlign; }
            set
            {
                this._textVAlign = value;
                this.OnPropertyChanged("TextVAlign");
            }
        }

        private int _textRowIndex = 1;
        public int TextRowIndex
        {
            get { return this._textRowIndex; }
            set
            {
                this._textRowIndex = value;
                this.OnPropertyChanged("TextRowIndex");
            }
        }

        private Thickness _textMargin = new Thickness();
        /// <summary>
        /// Text Margin
        /// </summary>
        public Thickness TextMargin
            {
            get { return this._textMargin; }
            set
            {
                this._textMargin = value;
                this.OnPropertyChanged("TextMargin");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
