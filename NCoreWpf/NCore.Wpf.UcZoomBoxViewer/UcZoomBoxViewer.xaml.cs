using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NCore.Wpf.UcZoomBoxViewer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UcZoomBoxViewer : UserControl, INotifyPropertyChanged
    {
        private BitmapSource m_bmpSource;
        private IntPtr m_bufferView = IntPtr.Zero;
        BitmapPalette m_myPalette;
        const int m_nBufferSize = 1280 * 1024 * 15;
        const int m_nFrameWidth = 1280;
        const int m_nFrameHeight = 1024;
        public UcZoomBoxViewer()
        {
            InitializeComponent();

            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            m_myPalette = new BitmapPalette(colors);
        }
        protected override void OnRender(DrawingContext dc)
        {
            //base.OnRender(dc);

            if (BufferView == IntPtr.Zero)
                return;

            //Bitmap Canvas = new Bitmap(m_nFrameWidth, m_nFrameHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            //BitmapData CanvasData = Canvas.LockBits(new System.Drawing.Rectangle(0, 0, m_nFrameWidth, m_nFrameHeight), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            //unsafe
            //{
             
            //    IntPtr Ptr = (IntPtr)CanvasData.Scan0.ToPointer();

            //    for (int i = 0; i < m_nFrameHeight; i++)
            //    {
                  
            //        CopyMemory((byte*)(Ptr + (i * m_nFrameWidth)), (byte*)(BufferView + (i * m_nFrameWidth)), m_nFrameWidth);
            //    }
            //    //Marshal.Copy(pBuffer + (i * nFrameWidth), nX, Ptr+(i * nViewWidth), nViewWidth);

            //    Canvas.UnlockBits(CanvasData);

            //    SetGrayscalePalette(Canvas);
            //}

            //Bitmap pImageBMP = Canvas;

            //Rect rectView = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
            //dc.DrawImage(BitmapToImageSource(pImageBMP), rectView);
            //Thread.Sleep(2);
        }
        public BitmapSource UcBmpSource
        {
            get { return m_bmpSource; }
            set
            {
                m_bmpSource = value;
            }
        }
        public IntPtr BufferView
        {
            get { return m_bufferView; }
            set 
            {
                if(Set(ref m_bufferView, value))
                {
                    if (m_bufferView == IntPtr.Zero)
                        return;

                    Bitmap Canvas = new Bitmap(m_nFrameWidth, m_nFrameHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                    BitmapData CanvasData = Canvas.LockBits(new System.Drawing.Rectangle(0, 0, m_nFrameWidth, m_nFrameHeight), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    unsafe
                    {

                        IntPtr Ptr = (IntPtr)CanvasData.Scan0.ToPointer();

                        for (int i = 0; i < m_nFrameHeight; i++)
                        {

                            CopyMemory((byte*)(Ptr + (i * m_nFrameWidth)), (byte*)(BufferView + (i * m_nFrameWidth)), m_nFrameWidth);
                        }
                        //Marshal.Copy(pBuffer + (i * nFrameWidth), nX, Ptr+(i * nViewWidth), nViewWidth);

                        Canvas.UnlockBits(CanvasData);

                        SetGrayscalePalette(Canvas);
                    }

                    Bitmap pImageBMP = Canvas;
                    imageViewer.Source = BitmapToImageSource(pImageBMP);
                }
            }
        }

        // KERNEL FUNCTIONS
        [DllImport("kernel32.dll")]
        public extern static unsafe void ZeroMemory(void* Destination, int Length);
        [DllImport("kernel32.dll")]
        public extern static unsafe void CopyMemory(void* Destination, void* Source, int Length);

        public static void SetGrayscalePalette(Bitmap Image)
        {
            ColorPalette GrayscalePalette = Image.Palette;

            for (int i = 0; i < 256; i++)
                GrayscalePalette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            Image.Palette = GrayscalePalette;
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
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T newValue = default(T), [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            field = newValue;

            OnPropertyChanged(propertyName);

            return true;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
