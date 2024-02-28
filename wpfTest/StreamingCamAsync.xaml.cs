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
using wfTestTaskProcessor;

namespace wpfTest
{
    /// <summary>
    /// Interaction logic for StreamingCamAsync.xaml
    /// </summary>
    public partial class StreamingCamAsync : Window
    {
        private CameraStreaming _cameraStreaming;
        public StreamingCamAsync()
        {
            InitializeComponent();

            _cameraStreaming = new CameraStreaming(1280, 1024, ucZb: ucViewer);
        }

        private async void btnStreaming_Click(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Release();
        }

        private async void btnStopStreaming_Click(object sender, RoutedEventArgs e)
        {
            await _cameraStreaming.Stop();
        }
    }
}
