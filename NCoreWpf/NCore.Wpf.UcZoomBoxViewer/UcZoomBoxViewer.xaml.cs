using NpcCore.Wpf.Controls;
using NpcCore.Wpf.Struct_Vision;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace NCore.Wpf.UcZoomBoxViewer
{
    public enum ModeView { Mono, Color }
    public enum ECamState { Stoped, Started}
    public enum EMachineMode 
    {
        [Description("Inspect")]
        EMachineMode_Inspect,
        [Description("Live")]
        EMachineMode_LiveCam,
        [Description("Manual")]
        EMachineMode_ManualTest,
        [Description("Simulator")]
        EMachineMode_Simulator
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UcZoomBoxViewer : UserControl, INotifyPropertyChanged
    {
        private int _camIdx = -1;
        private bool _hasRecipe;
        private bool _isVisibleRecipeButton = true;
        private bool m_bStreamming = false;
        private bool m_bAllInspectionOK = false;
        private BitmapSource _ucBmpSource;
        private IntPtr _bufferView = IntPtr.Zero;

        private int _frameWidth = 640;
        private int _frameHeight = 480;

        private BitmapPalette _palette;
        private int _bufferSize;
        private int _stride;
        private const int _resolutionX = 96;
        private const int _resolutionY = 96;

        private List<string> _machineModeList = new List<string>();
        private string _machineModeSelected = "Simulator";
        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";

        private ModeView _eModeView = ModeView.Mono;
        private ECamState _camState = ECamState.Stoped;
        private EMachineMode _eMachineMode = EMachineMode.EMachineMode_Simulator;

        private CLocatorTool_TemplateMatching_Result m_TemplateMatchingResult = new CLocatorTool_TemplateMatching_Result();

        public UcZoomBoxViewer()
        {
            InitializeComponent();

            this.DataContext = this;

            MachineModeList = GetEnumDescriptionToListString();

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Green);
            colors.Add(System.Windows.Media.Colors.Blue);
            _palette = new BitmapPalette(colors);

            _bufferSize = _frameWidth * _frameHeight * 3;
            _stride = _frameWidth * 3;
        }

        #region Properties
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

                }
            }
        }

        public EMachineMode MachineMode
        {
            get { return _eMachineMode; }
            set
            {
                if (SetProperty(ref _eMachineMode, value))
                {
                    RaiseEvent(new RoutedEventArgs(SwitchMachineModeEvent, this));
                    switch (_eMachineMode)
                    {
                        case EMachineMode.EMachineMode_Inspect:
                            btnContinuousGrab.IsEnabled = false;
                            btnSingleGrab.IsEnabled = false;
                            btnLoadImage.IsEnabled = false;
                            btnContinuousGrab.Opacity = 0.2d;
                            btnSingleGrab.Opacity = 0.2d;
                            btnLoadImage.Opacity = 0.2d;
                            break;
                        case EMachineMode.EMachineMode_LiveCam:
                            btnContinuousGrab.IsEnabled = true;
                            btnSingleGrab.IsEnabled = false;
                            btnLoadImage.IsEnabled = false;
                            btnContinuousGrab.Opacity = 1.0d;
                            btnSingleGrab.Opacity = 0.2d;
                            btnLoadImage.Opacity = 0.2d;
                            break;
                        case EMachineMode.EMachineMode_ManualTest:
                            btnContinuousGrab.IsEnabled = false;
                            btnSingleGrab.IsEnabled = true;
                            btnLoadImage.IsEnabled = false;
                            btnContinuousGrab.Opacity = 0.2d;
                            btnSingleGrab.Opacity = 1.0d;
                            btnLoadImage.Opacity = 0.2d;
                            break;
                        case EMachineMode.EMachineMode_Simulator:
                            btnContinuousGrab.IsEnabled = false;
                            btnSingleGrab.IsEnabled = false;
                            btnLoadImage.IsEnabled = true;
                            btnContinuousGrab.Opacity = 0.2d;
                            btnSingleGrab.Opacity = 0.2d;
                            btnLoadImage.Opacity = 1.0d;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public int CameraIndex
        {
            get => _camIdx;
            set
            {
                if (SetProperty(ref _camIdx, value))
                {

                }
            }
        }
        public bool HasRecipe
        {
            get => _hasRecipe;
            set
            {
                if (SetProperty(ref _hasRecipe, value))
                {
                    if (_hasRecipe)
                    {
                        btnCreateRecipe.Opacity = 0.3;
                        btnUpdateRecipe.Opacity = 1.0;
                    }
                    else
                    {
                        btnCreateRecipe.Opacity = 1.0;
                        btnUpdateRecipe.Opacity = 0.3;
                    }
                }
            }
        }
        public bool IsVisibleRecipeButton
        {
            get => _isVisibleRecipeButton;
            set
            {
                if (SetProperty(ref _isVisibleRecipeButton, value))
                {

                }
            }
        }

        public List<string> MachineModeList
        {
            get => _machineModeList;
            set
            {
                if (SetProperty(ref _machineModeList, value))
                {

                }
            }
        }

        public string MachineModeSelected
        {
            get => _machineModeSelected;
            set
            {
                if (SetProperty(ref _machineModeSelected, value))
                {
                    switch (_machineModeSelected)
                    {
                        case "Inspect":
                            MachineMode = EMachineMode.EMachineMode_Inspect;
                            break;
                        case "Live":
                            MachineMode = EMachineMode.EMachineMode_LiveCam;
                            break;
                        case "Manual":
                            MachineMode = EMachineMode.EMachineMode_ManualTest;
                            break;
                        case "Simulator":
                            MachineMode = EMachineMode.EMachineMode_Simulator;
                            break;
                        default:
                            MachineMode = EMachineMode.EMachineMode_Inspect;
                            break;
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

        public bool IsStreamming
        {
            get => m_bStreamming;
            set
            {
                if (SetProperty(ref m_bStreamming, value))
                {
                    if (m_bStreamming)
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

        public bool IsAllInspectionOK
        {
            get => m_bAllInspectionOK;
            set
            {
                if (SetProperty(ref m_bAllInspectionOK, value))
                {
                    
                }
            }
        }

        public CLocatorTool_TemplateMatching_Result TemplateMatchingResult
        {
            get => m_TemplateMatchingResult;
            set
            {
                if (SetProperty(ref m_TemplateMatchingResult, value))
                {

                }
            }
        }

        #endregion

        // KERNEL FUNCTIONS
        [DllImport("kernel32.dll")]
        public extern static unsafe void ZeroMemory(void* Destination, int Length);
        [DllImport("kernel32.dll")]
        public extern static unsafe void CopyMemory(void* Destination, void* Source, int Length);

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

                    Bitmap pImageBMP = DrawResult(bmp);
                   
                    BitmapSource bmpSrc = BitmapToImageSource(pImageBMP);
                    bmpSrc.Freeze();

                    imageViewer.Dispatcher.Invoke(() => 
                    { 
                        imageViewer.Source = bmpSrc;
                    });
                }
                else if (_eModeView == ModeView.Color)
                {
                    BitmapSource bmpSrc = BitmapSource.Create(FrameWidth, FrameHeight, _resolutionX, _resolutionY, PixelFormats.Bgr24, _palette, _bufferView, BufferSize, stride: Stride);
                    bmpSrc.Freeze();
                    imageViewer.Dispatcher.Invoke(() => imageViewer.Source = bmpSrc);
                }
            });

            task.Start();
            await task;
        }

        private Bitmap DrawResult(Bitmap pImageBMP)
        {
            Bitmap bp = new Bitmap(pImageBMP.Width, pImageBMP.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics gr = Graphics.FromImage(bp))
                gr.DrawImage(pImageBMP, new System.Drawing.Rectangle(0, 0, bp.Width, bp.Height));

            Graphics g = Graphics.FromImage(bp);
            // Create a brush while specifying its color
            if (m_bAllInspectionOK && m_TemplateMatchingResult.m_bResult)
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(m_TemplateMatchingResult.m_nLeft, m_TemplateMatchingResult.m_nTop,
                    m_TemplateMatchingResult.m_nWidth, m_TemplateMatchingResult.m_nHeight);

                System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Blue));
                // Create a pen
                System.Drawing.Pen pen = new System.Drawing.Pen(brush, 2.0f);

                g.DrawRectangle(pen, rect);
            }
            else
            {
                var fontFamily = new System.Drawing.FontFamily("Microsoft Sans Serif");
                var font = new Font(fontFamily, 20, GraphicsUnit.Pixel);
                var solidBrush = new SolidBrush(System.Drawing.Color.Red);

                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.DrawString("Can't Find Template", font, solidBrush, new PointF(20, 20));
            }

            return bp;
        }

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
            List<EMachineMode> modeTests = Enum.GetValues(typeof(EMachineMode))
                                           .Cast<EMachineMode>()
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

        public void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
        }

        #endregion

        #region Event
        public static readonly RoutedEvent CreateRecipeEvent = EventManager.RegisterRoutedEvent(
            "CreateRecipe",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public event RoutedEventHandler CreateRecipe
        {
            add
            {
                base.AddHandler(CreateRecipeEvent, value);
            }
            remove
            {
                base.RemoveHandler(CreateRecipeEvent, value);
            }
        }

        public static readonly RoutedEvent UpdateRecipeEvent = EventManager.RegisterRoutedEvent(
            "UpdateRecipe",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public event RoutedEventHandler UpdateRecipe
        {
            add
            {
                base.AddHandler(UpdateRecipeEvent, value);
            }
            remove
            {
                base.RemoveHandler(UpdateRecipeEvent, value);
            }
        }

        public static readonly RoutedEvent StartCamEvent = EventManager.RegisterRoutedEvent(
            "StartCam",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public event RoutedEventHandler StartCam
        {
            add
            {
                base.AddHandler(StartCamEvent, value);
            }
            remove
            {
                base.RemoveHandler(StartCamEvent, value);
            }
        }

        public static readonly RoutedEvent StopCamEvent = EventManager.RegisterRoutedEvent(
            "StopCam",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public event RoutedEventHandler StopCam
        {
            add
            {
                base.AddHandler(StopCamEvent, value);
            }
            remove
            {
                base.RemoveHandler(StopCamEvent, value);
            }
        }

        public static readonly RoutedEvent UcContinuousGrabEvent = EventManager.RegisterRoutedEvent(
        "UcContinuousGrab",
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ImageExt));

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

        public static readonly RoutedEvent UcSingleGrabEvent = EventManager.RegisterRoutedEvent(
           "UcSingleGrab",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt));

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

        public static readonly RoutedEvent UcLoadImageEvent = EventManager.RegisterRoutedEvent(
           "UcLoadImage",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt));

        public event RoutedEventHandler UcLoadImage
        {
            add
            {
                base.AddHandler(UcLoadImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(UcLoadImageEvent, value);
            }
        }

        public static readonly RoutedEvent SwitchMachineModeEvent = EventManager.RegisterRoutedEvent(
           "SwitchMachineMode",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(ImageExt));

        public event RoutedEventHandler SwitchMachineMode
        {
            add
            {
                base.AddHandler(SwitchMachineModeEvent, value);
            }
            remove
            {
                base.RemoveHandler(SwitchMachineModeEvent, value);
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

        private void btnCreateRecipe_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CreateRecipeEvent, this));
        }

        private void btnUpdateRecipe_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UpdateRecipeEvent, this));
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(StartCamEvent, this));
            CamState = ECamState.Started;
        }

        private void btnContinuousGrab_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UcContinuousGrabEvent, this));
        }

        private void btnSingleGrab_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UcSingleGrabEvent, this));
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(UcLoadImageEvent, this));
        }

        //private void btnStop_Click(object sender, RoutedEventArgs e)
        //{
        //    RaiseEvent(new RoutedEventArgs(StopCamEvent, this));
        //    CamState = ECamState.Stoped;
        //}

    }
}
