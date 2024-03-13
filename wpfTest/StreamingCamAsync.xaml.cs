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

namespace wpfTest
{
    /// <summary>
    /// Interaction logic for StreamingCamAsync.xaml
    /// </summary>
    public partial class StreamingCamAsync : Window
    {
        private CameraStreaming _cameraStreaming;
        private int _camIdx = 0;
        public StreamingCamAsync()
        {
            InitializeComponent();

            _cameraStreaming = new CameraStreaming(2448, 2048, ucZb: ucViewer, _camIdx, modeView: ModeView.Color);
        }

        private async void btnStreaming_Click(object sender, RoutedEventArgs e)
        {
            //InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.StartGrabUsbCam(_camIdx);
            await _cameraStreaming.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.ShowLogView(1);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Release();
        }

        private async void btnStopStreaming_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.StopGrabUsbCam(_camIdx);
            await _cameraStreaming.Stop();
        }

        private void btnTrigger_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.SingleGrabUsbCam(_camIdx);
            _cameraStreaming.SingleGrab();
        }
    }
}
