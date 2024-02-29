using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpfTest
{
    /// <summary>
    /// Interaction logic for TestUcZoomBoxViewer.xaml
    /// </summary>
    public partial class TestUcZoomBoxViewer : Window
    {
        private int nBuff = 0;
        private int m_nCamIdx = 0;
        Timer m_time;
        public TestUcZoomBoxViewer()
        {
            InitializeComponent();

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
            
            m_time = new Timer();
            m_time.Interval = 100;
            m_time.Elapsed += M_time_Elapsed;

        }

        private void M_time_Elapsed(object sender, ElapsedEventArgs e)
        {
            ucZBViewer.BufferView = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetBaslerCamBufferImage_New(m_nCamIdx);
            //ucZBViewer.InvalidateVisual();
        }

        private void SimulationThread_UpdateUI(int nBuff)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ucZBViewer.BufferView = InterfaceManager.Instance.m_imageProcessorManager.m_imageProcessor.GetBufferImage(nBuff, 0);
                ucZBViewer.InvalidateVisual();
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //if (ofd.ShowDialog() == true)
            //{
            //    ucZBViewer.UcBmpSource = new BitmapImage(new Uri(ofd.FileName));
            //    ucZBViewer.InvalidateVisual();
            //}

            InterfaceManager.Instance.m_simulationThread.LoadImage(nBuff);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //InterfaceManager.Instance.m_imageProcessorManager.Initialize();
        }

        private void btnLive_Click(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Initialize();
            m_time.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            InterfaceManager.Instance.m_imageProcessorManager.Release();
        }
    }
}
