using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace WpfTestImageExt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IntPtr _bufferImg = IntPtr.Zero;
        private int _frameWidth = 640;
        private int _frameHeight = 480;

        private BitmapPalette _palette;
        private int _bufferSize;
        private int _stride;
        private const int _resolutionX = 96;
        private const int _resolutionY = 96;
        public MainWindow()
        {
            InitializeComponent();

            scrollViewerExt.ImageExt = imageExt;
            scrollViewerExt.Grid = gridMain;
            imageExt.SelectedROI += ImageExt_GetROI;


            // Try creating a new image with a custom palette.
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            colors.Add(System.Windows.Media.Colors.Red);
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);

            _palette = new BitmapPalette(colors);
            _bufferSize = _frameWidth * _frameHeight * 3;
            _stride = _frameWidth * 3;
        }

        private void ImageExt_GetROI(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(sender.GetType().ToString());
        }

        private void btnnLoadImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                imageExt.Source = new BitmapImage(new Uri(ofd.FileName));
            }
        }

        private void btnActiveROI_Click(object sender, RoutedEventArgs e)
        {
            //imageExt.EnableSelectRoiTool = true;
            //imageExt.IsSelectingRoi = true;
            //imageExt.EnableSelectRect = true;
            //imageExt.EnableRotate = true;
            //imageExt.Drag = true;

            //imageExt.EnableLocatorTool = true;
            //imageExt.InvalidateVisual();
        }

        private void btnSingleGrab_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.SingleGrabUsbCam(0);
           _bufferImg = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetUsbCamBufferImage(0);

            BitmapSource bmpSrc = BitmapSource.Create(_frameWidth, _frameHeight, _resolutionX, _resolutionY, PixelFormats.Bgr24, _palette, _bufferImg, _bufferSize, stride: _stride);
            bmpSrc.Freeze();
            imageExt.Dispatcher.Invoke(() => imageExt.Source = bmpSrc);
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.ShowLogView(1);
        }
    }
}
