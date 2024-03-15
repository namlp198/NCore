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

        CameraStreaming _cameraStreaming5 = null;
        CameraStreaming _cameraStreaming6 = null;
        CameraStreaming _cameraStreaming7 = null;
        CameraStreaming _cameraStreaming8 = null;
        CameraStreaming _cameraStreaming9 = null;
        CameraStreaming _cameraStreaming10 = null;
        CameraStreaming _cameraStreaming11 = null;
        CameraStreaming _cameraStreaming12 = null;
        public MainWindow()
        {
            InitializeComponent();

            ucZoomBox1.CameraIndex = 0;
            ucZoomBox2.CameraIndex = 1;
            ucZoomBox3.CameraIndex = 2;
            ucZoomBox4.CameraIndex = 3;

            ucZoomBox5.CameraIndex = 0;
            ucZoomBox6.CameraIndex = 1;
            ucZoomBox7.CameraIndex = 2;
            ucZoomBox8.CameraIndex = 3;
            ucZoomBox9.CameraIndex = 4;
            ucZoomBox10.CameraIndex = 5;
            ucZoomBox11.CameraIndex = 6;
            ucZoomBox12.CameraIndex = 7;


            ucZoomBox1.StartCam += UcZoomBox1_StartCam;
            ucZoomBox1.StopCam += UcZoomBox1_StopCam;
            ucZoomBox2.StartCam += UcZoomBox2_StartCam;
            ucZoomBox2.StopCam += UcZoomBox2_StopCam;
            ucZoomBox3.StartCam += UcZoomBox3_StartCam;
            ucZoomBox3.StopCam += UcZoomBox3_StopCam;
            ucZoomBox4.StartCam += UcZoomBox4_StartCam;
            ucZoomBox4.StopCam += UcZoomBox4_StopCam;

            ucZoomBox5.StartCam += UcZoomBox5_StartCam;
            ucZoomBox5.StopCam += UcZoomBox5_StopCam;
            ucZoomBox6.StartCam += UcZoomBox6_StartCam;
            ucZoomBox6.StopCam += UcZoomBox6_StopCam;
            ucZoomBox7.StartCam += UcZoomBox7_StartCam;
            ucZoomBox7.StopCam += UcZoomBox7_StopCam;
            ucZoomBox8.StartCam += UcZoomBox8_StartCam;
            ucZoomBox8.StopCam += UcZoomBox8_StopCam;
            ucZoomBox9.StartCam += UcZoomBox9_StartCam;
            ucZoomBox9.StopCam += UcZoomBox9_StopCam;
            ucZoomBox10.StartCam += UcZoomBox10_StartCam;
            ucZoomBox10.StopCam += UcZoomBox10_StopCam;
            ucZoomBox11.StartCam += UcZoomBox11_StartCam;
            ucZoomBox11.StopCam += UcZoomBox11_StopCam;
            ucZoomBox12.StartCam += UcZoomBox12_StartCam;
            ucZoomBox12.StopCam += UcZoomBox12_StopCam;


            _cameraStreaming1 = new CameraStreaming(2448, 2048, ucZoomBox1, ucZoomBox1.CameraIndex, ModeView.Color);
            _cameraStreaming2 = new CameraStreaming(2448, 2048, ucZoomBox2, ucZoomBox2.CameraIndex, ModeView.Color);
            _cameraStreaming3 = new CameraStreaming(2448, 2048, ucZoomBox3, ucZoomBox3.CameraIndex, ModeView.Color);
            _cameraStreaming4 = new CameraStreaming(2448, 2048, ucZoomBox4, ucZoomBox4.CameraIndex, ModeView.Color);

            _cameraStreaming5 = new CameraStreaming(2448, 2048, ucZoomBox5, ucZoomBox5.CameraIndex, ModeView.Mono);
            _cameraStreaming6 = new CameraStreaming(2448, 2048, ucZoomBox6, ucZoomBox6.CameraIndex, ModeView.Mono);
            _cameraStreaming7 = new CameraStreaming(2448, 2048, ucZoomBox7, ucZoomBox7.CameraIndex, ModeView.Mono);
            _cameraStreaming8 = new CameraStreaming(2448, 2048, ucZoomBox8, ucZoomBox8.CameraIndex, ModeView.Mono);
            _cameraStreaming9 = new CameraStreaming(2448, 2048, ucZoomBox9, ucZoomBox9.CameraIndex, ModeView.Mono);
            _cameraStreaming10 = new CameraStreaming(2448, 2048, ucZoomBox10, ucZoomBox10.CameraIndex, ModeView.Mono);
            _cameraStreaming11 = new CameraStreaming(2448, 2048, ucZoomBox11, ucZoomBox11.CameraIndex, ModeView.Mono);
            _cameraStreaming12 = new CameraStreaming(2448, 2048, ucZoomBox12, ucZoomBox12.CameraIndex, ModeView.Mono);
        }

        private async void UcZoomBox12_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming12.Stop(CameraType.Hik);
        }

        private async void UcZoomBox12_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming12.Start(CameraType.Hik);
        }

        private async void UcZoomBox11_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming11.Stop(CameraType.Hik);
        }

        private async void UcZoomBox11_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming11.Start(CameraType.Hik);
        }

        private async void UcZoomBox10_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming10.Stop(CameraType.Hik);
        }

        private async void UcZoomBox10_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming10.Start(CameraType.Hik);
        }

        private async void UcZoomBox9_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming9.Stop(CameraType.Hik);
        }

        private async void UcZoomBox9_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming9.Start(CameraType.Hik);
        }

        private async void UcZoomBox8_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming8.Stop(CameraType.Hik);
        }

        private async void UcZoomBox8_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming8.Start(CameraType.Hik);
        }

        private async void UcZoomBox7_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming7.Stop(CameraType.Hik);
        }

        private async void UcZoomBox7_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming7.Start(CameraType.Hik);
        }

        private async void UcZoomBox6_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming6.Stop(CameraType.Hik);
        }

        private async void UcZoomBox6_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming6.Start(CameraType.Hik);
        }

        private async void UcZoomBox5_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming5.Stop(CameraType.Hik);
        }

        private async void UcZoomBox5_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming5.Start(CameraType.Hik);
        }

        #region Stop Grab Hik Cam USB3
        /* Stop */
        private async void UcZoomBox4_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming4.Stop(CameraType.Hik);
        }
        private async void UcZoomBox3_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming3.Stop(CameraType.Hik);
        }
        private async void UcZoomBox2_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming2.Stop(CameraType.Hik);
        }
        private async void UcZoomBox1_StopCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming1.Stop(CameraType.Hik);
        }
        #endregion

        #region Start Grab Hik Cam USB3
        /* Start */
        private async void UcZoomBox4_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming4.Start(CameraType.Hik);
        }

        private async void UcZoomBox3_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming3.Start(CameraType.Hik);
        }
        private async void UcZoomBox2_StartCam(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming2.Start(CameraType.Hik);
        }

        private async void UcZoomBox1_StartCam(object sender, RoutedEventArgs e)
        {
           await _cameraStreaming1.Start(CameraType.Hik);
        }
        #endregion

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
