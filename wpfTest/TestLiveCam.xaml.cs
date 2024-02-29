using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpfTest
{
    /// <summary>
    /// Interaction logic for TestLiveCam.xaml
    /// </summary>
    public partial class TestLiveCam : Window
    {
        Timer m_time;
        private int m_nCamIdx = 0;
        IntPtr m_bufferView = IntPtr.Zero;
   
        const int m_nBufferSize = 1280 * 1024 * 15;
        const int m_nFrameWidth = 1280;
        const int m_nFrameHeight = 1024;
        public TestLiveCam()
        {
            InitializeComponent();

            m_time = new Timer();
            m_time.Interval = 50;
            m_time.Elapsed += M_time_Elapsed;

        }

        private void M_time_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_bufferView = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetBaslerCamBufferImage_New(m_nCamIdx);

            if (m_bufferView == IntPtr.Zero)
                return;

            Bitmap Canvas = new Bitmap(m_nFrameWidth, m_nFrameHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            BitmapData CanvasData = Canvas.LockBits(new System.Drawing.Rectangle(0, 0, m_nFrameWidth, m_nFrameHeight), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            unsafe
            {

                IntPtr Ptr = (IntPtr)CanvasData.Scan0.ToPointer();

                for (int i = 0; i < m_nFrameHeight; i++)
                {

                    CopyMemory((byte*)(Ptr + (i * m_nFrameWidth)), (byte*)(m_bufferView + (i * m_nFrameWidth)), m_nFrameWidth);
                }
                //Marshal.Copy(pBuffer + (i * nFrameWidth), nX, Ptr+(i * nViewWidth), nViewWidth);

                Canvas.UnlockBits(CanvasData);

                SetGrayscalePalette(Canvas);
            }

            Bitmap pImageBMP = Canvas;
            //picView.Image = pImageBMP;
            //BitmapSource bmpSrc = BitmapToImageSource(pImageBMP);
            ImageSource bmpSrc = ImageSourceForBitmap(pImageBMP);
            bmpSrc.Freeze();
            imgView.Dispatcher.Invoke(() => imgView.Source = bmpSrc );
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.StartGrabBaslerCam_New(m_nCamIdx);
            m_time.Start();
        }

        protected override void OnRender(DrawingContext dc)
        {

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

        private void Window_Closed(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Release();
        }
    }
}
