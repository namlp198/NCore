using DinoVisionGUI.ViewModels;
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
using System.Windows.Shapes;

namespace DinoVisionGUI.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        CameraStreamingController m_CameraStreaming;
        public MainView()
        {
            InitializeComponent();

            ucZoomBoxViewer.IsVisibleRecipeButton = false;
            ucZoomBoxViewer.UcSingleGrab += UcZoomBoxViewer_UcSingleGrab;
            ucZoomBoxViewer.UcContinuousGrab += UcZoomBoxViewer_UcContinuousGrab;
            ucZoomBoxViewer.UcLoadImage += UcZoomBoxViewer_UcLoadImage;
            ucZoomBoxViewer.MachineModeSelected = ucZoomBoxViewer.MachineModeList[2];

            InterfaceManager.Instance.JigInspProcessorManager.Initialize();

            m_CameraStreaming = new CameraStreamingController(640, 480, ucZoomBoxViewer, 0, ModeView.Color);
        }

        private void UcZoomBoxViewer_UcLoadImage(object sender, RoutedEventArgs e)
        {
            
        }

        private void UcZoomBoxViewer_UcContinuousGrab(object sender, RoutedEventArgs e)
        {
            
        }

        private async void UcZoomBoxViewer_UcSingleGrab(object sender, RoutedEventArgs e)
        {
            await m_CameraStreaming.SingleGrab();
        }
    }
}
