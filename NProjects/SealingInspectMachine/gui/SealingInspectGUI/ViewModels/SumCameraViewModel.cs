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
using SealingInspectGUI.Manager.Class;
using System.Net.Http.Headers;
using System.Windows;

namespace SealingInspectGUI.ViewModels
{
    public class SumCameraViewModel : ViewModelBase
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcSumCameraView _sumCameraView;
        public IOManager_PLC_Wecon PLC_Wecon_1;
        public IOManager_PLC_Wecon PLC_Wecon_2;

        // status result cavity 1
        private int m_nTopCamFrame1_Cavity1 = -1;
        private int m_nTopCamFrame2_Cavity1 = -1;
        private int m_nSideCamFrame1_Cavity1 = -1;
        private int m_nSideCamFrame2_Cavity1 = -1;
        private int m_nSideCamFrame3_Cavity1 = -1;
        private int m_nSideCamFrame4_Cavity1 = -1;

        // status result cavity 1
        private int m_nTopCamFrame1_Cavity2 = -1;
        private int m_nTopCamFrame2_Cavity2 = -1;
        private int m_nSideCamFrame1_Cavity2 = -1;
        private int m_nSideCamFrame2_Cavity2 = -1;
        private int m_nSideCamFrame3_Cavity2 = -1;
        private int m_nSideCamFrame4_Cavity2 = -1;
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
            _sumCameraView.buffTopCam1_Frame1.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam1_Frame1.ShowDetail += BuffTopCam1_Frame1_ShowDetail;

            _sumCameraView.buffTopCam1_Frame2.CameraIndex = 0;
            _sumCameraView.buffTopCam1_Frame2.ModeView = ModeView.Color;
            _sumCameraView.buffTopCam1_Frame2.CameraName = "[Top Cam 1 - 4Bar]";
            _sumCameraView.buffTopCam1_Frame2.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam1_Frame2.ShowDetail += BuffTopCam1_Frame2_ShowDetail;

            _sumCameraView.buffSideCam1.CameraIndex = 2;
            _sumCameraView.buffSideCam1.ModeView = ModeView.Color;
            _sumCameraView.buffSideCam1.CameraName = "[Side Cam 1]";
            _sumCameraView.buffSideCam1.ShowDetail += BuffSideCam1_ShowDetail;

            // cavity 2
            _sumCameraView.buffTopCam2_Frame1.CameraIndex = 1;
            _sumCameraView.buffTopCam2_Frame1.ModeView = ModeView.Color;
            _sumCameraView.buffTopCam2_Frame1.CameraName = "[Top Cam 2 - Ring]";
            _sumCameraView.buffTopCam2_Frame1.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam2_Frame1.ShowDetail += BuffTopCam2_Frame1_ShowDetail;

            _sumCameraView.buffTopCam2_Frame2.CameraIndex = 1;
            _sumCameraView.buffTopCam2_Frame2.ModeView = ModeView.Color;
            _sumCameraView.buffTopCam2_Frame2.CameraName = "[Top Cam 2 - 4Bar]";
            _sumCameraView.buffTopCam2_Frame2.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam2_Frame2.ShowDetail += BuffTopCam2_Frame2_ShowDetail;

            _sumCameraView.buffSideCam2.CameraIndex = 3;
            _sumCameraView.buffSideCam2.ModeView = ModeView.Color;
            _sumCameraView.buffSideCam2.CameraName = "[Side Cam 2]";
            _sumCameraView.buffSideCam2.ShowDetail += BuffSideCam2_ShowDetail;

            this.LoadAllImageCmd = new LoadAllImageCmd();
            this.SelectSideCamFrameCmd = new SelectSideCamFrameCmd();
            this.GrabCavity1Cmd = new GrabCavity1Cmd();
            this.GrabCavity2Cmd = new GrabCavity2Cmd();
            this.GrabAllCmd = new GrabAllCmd();
            this.TestIOCmd = new TestIOCmd();

            PLC_Wecon_1 = new IOManager_PLC_Wecon("192.168.0.10", 1);
            PLC_Wecon_2 = new IOManager_PLC_Wecon("192.168.0.11", 2);

            if (PLC_Wecon_1.ConnectPLC())
            if (PLC_Wecon_2.ConnectPLC())

            SimulationThread.UpdateUI_SumCameraView += SimulationThread_UpdateUI_SumCameraView;
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
            InterfaceManager.InspectionTopCamComplete += new InterfaceManager.InspectionTopCamComplete_Handler(InspectTopCamCompleted);
        }

        private async void SimulationThread_UpdateUI_SumCameraView()
        {
            _sumCameraView.buffTopCam1_Frame1.CameraIndex = 99;
            _sumCameraView.buffTopCam1_Frame1.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                           m_sealingInspProcessorDll.GetBufferImage_TOP(0, 0);
            await _sumCameraView.buffTopCam1_Frame1.UpdateImage();

            _sumCameraView.buffTopCam1_Frame2.CameraIndex = 99;
            _sumCameraView.buffTopCam1_Frame2.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                           m_sealingInspProcessorDll.GetBufferImage_TOP(0, 1);
            await _sumCameraView.buffTopCam1_Frame2.UpdateImage();

            _sumCameraView.buffTopCam2_Frame1.CameraIndex = 99;
            _sumCameraView.buffTopCam2_Frame1.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                           m_sealingInspProcessorDll.GetBufferImage_TOP(1, 0);
            await _sumCameraView.buffTopCam2_Frame1.UpdateImage();

            _sumCameraView.buffTopCam2_Frame2.CameraIndex = 99;
            _sumCameraView.buffTopCam2_Frame2.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                           m_sealingInspProcessorDll.GetBufferImage_TOP(1, 1);
            await _sumCameraView.buffTopCam2_Frame2.UpdateImage();

            _sumCameraView.buffSideCam1.CameraIndex = 99;
            _sumCameraView.buffSideCam1.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                     m_sealingInspProcessorDll.GetBufferImage_SIDE(0, 0);
            await _sumCameraView.buffSideCam1.UpdateImage();

            _sumCameraView.buffSideCam2.CameraIndex = 99;
            _sumCameraView.buffSideCam2.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                                     m_sealingInspProcessorDll.GetBufferImage_SIDE(1, 0);
            await _sumCameraView.buffSideCam2.UpdateImage();
        }

        private async void BuffSideCam2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffSideCam2.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffSideCam2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(Defines.FRAME_WIDTH_SIDECAM, Defines.FRAME_HEIGHT_SIDECAM);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffSideCam2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffSideCam2.BufferView;
                ucShowDetail.buffVS.InspectResult = _sumCameraView.buffSideCam2.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        private async void BuffTopCam2_Frame2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam2_Frame2.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam2_Frame2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffTopCam2_Frame2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam2_Frame2.BufferView;
                ucShowDetail.buffVS.InspectResult = _sumCameraView.buffTopCam2_Frame2.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        private async void BuffTopCam2_Frame1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam2_Frame1.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam2_Frame1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffTopCam2_Frame1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam2_Frame1.BufferView;
                ucShowDetail.buffVS.InspectResult = _sumCameraView.buffTopCam2_Frame1.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        private async void BuffSideCam1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffSideCam1.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffSideCam1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(Defines.FRAME_WIDTH_SIDECAM, Defines.FRAME_HEIGHT_SIDECAM);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffSideCam1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffSideCam1.BufferView;
                ucShowDetail.buffVS.InspectResult = _sumCameraView.buffSideCam1.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        private async void BuffTopCam1_Frame2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam1_Frame2.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam1_Frame2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffTopCam1_Frame2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam1_Frame2.BufferView;
                ucShowDetail.buffVS.InspectResult = _sumCameraView.buffTopCam1_Frame2.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        private async void BuffTopCam1_Frame1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = _sumCameraView.buffTopCam1_Frame1.CameraIndex;
            ucShowDetail.buffVS.CameraName = _sumCameraView.buffTopCam1_Frame1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;
            if (_sumCameraView.buffTopCam1_Frame1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = _sumCameraView.buffTopCam1_Frame1.BufferView;
                ucShowDetail.buffVS.InspectResult = _sumCameraView.buffTopCam1_Frame1.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        #endregion

        #region methods
        private void InspectionComplete(emInspectCavity eInspCav, int bSetting)
        {
            if (bSetting == 1)
                return;

            int nCoreIdx = 0;
            switch (eInspCav)
            {
                case emInspectCavity.emInspectCavity_Cavity1:
                    nCoreIdx = 0;
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                        GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);

                    TopCamFrame1_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame1;
                    UpdateResultView(SumCameraView.buffTopCam1_Frame1, TopCamFrame1_Cavity1, 0, 0, "TOP");

                    TopCamFrame2_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame2;
                    UpdateResultView(SumCameraView.buffTopCam1_Frame2, TopCamFrame2_Cavity1, 0, 1, "TOP");

                    SideCamFrame1_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame1;
                    //UpdateResultView(SumCameraView.buffSideCam1, SideCamFrame1_Cavity1, 0, 0, "SIDE");

                    SideCamFrame2_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame2;
                    //UpdateResultView(SumCameraView.buffSideCam1, SideCamFrame2_Cavity1, 0, 1, "SIDE");

                    SideCamFrame3_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame3;
                    //UpdateResultView(SumCameraView.buffSideCam1, SideCamFrame3_Cavity1, 0, 2, "SIDE");

                    SideCamFrame4_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame4;
                    UpdateResultView(SumCameraView.buffSideCam1, SideCamFrame4_Cavity1, 0, 3, "SIDE");

                    // check result final
                    if (CheckStatusFinal(eInspCav) == 1)
                    {
                        lock (_lockObj)
                        {
                            PLC_Wecon_1.IsJudgementOKNG = true;
                            PLC_Wecon_1.IsInspectCompleted = true;
                        }
                        MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity1 = EInspectResult.InspectResult_OK;
                    }
                    else if (CheckStatusFinal(eInspCav) == 0)
                    {
                        lock (_lockObj)
                        {
                            PLC_Wecon_1.IsJudgementOKNG = false;
                            PLC_Wecon_1.IsInspectCompleted = true;
                        }
                        MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity1 = EInspectResult.InspectResult_NG;
                    }
                    else MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity1 = EInspectResult.InspectResult_UNKNOWN;

                    break;
                case emInspectCavity.emInspectCavity_Cavity2:
                    nCoreIdx = 1;
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                        GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);

                    TopCamFrame1_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame1;
                    UpdateResultView(SumCameraView.buffTopCam2_Frame1, TopCamFrame1_Cavity2, 1, 0, "TOP");

                    TopCamFrame2_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame2;
                    UpdateResultView(SumCameraView.buffTopCam2_Frame2, TopCamFrame2_Cavity2, 1, 1, "TOP");

                    SideCamFrame1_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_SideCam.m_bStatusFrame1;
                    //UpdateResultView(SumCameraView.buffSideCam2, SideCamFrame1_Cavity2, 1, 0, "SIDE");

                    SideCamFrame2_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame2;
                    //UpdateResultView(SumCameraView.buffSideCam2, SideCamFrame2_Cavity2, 1, 1, "SIDE");

                    SideCamFrame3_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame3;
                    //UpdateResultView(SumCameraView.buffSideCam2, SideCamFrame3_Cavity2, 1, 2, "SIDE");

                    SideCamFrame4_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame4;
                    UpdateResultView(SumCameraView.buffSideCam2, SideCamFrame4_Cavity2, 1, 3, "SIDE");

                    // check result final
                    if (CheckStatusFinal(eInspCav) == 1)
                    {
                        lock (_lockObj)
                        {
                            PLC_Wecon_2.IsJudgementOKNG = true;
                            PLC_Wecon_2.IsInspectCompleted = true;
                        }
                        MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity2 = EInspectResult.InspectResult_OK;
                    }
                    else if (CheckStatusFinal(eInspCav) == 0)
                    {
                        lock (_lockObj)
                        {
                            PLC_Wecon_2.IsJudgementOKNG = false;
                            PLC_Wecon_2.IsInspectCompleted = true;
                        }
                        MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity2 = EInspectResult.InspectResult_NG;
                    }
                    else MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity2 = EInspectResult.InspectResult_UNKNOWN;

                    break;
            }

        }
        private void InspectTopCamCompleted(emInspectCavity eInspCav)
        {
            switch (eInspCav)
            {
                case emInspectCavity.emInspectCavity_Cavity1:
                    lock (_lockObj)
                    {
                        PLC_Wecon_1.IsInspectTopCamCompleted = true;
                    }
                    break;
                case emInspectCavity.emInspectCavity_Cavity2:
                    lock (_lockObj)
                    {
                        PLC_Wecon_2.IsInspectTopCamCompleted = true;
                    }
                    break;
            }
        }
        private async void UpdateResultView(BufferViewerSimple bufferSimple, int nStatus, int nBuff, int nFrame, string s)
        {
            if (string.Compare(s.ToUpper(), "TOP") == 0)
            {
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetResultBuffer_TOP(nBuff, nFrame);
            }
            else if (string.Compare(s.ToUpper(), "SIDE") == 0)
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetResultBuffer_SIDE(nBuff, nFrame);
            await bufferSimple.UpdateImage();

            if (nStatus == 1) bufferSimple.InspectResult = EInspectResult.InspectResult_OK;
            else if (nStatus == 0) bufferSimple.InspectResult = EInspectResult.InspectResult_NG;
            else bufferSimple.InspectResult = EInspectResult.InspectResult_UNKNOWN;
        }
        private int CheckStatusFinal(emInspectCavity emInspectCavity)
        {
            switch (emInspectCavity)
            {
                case emInspectCavity.emInspectCavity_Cavity1:
                    if (m_nTopCamFrame1_Cavity1 == 1 &&
                       m_nTopCamFrame2_Cavity1 == 1 &&
                       m_nSideCamFrame1_Cavity1 == 1 &&
                       m_nSideCamFrame2_Cavity1 == 1 &&
                       m_nSideCamFrame3_Cavity1 == 1 &&
                       m_nSideCamFrame4_Cavity1 == 1) return 1;
                    else return 0;
                case emInspectCavity.emInspectCavity_Cavity2:
                    if (m_nTopCamFrame1_Cavity2 == 1 &&
                       m_nTopCamFrame2_Cavity2 == 1 &&
                       m_nSideCamFrame1_Cavity2 == 1 &&
                       m_nSideCamFrame2_Cavity2 == 1 &&
                       m_nSideCamFrame3_Cavity2 == 1 &&
                       m_nSideCamFrame4_Cavity2 == 1) return 1;
                    else return 0;
                default: return -1;
            }
        }
        #endregion

        #region Properties
        public UcSumCameraView SumCameraView { get { return _sumCameraView; } }

        #region properties Cavity 1
        public int TopCamFrame1_Cavity1
        {
            get => m_nTopCamFrame1_Cavity1;
            set
            {
                if (SetProperty(ref m_nTopCamFrame1_Cavity1, value)) { }
            }
        }
        public int TopCamFrame2_Cavity1
        {
            get => m_nTopCamFrame2_Cavity1;
            set
            {
                if (SetProperty(ref m_nTopCamFrame2_Cavity1, value)) { }
            }
        }
        public int SideCamFrame1_Cavity1
        {
            get => m_nSideCamFrame1_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame1_Cavity1, value)) { }
            }
        }
        public int SideCamFrame2_Cavity1
        {
            get => m_nSideCamFrame2_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame2_Cavity1, value)) { }
            }
        }
        public int SideCamFrame3_Cavity1
        {
            get => m_nSideCamFrame3_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame3_Cavity1, value)) { }
            }
        }
        public int SideCamFrame4_Cavity1
        {
            get => m_nSideCamFrame4_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame4_Cavity1, value)) { }
            }
        }
        #endregion

        #region properties Cavity 2
        public int TopCamFrame1_Cavity2
        {
            get => m_nTopCamFrame1_Cavity2;
            set
            {
                if (SetProperty(ref m_nTopCamFrame1_Cavity2, value)) { }
            }
        }
        public int TopCamFrame2_Cavity2
        {
            get => m_nTopCamFrame2_Cavity2;
            set
            {
                if (SetProperty(ref m_nTopCamFrame2_Cavity2, value)) { }
            }
        }
        public int SideCamFrame1_Cavity2
        {
            get => m_nSideCamFrame1_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame1_Cavity2, value)) { }
            }
        }
        public int SideCamFrame2_Cavity2
        {
            get => m_nSideCamFrame2_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame2_Cavity2, value)) { }
            }
        }
        public int SideCamFrame3_Cavity2
        {
            get => m_nSideCamFrame3_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame3_Cavity2, value)) { }
            }
        }
        public int SideCamFrame4_Cavity2
        {
            get => m_nSideCamFrame4_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame4_Cavity2, value)) { }
            }
        }
        #endregion
        #endregion

        #region Commands
        public ICommand LoadAllImageCmd { get; }
        public ICommand SelectSideCamFrameCmd { get; }
        public ICommand GrabCavity1Cmd { get; }
        public ICommand GrabCavity2Cmd { get; }
        public ICommand GrabAllCmd { get; }
        public ICommand TestIOCmd { get; }
        #endregion
    }
}
