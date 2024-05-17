using Npc.Foundation.Base;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SealingInspectGUI.ViewModels
{
    public class SumCameraViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSumCameraView _sumCameraView;
        #endregion

        #region Constructor
        public SumCameraViewModel(Dispatcher dispatcher, UcSumCameraView sumCameraView)
        {
            _dispatcher = dispatcher;
            _sumCameraView = sumCameraView;

            // cavity 1
            _sumCameraView.buffTopCam1_Ring.CameraIndex = 0;
            _sumCameraView.buffTopCam1_Ring.CameraName = "[Top Cam 1 - Ring]";
            _sumCameraView.buffTopCam1_Ring.ShowDetail += BuffTopCam1_Ring_ShowDetail;

            _sumCameraView.buffTopCam1_Bar.CameraIndex = 0;
            _sumCameraView.buffTopCam1_Bar.CameraName = "[Top Cam 1 - Bar]";
            _sumCameraView.buffTopCam1_Bar.ShowDetail += BuffTopCam1_Bar_ShowDetail;

            _sumCameraView.buffSideCam1.CameraIndex = 1;
            _sumCameraView.buffSideCam1.CameraName = "[Side Cam 1]";
            _sumCameraView.buffSideCam1.ShowDetail += BuffSideCam1_ShowDetail;

            // cavity 2
            _sumCameraView.buffTopCam2_Ring.CameraIndex = 2;
            _sumCameraView.buffTopCam2_Ring.CameraName = "[Top Cam 2 - Ring]";
            _sumCameraView.buffTopCam2_Ring.ShowDetail += BuffTopCam2_Ring_ShowDetail;

            _sumCameraView.buffTopCam2_Bar.CameraIndex = 2;
            _sumCameraView.buffTopCam2_Bar.CameraName = "[Top Cam 2 - Bar]";
            _sumCameraView.buffTopCam2_Bar.ShowDetail += BuffTopCam2_Bar_ShowDetail;

            _sumCameraView.buffSideCam2.CameraIndex = 3;
            _sumCameraView.buffSideCam2.CameraName = "[Side Cam 2]";
            _sumCameraView.buffSideCam2.ShowDetail += BuffSideCam2_ShowDetail;
        }

        private void BuffSideCam2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffSideCam2.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffSideCam2.CameraName;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
        }

        private void BuffTopCam2_Bar_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam2_Bar.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam2_Bar.CameraName;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
        }

        private void BuffTopCam2_Ring_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam2_Ring.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam2_Ring.CameraName;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
        }

        private void BuffSideCam1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffSideCam1.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffSideCam1.CameraName;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
        }

        private void BuffTopCam1_Bar_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam1_Bar.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam1_Bar.CameraName;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
        }

        private void BuffTopCam1_Ring_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam1_Ring.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam1_Ring.CameraName;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
        }
        #endregion

        #region Properties
        public UcSumCameraView SumCameraView { get { return _sumCameraView; } }
        #endregion
    }
}
