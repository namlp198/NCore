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
using System.Windows.Shapes;
using NCore.Wpf.UcZoomBoxViewer;

namespace StreamingMultiCam
{
    /// <summary>
    /// Interaction logic for Streaming12Camera.xaml
    /// </summary>
    public partial class Streaming12Camera : Window
    {
        CameraStreaming _cameraStreaming1 = null;
        CameraStreaming _cameraStreaming2 = null;
        CameraStreaming _cameraStreaming3 = null;
        CameraStreaming _cameraStreaming4 = null;
        CameraStreaming _cameraStreaming5 = null;
        CameraStreaming _cameraStreaming6 = null;
        CameraStreaming _cameraStreaming7 = null;
        CameraStreaming _cameraStreaming8 = null;
        CameraStreaming _cameraStreaming9 = null;
        CameraStreaming _cameraStreaming10 = null;
        CameraStreaming _cameraStreaming11 = null;
        CameraStreaming _cameraStreaming12 = null;
        public Streaming12Camera()
        {
            InitializeComponent();

            // view for 4 cam Hik
            ucZoomViewer1.CameraIndex = 0;
            ucZoomViewer1.StartCam += UcZoomViewer1_StartCam;
            ucZoomViewer1.StopCam += UcZoomViewer1_StopCam;

            ucZoomViewer2.CameraIndex = 1;
            ucZoomViewer2.StartCam += UcZoomViewer2_StartCam;
            ucZoomViewer2.StopCam += UcZoomViewer2_StopCam;

            ucZoomViewer3.CameraIndex = 2;
            ucZoomViewer3.StartCam += UcZoomViewer3_StartCam;
            ucZoomViewer3.StopCam += UcZoomViewer3_StopCam;

            ucZoomViewer4.CameraIndex = 3;
            ucZoomViewer4.StartCam += UcZoomViewer4_StartCam;
            ucZoomViewer4.StopCam += UcZoomViewer4_StopCam;

            // view for 8 cam iRayple
            ucZoomViewer5.CameraIndex = 0;
            ucZoomViewer5.StartCam += UcZoomViewer5_StartCam;
            ucZoomViewer5.StopCam += UcZoomViewer5_StopCam;

            ucZoomViewer6.CameraIndex = 1;
            ucZoomViewer6.StartCam += UcZoomViewer6_StartCam;
            ucZoomViewer6.StopCam += UcZoomViewer6_StopCam;

            ucZoomViewer7.CameraIndex = 2;
            ucZoomViewer7.StartCam += UcZoomViewer7_StartCam;
            ucZoomViewer7.StopCam += UcZoomViewer7_StopCam;

            ucZoomViewer8.CameraIndex = 3;
            ucZoomViewer8.StartCam += UcZoomViewer8_StartCam;
            ucZoomViewer8.StopCam += UcZoomViewer8_StopCam;

            ucZoomViewer9.CameraIndex = 4;
            ucZoomViewer9.StartCam += UcZoomViewer9_StartCam;
            ucZoomViewer9.StopCam += UcZoomViewer9_StopCam;

            ucZoomViewer10.CameraIndex = 5;
            ucZoomViewer10.StartCam += UcZoomViewer10_StartCam;
            ucZoomViewer10.StopCam += UcZoomViewer10_StopCam;

            ucZoomViewer11.CameraIndex = 6;
            ucZoomViewer11.StartCam += UcZoomViewer11_StartCam;
            ucZoomViewer11.StopCam += UcZoomViewer11_StopCam;

            ucZoomViewer12.CameraIndex = 7;
            ucZoomViewer12.StartCam += UcZoomViewer12_StartCam;
            ucZoomViewer12.StopCam += UcZoomViewer12_StopCam;

            // streaming for 4 cam hik usb3
            _cameraStreaming1 = new CameraStreaming(2448, 2048, ucZoomViewer1, ucZoomViewer1.CameraIndex, ModeView.Color);
            _cameraStreaming2 = new CameraStreaming(2448, 2048, ucZoomViewer2, ucZoomViewer2.CameraIndex, ModeView.Color);
            _cameraStreaming3 = new CameraStreaming(2448, 2048, ucZoomViewer3, ucZoomViewer3.CameraIndex, ModeView.Color);
            _cameraStreaming4 = new CameraStreaming(2448, 2048, ucZoomViewer4, ucZoomViewer4.CameraIndex, ModeView.Color);

            // streaming for 8 cam iRayple gige
            _cameraStreaming5 = new CameraStreaming(2448, 2048, ucZoomViewer5, ucZoomViewer5.CameraIndex, ModeView.Color);
            _cameraStreaming6 = new CameraStreaming(2448, 2048, ucZoomViewer6, ucZoomViewer6.CameraIndex, ModeView.Color);
            _cameraStreaming7 = new CameraStreaming(2448, 2048, ucZoomViewer7, ucZoomViewer7.CameraIndex, ModeView.Color);
            _cameraStreaming8 = new CameraStreaming(2448, 2048, ucZoomViewer8, ucZoomViewer8.CameraIndex, ModeView.Color);
            _cameraStreaming9 = new CameraStreaming(2448, 2048, ucZoomViewer9, ucZoomViewer9.CameraIndex, ModeView.Color);
            _cameraStreaming10 = new CameraStreaming(2448, 2048, ucZoomViewer10, ucZoomViewer10.CameraIndex, ModeView.Color);
            _cameraStreaming11 = new CameraStreaming(2448, 2048, ucZoomViewer11, ucZoomViewer11.CameraIndex, ModeView.Color);
            _cameraStreaming12 = new CameraStreaming(2448, 2048, ucZoomViewer12, ucZoomViewer12.CameraIndex, ModeView.Color);
        }

        private async void UcZoomViewer12_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming12.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer12_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming12.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer11_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming12.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer11_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming11.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer10_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming10.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer10_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming10.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer9_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming9.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer9_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming9.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer8_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming8.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer8_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming8.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer7_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming7.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer7_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming7.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer6_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming6.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer6_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming6.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer5_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming5.Stop(CameraType.iRayple);
        }

        private async void UcZoomViewer5_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming5.Start(CameraType.iRayple);
        }

        private async void UcZoomViewer4_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming4.Stop(CameraType.Hik);
        }

        private async void UcZoomViewer4_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming4.Start(CameraType.Hik);
        }

        private async void UcZoomViewer3_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming3.Stop(CameraType.Hik);
        }

        private async void UcZoomViewer3_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming3.Start(CameraType.Hik);
        }

        private async void UcZoomViewer2_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming2.Stop(CameraType.Hik);
        }

        private async void UcZoomViewer2_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming2.Start(CameraType.Hik);
        }

        private async void UcZoomViewer1_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming1.Stop(CameraType.Hik);
        }

        private async void UcZoomViewer1_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming1.Start(CameraType.Hik);
        }


        private void btnInitialize_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_streamingMultiCamProcessorManager.Initialize();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_streamingMultiCamProcessorManager.Release();
        }
    }
}
