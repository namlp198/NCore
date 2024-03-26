using Microsoft.Win32;
using NpcCore.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NCore.Wpf.NUcBufferViewer
{
    public enum ModeGrab
    {
        [Description("None")]
        None = -1,
        [Description("Single Grab")]
        SingleGrab = 0,
        [Description("Continuous Grab")]
        ContinuousGrab = 1
    }
    public enum ETriggerMode
    {
        TriggerMode_None = -1,
        TriggerMode_Internal,
        TriggerMode_External
    }
    public enum ETriggerSource
    {
        TriggerSource_None = -1,
        TriggerSource_Software,
        TriggerSource_Hardware
    }

    public enum ModeView { Mono, Color }
    public enum ECamState { Stoped, Started }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class NUcBufferViewer : UserControl, INotifyPropertyChanged
    {
        private int _camIdx = -1;
        private string _cameraName = string.Empty;
        private bool _isFakeCamera = true;
        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private List<string> _cameraLst = new List<string>();

        private List<string> _modeGrabStr = new List<string>();
        private ModeGrab _modeGrabSelected = ModeGrab.None;
        private ETriggerMode _eTriggerMode;
        private ETriggerSource _eTriggerSource;
        private int _modeGrabSelectedIdx = -1;
        private double m_dOpacity1 = 0.2d;
        private double m_dOpacity2 = 0.2d;

        // ROI and angle rotate
        private Rect _rectOutSide = new Rect();
        private Rect _rectInSide = new Rect();
        private int[] _dataTrain = new int[2];
        private Rect _roi = new Rect();
        private double _angleRotate = 0.0;

        // copy to UcZoomBoxViewer
        private bool _hasRecipe;
        private BitmapSource _ucBmpSource;
        private IntPtr _bufferView = IntPtr.Zero;

        private int _frameWidth = 640;
        private int _frameHeight = 480;

        private BitmapPalette _palette;
        private int _bufferSize;
        private int _stride;
        private const int _resolutionX = 96;
        private const int _resolutionY = 96;

        private ModeView _eModeView = ModeView.Mono;
        private ECamState _camState = ECamState.Stoped;

        public NUcBufferViewer()
        {
            InitializeComponent();

            CameraList.Add("Fake Camera");

            ModeGrabString = GetEnumDescriptionToListString();

            this.DataContext = this;
            scrollViewerExt.ImageExt = imageExt;
            scrollViewerExt.Grid = gridMain;

            imageExt.SelectedROI += ImageExt_SelectedROI;
            imageExt.TrainLocator += ImageExt_TrainLocator;
            imageExt.SaveImage += ImageExt_SaveImage;

            // copy to UcZoomBoxViewer

            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Green);
            colors.Add(System.Windows.Media.Colors.Blue);
            _palette = new BitmapPalette(colors);

            _bufferSize = _frameWidth * _frameHeight;
            _stride = _frameWidth;
        }

        private void ImageExt_SaveImage(object sender, RoutedEventArgs e)
        {
            ImageExt imageExt = (ImageExt)sender;
            ROISelected = imageExt.Rect;
            AngleRotate = imageExt.RectRotation;

            RaiseEvent(new RoutedEventArgs(UcSaveImageEvent, this));
        }

        private void ImageExt_TrainLocator(object sender, RoutedEventArgs e)
        {
            ImageExt imageExt = (ImageExt)sender;
            RectOutSide = imageExt.RectReal;

            Rect rectInside = new Rect(imageExt.RectInside.Left + imageExt.OffsetRect.X, imageExt.RectInside.Top + imageExt.OffsetRect.Y,
                                       imageExt.RectInside.Width, imageExt.RectInside.Height);
            RectInSide = rectInside;

            RaiseEvent(new RoutedEventArgs(UcTrainLocatorEvent, this));
        }

        private void ImageExt_SelectedROI(object sender, RoutedEventArgs e)
        {
            ImageExt imageExt = (ImageExt)sender;
            ROISelected = imageExt.RectReal;
            AngleRotate = imageExt.RectRotation;

            RaiseEvent(new RoutedEventArgs(UcSelectedROIEvent, this));
        }

        #region Convert Bitmap to ImageSource
        /// <summary>
        /// convert Bitmap to ImageSource show on Image WPF
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Convert Bitmap to ImageSource
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(handle);
                return newSource;
            }
            catch (Exception ex)
            {
                DeleteObject(handle);
                return null;
            }
        }

        public BitmapSource ToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            BitmapSource bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Gray8, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
        #endregion

        #region Methods
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (fieldInfo != null)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray != null && attribArray.Length > 0 && attribArray[0] is DescriptionAttribute attrib)
                {
                    return attrib.Description;
                }
            }
            return enumObj.ToString();
        }
        private List<string> GetEnumDescriptionToListString()
        {
            List<string> modeTestString = new List<string>();
            List<ModeGrab> modeTests = Enum.GetValues(typeof(ModeGrab))
                                           .Cast<ModeGrab>()
                                           .ToList();

            foreach (var item in modeTests)
            {
                string s = GetEnumDescription(item);
                //if (s.Equals("Null"))
                //    continue;
                modeTestString.Add(s);
            }

            return modeTestString;
        }

        public void SetParamsModeColor(int fwidth, int fheight)
        {
            FrameWidth = fwidth;
            FrameHeight = fheight;
            Stride = FrameWidth * 3;
            BufferSize = FrameWidth * FrameHeight * 3;
        }

        public async Task UpdateImage()
        {
            Task task = new Task(() =>
            {
                if (_bufferView == IntPtr.Zero)
                    return;

                if (_eModeView == ModeView.Mono)
                {
                    // create "empty" all zeros 24bpp bitmap object
                    Bitmap bmp = new Bitmap(_frameWidth, _frameHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    // create rectangle and lock bitmap into system memory
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, _frameWidth, _frameHeight);

                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

                    unsafe
                    {
                        IntPtr Ptr = (IntPtr)bmpData.Scan0.ToPointer();

                        for (int i = 0; i < _frameHeight; i++)
                        {

                            CopyMemory((byte*)(Ptr + (i * _frameWidth)), (byte*)(_bufferView + (i * _frameWidth)), _frameWidth);
                        }

                        bmp.UnlockBits(bmpData);

                        SetGrayscalePalette(bmp);
                    }

                    Bitmap pImageBMP = bmp;
                    BitmapSource bmpSrc = BitmapToImageSource(pImageBMP);
                    bmpSrc.Freeze();
                    imageExt.Dispatcher.Invoke(() => imageExt.Source = bmpSrc);
                }
                else if (_eModeView == ModeView.Color)
                {
                    BitmapSource bmpSrc = BitmapSource.Create(FrameWidth, FrameHeight, _resolutionX, _resolutionY, PixelFormats.Bgr24, _palette, _bufferView, BufferSize, stride: Stride);
                    bmpSrc.Freeze();
                    imageExt.Dispatcher.Invoke(() => imageExt.Source = bmpSrc);
                }
            });

            task.Start();
            await task;
        }

        public void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
        }
        #endregion

        #region event form
        private void btnLocatorTool_Click(object sender, RoutedEventArgs e)
        {
            if (imageExt.Source == null)
                return;

            imageExt.EnableLocatorTool = true;
            RaiseEvent(new RoutedEventArgs(SettingLocatorEvent, this));
        }
        private void btnSelectROITool_Click(object sender, RoutedEventArgs e)
        {
            if (imageExt.Source == null)
                return;

            imageExt.EnableSelectRoiTool = true;
            RaiseEvent(new RoutedEventArgs(SettingROIEvent, this));
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if(CameraIndex == -1)
            //{
            //    TextBlock textBlock = new TextBlock();
            //    textBlock.Text = "No Camera";
            //    textBlock.Foreground = new SolidColorBrush(Colors.Orange);
            //    textBlock.FontSize = 14;
            //    textBlock.FontWeight = FontWeights.SemiBold;
            //    textBlock.TextAlignment = TextAlignment.Center;
            //    textBlock.VerticalAlignment = VerticalAlignment.Center;
            //    Canvas.SetLeft(textBlock, 15);
            //    Canvas.SetTop(textBlock, 10);
            //    canvasImage.Children.Add(textBlock);
            //}
        }
        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                imageExt.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
        private void btnContinuousGrab_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UcContinuousGrabEvent, this));
        }

        private void btnSingleGrab_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UcSingleGrabEvent, this));
        }
        #endregion

        #region Properties
        public int CameraIndex
        {
            get => _camIdx;
            set
            {
                if(SetProperty(ref _camIdx, value))
                {

                }
            }
        }
        public bool IsFakeCamera
        {
            get => _isFakeCamera;
            set
            {
                if(SetProperty(ref _isFakeCamera, value))
                {
                    if(_isFakeCamera == true)
                    {
                        ModeGrabSelected = ModeGrab.None; // when using Fake Camera mode then Grab Mode also reset become None Mode
                        ModeGrabSelectedIndex = 2; // combobox also select index have item is None

                        DOpacity1 = 0.2d;
                        DOpacity2 = 1.0d;
                    }
                    else
                    {
                        DOpacity1 = 1.0d;
                        DOpacity2 = 0.2d;
                    }
                }
            }
        }

        public double DOpacity1
        {
            get => m_dOpacity1;
            set
            {
                if(SetProperty(ref m_dOpacity1, value))
                {

                }
            }
        }
        public double DOpacity2
        {
            get => m_dOpacity2;
            set
            {
                if (SetProperty(ref m_dOpacity2, value))
                {

                }
            }
        }
        public List<string> ModeGrabString
        {
            get => _modeGrabStr;
            set
            {
                if(SetProperty(ref _modeGrabStr, value))
                {

                }
            }
        }
        public ModeGrab ModeGrabSelected
        {
            get => _modeGrabSelected;
            set
            {
                if (SetProperty(ref _modeGrabSelected, value))
                {
                    switch (_modeGrabSelected)
                    {
                        case ModeGrab.SingleGrab:
                            TriggerMode = ETriggerMode.TriggerMode_External; // Trigger Mode ON
                            TriggerSoure = ETriggerSource.TriggerSource_Software; // Trigger Source = SOFTWARE TRIGGER
                            break;
                        case ModeGrab.ContinuousGrab:
                            TriggerMode = ETriggerMode.TriggerMode_Internal; // Trigger Mode OFF
                            TriggerSoure = ETriggerSource.TriggerSource_Hardware; // Trigger Source = HARDWARE TRIGGER

                            imageExt.ModeGrab = NpcCore.Wpf.Controls.ModeGrab.ModeGrab_ContinuousGrab;

                            break;
                        case ModeGrab.None:
                            TriggerMode = ETriggerMode.TriggerMode_None;
                            TriggerSoure = ETriggerSource.TriggerSource_None;
                            break;
                    }


                }
            }
        }

        public ETriggerMode TriggerMode
        {
            get => _eTriggerMode;
            set
            {
                if (SetProperty(ref _eTriggerMode, value))
                {
                }
            }
        }
        public ETriggerSource TriggerSoure
        {
            get => _eTriggerSource;
            set
            {
                if (SetProperty(ref _eTriggerSource, value))
                {
                }
            }
        }
        public int ModeGrabSelectedIndex
        {
            get => _modeGrabSelectedIdx;
            set
            {
                if(SetProperty(ref _modeGrabSelectedIdx, value))
                {
                    switch(_modeGrabSelectedIdx)
                    {
                        case 0:
                            ModeGrabSelected = ModeGrab.SingleGrab;
                            break;
                        case 1:
                            ModeGrabSelected = ModeGrab.ContinuousGrab;
                            break;
                        default:
                            ModeGrabSelected = ModeGrab.None;
                            break;
                    }
                }
            }
        }

        public List<string> CameraList
        {
            get => _cameraLst;
            set
            {
                if(SetProperty(ref _cameraLst, value))
                {

                }
            }
        }
        public string CameraName
        {
            get => _cameraName;
            set
            {
                if(SetProperty(ref _cameraName, value))
                {
                    if(string.Compare(_cameraName, "Fake Camera") == 0)
                    {
                        IsFakeCamera = true;
                        //CameraIndex = 100; // set index camera for show imageExt
                    }
                    else IsFakeCamera = false;
                }
            }
        }

        public Rect RectInSide
        {
            get => _rectInSide;
            set { if (SetProperty(ref _rectInSide, value)) { } }
        }
        public Rect RectOutSide
        {
            get => _rectOutSide;
            set { if (SetProperty(ref _rectOutSide, value)) { } }
        }
        public int[] DataTrain
        {
            get => _dataTrain;
            set { if(SetProperty(ref _dataTrain, value)) { } }
        }
        public Rect ROISelected
        {
            get => _roi;
            set { if (SetProperty(ref _roi, value)) { } }
        }
        public double AngleRotate
        {
            get => _angleRotate;
            set { if(SetProperty(ref _angleRotate, value)) { } }
        }

        // copy to UcZoomBoxViewer
        public BitmapSource UcBmpSource
        {
            get { return _ucBmpSource; }
            set
            {
                if (SetProperty(ref _ucBmpSource, value))
                {

                }
            }
        }
        public IntPtr BufferView
        {
            get { return _bufferView; }
            set
            {
                if (SetProperty(ref _bufferView, value))
                {

                }
            }
        }
        public int FrameWidth
        {
            get { return _frameWidth; }
            set { _frameWidth = value; }
        }
        public int FrameHeight
        {
            get { return _frameHeight; }
            set { _frameHeight = value; }
        }
        public int Stride
        {
            get { return _stride; }
            set { _stride = value; }
        }
        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }

        public ModeView ModeView
        {
            get { return _eModeView; }
            set
            {
                if (SetProperty(ref _eModeView, value))
                {

                }
            }
        }

        public ECamState CamState
        {
            get { return _camState; }
            set
            {
                if (SetProperty(ref _camState, value))
                {
                    if (CamState == ECamState.Started)
                    {
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/btn_stop_all_50.png";
                    }
                    else
                    {
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
                    }
                }
            }
        }

        public string DisplayImagePath
        {
            get => _displayImagePath;
            set
            {
                if (SetProperty(ref _displayImagePath, value))
                {

                }
            }
        }
        #endregion

        #region Event
        public static readonly RoutedEvent SettingLocatorEvent = EventManager.RegisterRoutedEvent(
            "SettingLocator",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public event RoutedEventHandler SettingLocator
        {
            add
            {
                base.AddHandler(SettingLocatorEvent, value);
            }
            remove
            {
                base.RemoveHandler(SettingLocatorEvent, value);
            }
        }

        public static readonly RoutedEvent SettingROIEvent = EventManager.RegisterRoutedEvent(
           "SettingROI",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt));
        public event RoutedEventHandler SettingROI
        {
            add
            {
                base.AddHandler(SettingROIEvent, value);
            }
            remove
            {
                base.RemoveHandler(SettingROIEvent, value);
            }
        }

        public static readonly RoutedEvent UcSelectedROIEvent = EventManager.RegisterRoutedEvent(
            "UcSelectedROI",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public static readonly RoutedEvent UcSaveImageEvent = EventManager.RegisterRoutedEvent(
            "UcSaveImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public static readonly RoutedEvent UcTrainLocatorEvent = EventManager.RegisterRoutedEvent(
            "UcTrainLocator",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));

        public static readonly RoutedEvent UcContinuousGrabEvent = EventManager.RegisterRoutedEvent(
           "UcContinuousGrab",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt));

        public static readonly RoutedEvent UcSingleGrabEvent = EventManager.RegisterRoutedEvent(
           "UcSingleGrab",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt));

        public event RoutedEventHandler UcSelectedROI
        {
            add
            {
                base.AddHandler(UcSelectedROIEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcSelectedROIEvent, value);
            }
        }
        public event RoutedEventHandler UcSaveImage
        {
            add
            {
                base.AddHandler(UcSaveImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcSaveImageEvent, value);
            }
        }
        public event RoutedEventHandler UcTrainLocator
        {
            add
            {
                base.AddHandler(UcTrainLocatorEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcTrainLocatorEvent, value);
            }
        }
        public event RoutedEventHandler UcContinuousGrab
        {
            add
            {
                base.AddHandler(UcContinuousGrabEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcContinuousGrabEvent, value);
            }
        }
        public event RoutedEventHandler UcSingleGrab
        {
            add
            {
                base.AddHandler(UcSingleGrabEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcSingleGrabEvent, value);
            }
        }
        #endregion

        #region Implement INotifyPropertyChanged
        //
        // Summary:
        //     Occurs when a property value changes.
        public event PropertyChangedEventHandler PropertyChanged;

        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Checks if a property already matches a desired value. Sets the property and notifies
        //     listeners only when necessary.
        //
        // Parameters:
        //   storage:
        //     Reference to a property with both getter and setter.
        //
        //   value:
        //     Desired value for the property.
        //
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support CallerMemberName.
        //
        //   onChanged:
        //     Action that is called after the property value has been changed.
        //
        // Type parameters:
        //   T:
        //     Type of the property.
        //
        // Returns:
        //     True if the value was changed, false if the existing value matched the desired
        //     value.
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   propertyName:
        //     Name of the property used to notify listeners. This value is optional and can
        //     be provided automatically when invoked from compilers that support System.Runtime.CompilerServices.CallerMemberNameAttribute.
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        //
        // Summary:
        //     Raises this object's PropertyChanged event.
        //
        // Parameters:
        //   args:
        //     The PropertyChangedEventArgs
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }
        #endregion

        // KERNEL FUNCTIONS
        [DllImport("kernel32.dll")]
        public extern static unsafe void ZeroMemory(void* Destination, int Length);
        [DllImport("kernel32.dll")]
        public extern static unsafe void CopyMemory(void* Destination, void* Source, int Length);

    }
}
