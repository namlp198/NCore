﻿#define _DRAW_RESULT

using NpcCore.Wpf.Controls;
using NpcCore.Wpf.MVVM;
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

namespace NCore.Wpf.BufferViewerSettingPRO
{
    public enum EnModeView { Mono, Color }
    public enum EnInspectResult { InspectResult_UNKNOWN, InspectResult_OK, InspectResult_NG }
    public enum EnTriggerMode
    {
        [Description("Internal")]
        TriggerMode_Internal = 0,
        [Description("External")]
        TriggerMode_External,
        [Description("Count")]
        TriggerMode_Count
    }
    public enum EnTriggerSource
    {
        [Description("Software")]
        TriggerSource_Software = 0,
        [Description("Hardware")]
        TriggerSource_Hardware,
        [Description("Count")]
        TriggerSource_Count
    }

    /// <summary>
    /// Interaction logic for BufferViewerSimple.xaml
    /// </summary>
    public partial class BufferViewerSettingPRO : UserControl, INotifyPropertyChanged
    {
        #region variables
        private int m_nCamIdx = -1;
        private int m_nBufferSize;
        private int m_nStride;
        private int m_nFrameWidth = 2448;
        private int m_nFrameHeight = 2048;
        private int m_nFrameIdx = 1;
        private const int m_nResolutionX = 96;
        private const int m_nResolutionY = 96;
        private int[] _dataTrain = new int[2]; // Center Point-> element 0: coordinates X, element 1: coordinates Y

        private string m_sCamName = "[unknown]";
        private string m_sFrameSelected = "1";

        private BitmapSource m_ucBmpSource;
        private BitmapPalette m_palette;
        private IntPtr m_bufferView = IntPtr.Zero;

        private double m_dExposureTime = 0.0;
        private double m_dAnalogGain = 0.0;
        private double m_dAngleRotate = 0.0;
        private double m_dMatchingRate = 0.0;

        private bool m_bStreamming = false;

        // enums
        private EnModeView m_enModeView = EnModeView.Mono;
        private EnInspectResult m_enInspectResult = EnInspectResult.InspectResult_UNKNOWN;
        private EnTriggerMode m_enTriggerMode = EnTriggerMode.TriggerMode_Internal;
        private EnTriggerSource m_enTriggerSource = EnTriggerSource.TriggerSource_Software;

        // list
        private List<string> m_lstCamera = new List<string>();
        private List<string> m_lstTriggerMode = new List<string>();
        private List<string> m_lstTriggerSource = new List<string>();
        private List<int> m_lstFrame = new List<int>();

        // Rect ROI
        private Rect m_rectOutSide = new Rect();
        private Rect m_rectInSide = new Rect();
        private Rect m_roiSelected = new Rect();

        #endregion

        public BufferViewerSettingPRO()
        {
            InitializeComponent();

            this.DataContext = this;

            scrollViewerExt_Basic.ImageExt_Basic = imageExt_Basic;
            scrollViewerExt_Basic.Grid = gridMain;

            imageExt_Basic.SelectedROI += ImageExt_Basic_SelectedROI;
            imageExt_Basic.SaveFullImage += ImageExt_Basic_SaveFullImage;
            imageExt_Basic.SaveROIImage += ImageExt_Basic_SaveROIImage;
            imageExt_Basic.TrainLocator += ImageExt_Basic_TrainLocator;
            imageExt_Basic.Fit += ImageExt_Basic_Fit;

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Green);
            colors.Add(System.Windows.Media.Colors.Blue);
            m_palette = new BitmapPalette(colors);

            m_nBufferSize = m_nFrameWidth * m_nFrameHeight * 3;
            m_nStride = m_nFrameWidth * 3;

            m_lstCamera.Add("[unknown]");
            m_sCamName = m_lstCamera[0];

            m_lstFrame.Add(1);
            m_sFrameSelected = m_lstFrame[0].ToString();

            m_lstTriggerMode = GetEnumDescriptionToListString<EnTriggerMode>();
            m_lstTriggerSource = GetEnumDescriptionToListString<EnTriggerSource>();

            //SetExposureTimeCmd = new AsyncRelayCommand(async () => { await SetExposureTimeAsync(); }, new Action<Exception>((Exception ex) => { MessageBox.Show(ex.Message); }));
        }

        #region Properties
        public BitmapSource UcBmpSource
        {
            get { return m_ucBmpSource; }
            set
            {
                if (SetProperty(ref m_ucBmpSource, value))
                {

                }
            }
        }
        public IntPtr BufferView
        {
            get { return m_bufferView; }
            set
            {
                if (SetProperty(ref m_bufferView, value))
                {
                    //this.Dispatcher.BeginInvoke(new Action(async() =>
                    //{
                    //    await UpdateImage();
                    //}));
                }
            }
        }
        public int FrameWidth
        {
            get { return m_nFrameWidth; }
            set { m_nFrameWidth = value; }
        }
        public int FrameHeight
        {
            get { return m_nFrameHeight; }
            set { m_nFrameHeight = value; }
        }
        public int Stride
        {
            get { return m_nStride; }
            set { m_nStride = value; }
        }
        public int BufferSize
        {
            get { return m_nBufferSize; }
            set { m_nBufferSize = value; }
        }
        public int FrameIdx
        {
            get => m_nFrameIdx;
            set
            {
                if (SetProperty(ref m_nFrameIdx, value))
                {

                }
            }
        }
        public int CameraIndex
        {
            get => m_nCamIdx;
            set
            {
                if (SetProperty(ref m_nCamIdx, value))
                {

                }
            }
        }
        /// <summary>
        /// Note: [element0 - Center X], [element1 - Center Y]
        /// </summary>
        public int[] DataTrain
        {
            get => _dataTrain;
            set { if (SetProperty(ref _dataTrain, value)) { } }
        }
        public double ExposureTime
        {
            get => m_dExposureTime;
            set
            {
                if (!SetProperty(ref m_dExposureTime, value)) { }
            }
        }
        public double AnalogGain
        {
            get => m_dAnalogGain;
            set
            {
                if (!SetProperty(ref m_dAnalogGain, value)) { }
            }
        }
        public double AngleRotate
        {
            get => m_dAngleRotate;
            set { if (SetProperty(ref m_dAngleRotate, value)) { } }
        }
        public double MatchingRate
        {
            get => m_dMatchingRate;
            set { if (SetProperty(ref m_dMatchingRate, value)) { } }
        }
        public EnModeView ModeView
        {
            get { return m_enModeView; }
            set
            {
                if (SetProperty(ref m_enModeView, value))
                {

                }
            }
        }
        public EnInspectResult InspectResult
        {
            get => m_enInspectResult;
            set
            {
                if (SetProperty(ref m_enInspectResult, value))
                {

                }
            }
        }
        public EnTriggerMode TriggerMode
        {
            get => m_enTriggerMode;
            set
            {
                if (SetProperty(ref m_enTriggerMode, value))
                {

                }
            }
        }
        public EnTriggerSource TriggerSource
        {
            get => m_enTriggerSource;
            set
            {
                if (SetProperty(ref m_enTriggerSource, value))
                {

                }
            }
        }
        public string CameraName
        {
            get => m_sCamName;
            set
            {
                if (SetProperty(ref m_sCamName, value))
                {

                }
            }
        }
        public string FrameSelected
        {
            get => m_sFrameSelected;
            set
            {
                if (SetProperty(ref m_sFrameSelected, value)) { }
            }
        }
        public List<string> CameraList
        {
            get => m_lstCamera;
            set
            {
                if (SetProperty(ref m_lstCamera, value)) { }
            }
        }
        public List<string> TriggerModeList
        {
            get => m_lstTriggerMode;
            set
            {
                if (SetProperty(ref m_lstTriggerMode, value))
                {

                }
            }
        }
        public List<string> TriggerSourceList
        {
            get => m_lstTriggerSource;
            set
            {
                if (SetProperty(ref m_lstTriggerSource, value)) { }
            }
        }
        public List<int> FrameList
        {
            get => m_lstFrame;
            set
            {
                if (SetProperty(ref m_lstFrame, value)) { }
            }
        }
        public Rect RectInSide
        {
            get => m_rectInSide;
            set { if (SetProperty(ref m_rectInSide, value)) { } }
        }
        public Rect RectOutSide
        {
            get => m_rectOutSide;
            set { if (SetProperty(ref m_rectOutSide, value)) { } }
        }
        public Rect ROISelected
        {
            get => m_roiSelected;
            set { if (SetProperty(ref m_roiSelected, value)) { } }
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
                        cbbCameraList.IsEnabled = false;
                    }
                    else
                    {
                        cbbCameraList.IsEnabled = true;
                    }
                }
            }
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
                if (m_bufferView == IntPtr.Zero)
                    return;

                if (m_enModeView == EnModeView.Mono)
                {
                    // create "empty" all zeros 24bpp bitmap object
                    Bitmap bmp = new Bitmap(m_nFrameWidth, m_nFrameHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    // create rectangle and lock bitmap into system memory
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, m_nFrameWidth, m_nFrameHeight);

                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

                    unsafe
                    {
                        IntPtr Ptr = (IntPtr)bmpData.Scan0.ToPointer();

                        for (int i = 0; i < m_nFrameHeight; i++)
                        {

                            CopyMemory((byte*)(Ptr + (i * m_nFrameWidth)), (byte*)(m_bufferView + (i * m_nFrameWidth)), m_nFrameWidth);
                        }

                        bmp.UnlockBits(bmpData);

                        SetGrayscalePalette(bmp);
                    }

#if DRAW_RESULT
                    Bitmap pImageBMP = DrawResult(bmp);
#else
                    Bitmap pImageBMP = bmp;
#endif

                    BitmapSource bmpSrc = BitmapToImageSource(pImageBMP);
                    bmpSrc.Freeze();

                    imageExt_Basic.Dispatcher.Invoke(() =>
                    {
                        imageExt_Basic.Source = bmpSrc;
                    });
                }
                else if (m_enModeView == EnModeView.Color)
                {
                    BitmapSource bmpSrc = BitmapSource.Create(FrameWidth, FrameHeight, m_nResolutionX, m_nResolutionY, PixelFormats.Bgr24, m_palette, m_bufferView, BufferSize, stride: Stride);
                    bmpSrc.Freeze();
                    imageExt_Basic.Dispatcher.Invoke(() => imageExt_Basic.Source = bmpSrc);
                }
            });

            task.Start();
            await task;
        }
#if DRAW_RESULT
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
#endif
        private string GetEnumDescription<T>(T enumObj)
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
        private List<string> GetEnumDescriptionToListString<T>()
        {
            List<string> listDesStr = new List<string>();
            List<T> listDes = Enum.GetValues(typeof(T))
                                           .Cast<T>()
                                           .ToList();

            foreach (var item in listDes)
            {
                string s = GetEnumDescription(item);
                //if (s.Equals("Null"))
                //    continue;
                listDesStr.Add(s);
            }

            return listDesStr;
        }
        public void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
        }
        //private Task SetExposureTimeAsync()
        //{
        //    return Task.Run(() =>
        //    {
        //        RaiseEvent(new RoutedEventArgs(SetExposureTimeEvent, this));
        //    });
        //}

        #endregion

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

        #region Events

        // ********************* Show Image Detail *********************
        public static readonly RoutedEvent ShowImageDetailEvent = EventManager.RegisterRoutedEvent(
            "ShowImageDetail",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler ShowDetail
        {
            add
            {
                base.AddHandler(ShowImageDetailEvent, value);
            }
            remove
            {
                base.RemoveHandler(ShowImageDetailEvent, value);
            }
        }

        // ********************* Set Exposure Time *********************
        public static readonly RoutedEvent SetExposureTimeEvent = EventManager.RegisterRoutedEvent(
            "SetExposureTime",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SetExposureTime
        {
            add
            {
                base.AddHandler(SetExposureTimeEvent, value);
            }
            remove
            {
                base.RemoveHandler(SetExposureTimeEvent, value);
            }
        }

        // ********************* Set Analog Gain *********************
        public static readonly RoutedEvent SetAnalogGainEvent = EventManager.RegisterRoutedEvent(
            "SetAnalogGain",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SetAnalogGain
        {
            add
            {
                base.AddHandler(SetAnalogGainEvent, value);
            }
            remove
            {
                base.RemoveHandler(SetAnalogGainEvent, value);
            }
        }

        // ********************* Select Camera Change *********************
        public static readonly RoutedEvent SelectCameraChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectCameraChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SelectCameraChanged
        {
            add
            {
                base.AddHandler(SelectCameraChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(SelectCameraChangedEvent, value);
            }
        }

        // ********************* Select Frame Change *********************
        public static readonly RoutedEvent SelectFrameChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectFrameChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SelectFrameChanged
        {
            add
            {
                base.AddHandler(SelectFrameChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(SelectFrameChangedEvent, value);
            }
        }

        // ********************* Select Trigger Mode *********************
        public static readonly RoutedEvent SelectTriggerModeChangedEvent = EventManager.RegisterRoutedEvent(
           "SelectTriggerModeChanged",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SelectTriggerModeChanged
        {
            add
            {
                base.AddHandler(SelectTriggerModeChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(SelectTriggerModeChangedEvent, value);
            }
        }

        // ********************* Select Trigger Source *********************
        public static readonly RoutedEvent SelectTriggerSourceChangedEvent = EventManager.RegisterRoutedEvent(
           "SelectTriggerSourceChanged",
           RoutingStrategy.Bubble,
           typeof(RoutedEventHandler),
           typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SelectTriggerSourceChanged
        {
            add
            {
                base.AddHandler(SelectTriggerSourceChangedEvent, value);
            }
            remove
            {
                base.RemoveHandler(SelectTriggerSourceChangedEvent, value);
            }
        }

        // ********************* ROI Selection Done *********************
        public static readonly RoutedEvent ROISelectionDoneEvent = EventManager.RegisterRoutedEvent(
            "ROISelectionDone",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler ROISelectionDone
        {
            add
            {
                base.AddHandler(ROISelectionDoneEvent, value);
            }
            remove
            {
                base.RemoveHandler(ROISelectionDoneEvent, value);
            }
        }

        // ********************* Save Full Image *********************
        public static readonly RoutedEvent SaveFullImageEvent = EventManager.RegisterRoutedEvent(
            "SaveFullImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SaveFullImage
        {
            add
            {
                base.AddHandler(SaveFullImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(SaveFullImageEvent, value);
            }
        }

        // ********************* Save ROI Image *********************
        public static readonly RoutedEvent SaveROIImageEvent = EventManager.RegisterRoutedEvent(
            "SaveROIImage",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler SaveROIImage
        {
            add
            {
                base.AddHandler(SaveROIImageEvent, value);
            }
            remove
            {
                base.RemoveHandler(SaveROIImageEvent, value);
            }
        }

        // ********************* Train Locator *********************
        public static readonly RoutedEvent TrainLocatorEvent = EventManager.RegisterRoutedEvent(
            "TrainLocator",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(BufferViewerSettingPRO));
        public event RoutedEventHandler TrainLocator
        {
            add
            {
                base.AddHandler(TrainLocatorEvent, value);
            }
            remove
            {
                base.RemoveHandler(TrainLocatorEvent, value);
            }
        }
        #endregion

        #region Commands
        public ICommand SetExposureTimeCmd { get; }
        public ICommand SetAnalogGainCmd { get; }
        #endregion

        #region Form Event
        private void btnShowDetail_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ShowImageDetailEvent, this));
        }
        private void btnSetExposureTime_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SetExposureTimeEvent, this));
        }
        private void txtExposureTime_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SetExposureTimeEvent, this));
        }
        private void btnSetAnalogGain_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SetAnalogGainEvent, this));
        }
        private void txtAnalogGain_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SetAnalogGainEvent, this));
        }

        private void cbbCameraList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CameraName = "[ " + cbbCameraList.SelectedItem.ToString() + " ]";
            CameraIndex = cbbCameraList.SelectedIndex;

            RaiseEvent(new RoutedEventArgs(SelectCameraChangedEvent, this));
        }

        private void cbbFrameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SelectFrameChangedEvent, this));
        }

        private void cbbTriggerMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.Compare(m_lstTriggerMode[cbbTriggerMode.SelectedIndex], "Internal") == 0)
            {
                TriggerMode = EnTriggerMode.TriggerMode_Internal;
            }
            else if (string.Compare(m_lstTriggerMode[cbbTriggerMode.SelectedIndex], "External") == 0)
            {
                TriggerMode = EnTriggerMode.TriggerMode_External;
            }
            else
            {
                TriggerMode = EnTriggerMode.TriggerMode_Count;
            }

            RaiseEvent(new RoutedEventArgs(SelectTriggerModeChangedEvent, this));
        }

        private void cbbTriggerSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.Compare(m_lstTriggerSource[cbbTriggerSource.SelectedIndex], "Software") == 0)
            {
                TriggerSource = EnTriggerSource.TriggerSource_Software;
            }
            else if (string.Compare(m_lstTriggerSource[cbbTriggerSource.SelectedIndex], "Hardware") == 0)
            {
                TriggerSource = EnTriggerSource.TriggerSource_Hardware;
            }
            else
            {
                TriggerSource = EnTriggerSource.TriggerSource_Count;
            }
            RaiseEvent(new RoutedEventArgs(SelectTriggerSourceChangedEvent, this));
        }
        private void ImageExt_Basic_TrainLocator(object sender, RoutedEventArgs e)
        {
            ImageExt_Basic imageExt_Basic = (ImageExt_Basic)sender;
            RectOutSide = imageExt_Basic.RectReal;

            Rect rectInside = new Rect(imageExt_Basic.RectInside.Left + imageExt_Basic.OffsetRect.X, imageExt_Basic.RectInside.Top + imageExt_Basic.OffsetRect.Y,
                                       imageExt_Basic.RectInside.Width, imageExt_Basic.RectInside.Height);
            RectInSide = rectInside;

            RaiseEvent(new RoutedEventArgs(TrainLocatorEvent, this));
        }

        private void ImageExt_Basic_SaveFullImage(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SaveFullImageEvent, this));
        }
        private void ImageExt_Basic_SaveROIImage(object sender, RoutedEventArgs e)
        {
            ImageExt_Basic imageExt_Basic = (ImageExt_Basic)sender;
            ROISelected = imageExt_Basic.RectReal;
            AngleRotate = imageExt_Basic.RectRotation;

            RaiseEvent(new RoutedEventArgs(SaveROIImageEvent, this));
        }
        private void ImageExt_Basic_SelectedROI(object sender, RoutedEventArgs e)
        {
            ImageExt_Basic imageExt_Basic = (ImageExt_Basic)sender;
            ROISelected = imageExt_Basic.RectReal;
            AngleRotate = imageExt_Basic.RectRotation;

            RaiseEvent(new RoutedEventArgs(ROISelectionDoneEvent, this));
        }
        private void ImageExt_Basic_Fit(object sender, RoutedEventArgs e)
        {
            scrollViewerExt_Basic.Reset();
        }
        #endregion

        // KERNEL FUNCTIONS
        [DllImport("kernel32.dll")]
        public extern static unsafe void ZeroMemory(void* Destination, int Length);
        [DllImport("kernel32.dll")]
        public extern static unsafe void CopyMemory(void* Destination, void* Source, int Length);
    }
}
