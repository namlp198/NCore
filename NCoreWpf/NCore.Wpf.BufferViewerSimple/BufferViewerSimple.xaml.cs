﻿#define DRAW_RESULT

using NCore.Wpf.BufferViewerSimple.Model;
using NpcCore.Wpf.Controls;
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

namespace NCore.Wpf.BufferViewerSimple
{
    public enum emModeView { Mono, Color }
    public enum emInspectResult { InspectResult_UNKNOWN, InspectResult_OK, InspectResult_NG }

    /// <summary>
    /// Interaction logic for BufferViewerSimple.xaml
    /// </summary>
    public partial class BufferViewerSimple : UserControl, INotifyPropertyChanged
    {
        #region variables
        private int _camIdx = -1;
        private string _camName = "[unknown]";
        private BitmapSource _ucBmpSource;
        private IntPtr _bufferView = IntPtr.Zero;

        private int _frameWidth = 2448;
        private int _frameHeight = 2048;

        private BitmapPalette _palette;
        private int _bufferSize;
        private int _stride;
        private const int _resolutionX = 96;
        private const int _resolutionY = 96;

        System.Drawing.Pen penBlue;
        System.Drawing.Pen penYellow;
        System.Drawing.Pen penRed;
        System.Drawing.FontFamily fontFamily;
        Font font;
        SolidBrush solidBrushYellow;
        SolidBrush solidBrushRed;
        SolidBrush solidBrushBlue;
        SolidBrush solidBrushGreen;

        private emModeView _eModeView = emModeView.Mono;
        private emInspectResult _eInspectResult = emInspectResult.InspectResult_UNKNOWN;
        #endregion

        public BufferViewerSimple()
        {
            InitializeComponent();

            this.DataContext = this;

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Green);
            colors.Add(System.Windows.Media.Colors.Blue);
            _palette = new BitmapPalette(colors);

            _bufferSize = _frameWidth * _frameHeight * 3;
            _stride = _frameWidth * 3;

            fontFamily = new System.Drawing.FontFamily("Microsoft Sans Serif");
            font = new Font(fontFamily, 20, GraphicsUnit.Pixel);
            solidBrushYellow = new SolidBrush(System.Drawing.Color.Yellow);
            solidBrushRed = new SolidBrush(System.Drawing.Color.Red);
            solidBrushBlue = new SolidBrush(System.Drawing.Color.Blue);
            solidBrushGreen = new SolidBrush(System.Drawing.Color.Green);

            penBlue = new System.Drawing.Pen(solidBrushBlue, 3.0f);
            penRed = new System.Drawing.Pen(solidBrushRed, 3.0f);
            penYellow = new System.Drawing.Pen(solidBrushYellow, 3.0f);
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
                    //this.Dispatcher.BeginInvoke(new Action(async() =>
                    //{
                    //    await UpdateImage();
                    //}));
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

        public emModeView ModeView
        {
            get { return _eModeView; }
            set
            {
                if (SetProperty(ref _eModeView, value))
                {

                }
            }
        }
        public emInspectResult InspectResult
        {
            get => _eInspectResult;
            set
            {
                if(SetProperty(ref _eInspectResult, value))
                {
                   
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
        public string CameraName
        {
            get => _camName;
            set
            {
                if(SetProperty(ref _camName, value))
                {

                }
            }
        }

        private int m_nNumberOfROICntPxl;
        public int NumberOfROICntPxl
        {
            get => m_nNumberOfROICntPxl;
            set => m_nNumberOfROICntPxl = value;
        }

        private LocatorModel m_locatorModel;
        public LocatorModel LocatorModel { get => m_locatorModel; set => m_locatorModel = value; }
        private CountPixelModel[] m_cntPxlModels;
        public CountPixelModel[] CountPixelModels { get => m_cntPxlModels; set => m_cntPxlModels = value; }
        #endregion

        #region Methods
        public void InitModels(int nNumberOfROICntPxl, params int[] rectOuter)
        {
            this.m_nNumberOfROICntPxl = nNumberOfROICntPxl;

            m_locatorModel = new LocatorModel();
            m_locatorModel.Init();
            m_locatorModel.RectOuter = rectOuter;

            m_cntPxlModels = new CountPixelModel[m_nNumberOfROICntPxl];
            for(int i = 0; i < m_cntPxlModels.Length; i++)
            {
                m_cntPxlModels[i] = new CountPixelModel();
                m_cntPxlModels[i].Init();
            }
        }
        public void SetParamsModeColor(int fwidth, int fheight)
        {
            FrameWidth = fwidth;
            FrameHeight = fheight;
            Stride = FrameWidth * 3;
            BufferSize = FrameWidth * FrameHeight * 3;
        }
        public void SetParamsModeMono(int fwidth, int fheight)
        {
            FrameWidth = fwidth;
            FrameHeight = fheight;
            Stride = FrameWidth * 1;
            BufferSize = FrameWidth * FrameHeight * 1;
        }

        public async Task UpdateImage()
        {
            Task task = new Task(() =>
            {
                if (_bufferView == IntPtr.Zero)
                    return;

                if (_eModeView == emModeView.Mono)
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

#if DRAW_RESULT
                    Bitmap pImageBMP = DrawResult(bmp);
#else
                    Bitmap pImageBMP = bmp;
#endif

                    BitmapSource bmpSrc = BitmapToImageSource(pImageBMP);
                    bmpSrc.Freeze();

                    imageViewer.Dispatcher.Invoke(() =>
                    {
                        imageViewer.Source = bmpSrc;
                    });
                }
                else if (_eModeView == emModeView.Color)
                {
                    BitmapSource bmpSrc = BitmapSource.Create(FrameWidth, FrameHeight, _resolutionX, _resolutionY, PixelFormats.Bgr24, _palette, _bufferView, BufferSize, stride: Stride);
                    bmpSrc.Freeze();
                    imageViewer.Dispatcher.Invoke(() => imageViewer.Source = bmpSrc);
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
            if (LocatorModel.Result)
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(LocatorModel.RectOuter[0], LocatorModel.RectOuter[1],
                    LocatorModel.RectOuter[2], LocatorModel.RectOuter[3]);

                System.Drawing.Brush brushBlue = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Blue));
                System.Drawing.Brush brushYellow = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Yellow));
                // Create a pen
                System.Drawing.Pen penBlue = new System.Drawing.Pen(brushBlue, 3.0f);
                System.Drawing.Pen penYellow = new System.Drawing.Pen(brushYellow, 5.0f);

                int cnt_x = LocatorModel.CenterPt[0];
                int cnt_y = LocatorModel.CenterPt[1];
                string s = string.Format("(x:{0}-y:{1})", cnt_x, cnt_y);

                g.DrawRectangle(penBlue, rect);
                g.DrawEllipse(penYellow, cnt_x, cnt_y, 3, 3);

                g.DrawString(s, font, solidBrushYellow, new PointF(cnt_x + 10, cnt_y - 10));
            }
            else
            {
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                g.DrawString("Can't Find Template", font, solidBrushRed, new PointF(20, 20));
            }

            foreach (CountPixelModel cntpxl in CountPixelModels)
            {
                if (cntpxl.Result)
                {
                    int centerPtx = cntpxl.ROI_CountPixel[0] + cntpxl.ROI_CountPixel[2] / 2;
                    int centerPty = cntpxl.ROI_CountPixel[1] + cntpxl.ROI_CountPixel[3] / 2;

                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(cntpxl.ROI_CountPixel[0], cntpxl.ROI_CountPixel[1],
                        cntpxl.ROI_CountPixel[2], cntpxl.ROI_CountPixel[3]);

                    System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Green));
                    // Create a pen
                    System.Drawing.Pen pen = new System.Drawing.Pen(brush, 3.0f);

                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.DrawString(cntpxl.NumberOfPixel + "", font, solidBrushGreen, new PointF(centerPtx, centerPty));

                    g.DrawRectangle(pen, rect);
                }
                else
                {
                    int centerPtx = cntpxl.ROI_CountPixel[0] + cntpxl.ROI_CountPixel[2] / 2;
                    int centerPty = cntpxl.ROI_CountPixel[1] + cntpxl.ROI_CountPixel[3] / 2;

                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(cntpxl.ROI_CountPixel[0], cntpxl.ROI_CountPixel[1],
                        cntpxl.ROI_CountPixel[2], cntpxl.ROI_CountPixel[3]);

                    System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Red));
                    // Create a pen
                    System.Drawing.Pen pen = new System.Drawing.Pen(brush, 3.0f);

                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.DrawString(cntpxl.NumberOfPixel + "", font, solidBrushRed, new PointF(centerPtx, centerPty));

                    g.DrawRectangle(pen, rect);
                }
            }

            return bp;
        }
#endif

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

        public void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
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
        public static readonly RoutedEvent ShowDetailEvent = EventManager.RegisterRoutedEvent(
            "ShowDetail",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(ImageExt));
        public event RoutedEventHandler ShowDetail
        {
            add
            {
                base.AddHandler(ShowDetailEvent, value);
            }
            remove
            {
                base.RemoveHandler(ShowDetailEvent, value);
            }
        }
        #endregion
        private void btnShowDetail_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ShowDetailEvent, this));
        }

        private void btnFit_Click(object sender, RoutedEventArgs e)
        {
            zoomBorder.Reset();
        }
    }
}
