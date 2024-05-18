using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Manager;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using NCore.Wpf.BufferViewerSimple;
using SealingInspectGUI.Commons;

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
            _sumCameraView.buffTopCam1_Frame1.CameraIndex = 0;
            _sumCameraView.buffTopCam1_Frame1.ModeView = ModeView.Color;
            _sumCameraView.buffTopCam1_Frame1.CameraName = "[Top Cam 1 - Ring]";
            _sumCameraView.buffTopCam1_Frame1.ShowDetail += BuffTopCam1_Frame1_ShowDetail;

            _sumCameraView.buffTopCam1_Frame2.CameraIndex = 0;
            _sumCameraView.buffTopCam1_Frame2.ModeView = ModeView.Color;
            _sumCameraView.buffTopCam1_Frame2.CameraName = "[Top Cam 1 - 4Bar]";
            _sumCameraView.buffTopCam1_Frame2.ShowDetail += BuffTopCam1_Frame2_ShowDetail;

            _sumCameraView.buffSideCam1.CameraIndex = 1;
            _sumCameraView.buffSideCam1.ModeView = ModeView.Color;
            _sumCameraView.buffSideCam1.CameraName = "[Side Cam 1]";
            _sumCameraView.buffSideCam1.ShowDetail += BuffSideCam1_ShowDetail;

            // cavity 2
            _sumCameraView.buffTopCam2_Frame1.CameraIndex = 2;
            _sumCameraView.buffTopCam2_Frame1.ModeView = ModeView.Color;
            _sumCameraView.buffTopCam2_Frame1.CameraName = "[Top Cam 2 - Ring]";
            _sumCameraView.buffTopCam2_Frame1.ShowDetail += BuffTopCam2_Frame1_ShowDetail;

            _sumCameraView.buffTopCam2_Frame2.CameraIndex = 2;
            _sumCameraView.buffTopCam2_Frame2.ModeView = ModeView.Color;
            _sumCameraView.buffTopCam2_Frame2.CameraName = "[Top Cam 2 - 4Bar]";
            _sumCameraView.buffTopCam2_Frame2.ShowDetail += BuffTopCam2_Frame2_ShowDetail;

            _sumCameraView.buffSideCam2.CameraIndex = 3;
            _sumCameraView.buffSideCam2.ModeView = ModeView.Color;
            _sumCameraView.buffSideCam2.CameraName = "[Side Cam 2]";
            _sumCameraView.buffSideCam2.ShowDetail += BuffSideCam2_ShowDetail;

            LoadAllImageCmd = new LoadAllImageCmd();
            SelectSideCamFrameCmd = new SelectSideCamFrameCmd();

            SimulationThread.UpdateUI_SumCameraView += SimulationThread_UpdateUI_SumCameraView;
        }

        private void SimulationThread_UpdateUI_SumCameraView(int nBuffIdx, string posCam)
        {
            
                if (string.Compare(posCam, "Top") == 0)
                {
                    switch (nBuffIdx)
                    {
                        case 0:
                            _sumCameraView.buffTopCam1_Frame1.CameraIndex = 99;
                            _sumCameraView.buffTopCam1_Frame1.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_TOP(nBuffIdx, 0);
                            break;
                        case 1:
                            _sumCameraView.buffTopCam1_Frame2.CameraIndex = 99;
                            _sumCameraView.buffTopCam1_Frame2.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_TOP(nBuffIdx, 0);
                            break;
                        case 2:
                            _sumCameraView.buffTopCam2_Frame1.CameraIndex = 99;
                            _sumCameraView.buffTopCam2_Frame1.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_TOP(nBuffIdx, 0);
                            break;
                        case 3:
                            _sumCameraView.buffTopCam2_Frame2.CameraIndex = 99;
                            _sumCameraView.buffTopCam2_Frame2.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_TOP(nBuffIdx, 0);
                            break;
                    }
                }
                else if (string.Compare(posCam, "Side") == 0)
                {
                    switch(nBuffIdx)
                    {
                        case 0:
                            _sumCameraView.buffSideCam1.CameraIndex = 99;
                            _sumCameraView.buffSideCam1.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_SIDE(nBuffIdx, 0);
                            break;
                        case Defines.MAX_IMAGE_BUFFER_SIDE / 2:
                            _sumCameraView.buffSideCam2.CameraIndex = 99;
                            _sumCameraView.buffSideCam2.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_SIDE(nBuffIdx, 0);
                            break;
                    }
                }
            
        }

        private void BuffSideCam2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffSideCam2.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffSideCam2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffSideCam2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffSideCam2.BufferView;
            }
        }

        private void BuffTopCam2_Frame2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam2_Frame2.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam2_Frame2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffTopCam2_Frame2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam2_Frame2.BufferView;
            }
        }

        private void BuffTopCam2_Frame1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam2_Frame1.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam2_Frame1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffTopCam2_Frame1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam2_Frame1.BufferView;
            }
        }

        private void BuffSideCam1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffSideCam1.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffSideCam1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffSideCam1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffSideCam1.BufferView;
            }
        }

        private void BuffTopCam1_Frame2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam1_Frame2.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam1_Frame2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffTopCam1_Frame2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam1_Frame2.BufferView;
            }
        }

        private void BuffTopCam1_Frame1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam1_Frame1.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam1_Frame1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if(_sumCameraView.buffTopCam1_Frame1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam1_Frame1.BufferView;
            }
        }
        #endregion

        #region Properties
        public UcSumCameraView SumCameraView { get { return _sumCameraView; } }
        #endregion

        #region Commands
        public ICommand LoadAllImageCmd { get; }
        public ICommand SelectSideCamFrameCmd { get; }
        #endregion
    }
}
