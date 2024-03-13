using NCore.Wpf.UcZoomBoxViewer;
using System;
using System.Collections.Generic;
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

namespace StreamingMultiCam
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CameraStreaming _cameraStreaming1 = null;
        CameraStreaming _cameraStreaming2 = null;
        CameraStreaming _cameraStreaming3 = null;
        CameraStreaming _cameraStreaming4 = null;
        public MainWindow()
        {
            InitializeComponent();

            ucZoomBox1.CameraIndex = 0;
            ucZoomBox2.CameraIndex = 1;
            ucZoomBox3.CameraIndex = 2;
            ucZoomBox4.CameraIndex = 3;

            ucZoomBox1.StartCam += UcZoomBox1_StartCam;
            ucZoomBox1.StopCam += UcZoomBox1_StopCam;
            ucZoomBox2.StartCam += UcZoomBox2_StartCam;
            ucZoomBox2.StopCam += UcZoomBox2_StopCam;
            ucZoomBox3.StartCam += UcZoomBox3_StartCam;
            ucZoomBox3.StopCam += UcZoomBox3_StopCam;
            ucZoomBox4.StartCam += UcZoomBox4_StartCam;
            ucZoomBox4.StopCam += UcZoomBox4_StopCam;

            _cameraStreaming1 = new CameraStreaming(2448, 2048, ucZoomBox1, ucZoomBox1.CameraIndex, ModeView.Color);
            _cameraStreaming2 = new CameraStreaming(2448, 2048, ucZoomBox2, ucZoomBox2.CameraIndex, ModeView.Color);
            _cameraStreaming3 = new CameraStreaming(2448, 2048, ucZoomBox3, ucZoomBox3.CameraIndex, ModeView.Color);
            _cameraStreaming4 = new CameraStreaming(2448, 2048, ucZoomBox4, ucZoomBox4.CameraIndex, ModeView.Color);
        }

        /* Stop */
        private void UcZoomBox4_StopCam(object sender, RoutedEventArgs e)
        {
            
        }
        private void UcZoomBox3_StopCam(object sender, RoutedEventArgs e)
        {
            
        }
        private void UcZoomBox2_StopCam(object sender, RoutedEventArgs e)
        {
            
        }
        private void UcZoomBox1_StopCam(object sender, RoutedEventArgs e)
        {
            
        }

        /* Start */
        private async void UcZoomBox4_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming4.Start();
        }

        private async void UcZoomBox3_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming3.Start();
        }
        private async void UcZoomBox2_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming2.Start();
        }

        private async void UcZoomBox1_StartCam(object sender, RoutedEventArgs e)
        {
           await _cameraStreaming1.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_streamingMultiCamProcessorManager.Release();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_streamingMultiCamProcessorManager.Initialize();
        }
    }
}
