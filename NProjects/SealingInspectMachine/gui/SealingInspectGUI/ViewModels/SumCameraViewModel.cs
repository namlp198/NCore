﻿using Npc.Foundation.Base;
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
using SealingInspectGUI.Manager.SumManager;
using SealingInspectGUI.Models;

namespace SealingInspectGUI.ViewModels
{
    public class SumCameraViewModel : ViewModelBase
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcSumCameraView _sumCameraView;
        public IOManager_PLC_Wecon PLC_Wecon;
        public Lighting_Controller_CSS_PD3 LightController_PD3;

        private List<TopCamResult_MapToDataGrid_Model> m_listTopCamResult_MapToDataGrid_Cavity1 = new List<TopCamResult_MapToDataGrid_Model>();
        private List<TopCamResult_MapToDataGrid_Model> m_listTopCamResult_MapToDataGrid_Cavity2 = new List<TopCamResult_MapToDataGrid_Model>();

        // status result cavity 1
        private int m_nTopCamFrame1_Cavity1 = -1;
        private int m_nTopCamFrame2_Cavity1 = -1;
        private int m_nSideCamFrame1_Cavity1 = -1;
        private int m_nSideCamFrame2_Cavity1 = -1;
        private int m_nSideCamFrame3_Cavity1 = -1;
        private int m_nSideCamFrame4_Cavity1 = -1;
        private int m_nSideCamFrame5_Cavity1 = -1;
        private int m_nSideCamFrame6_Cavity1 = -1;
        private int m_nSideCamFrame7_Cavity1 = -1;
        private int m_nSideCamFrame8_Cavity1 = -1;
        private int m_nSideCamFrame9_Cavity1 = -1;
        private int m_nSideCamFrame10_Cavity1 = -1;

        // status result cavity 1
        private int m_nTopCamFrame1_Cavity2 = -1;
        private int m_nTopCamFrame2_Cavity2 = -1;
        private int m_nSideCamFrame1_Cavity2 = -1;
        private int m_nSideCamFrame2_Cavity2 = -1;
        private int m_nSideCamFrame3_Cavity2 = -1;
        private int m_nSideCamFrame4_Cavity2 = -1;
        private int m_nSideCamFrame5_Cavity2 = -1;
        private int m_nSideCamFrame6_Cavity2 = -1;
        private int m_nSideCamFrame7_Cavity2 = -1;
        private int m_nSideCamFrame8_Cavity2 = -1;
        private int m_nSideCamFrame9_Cavity2 = -1;
        private int m_nSideCamFrame10_Cavity2 = -1;

        private int m_nTotal_Cavity1 = 0;
        private int m_nOK_Cavity1 = 0;
        private int m_nNG_Cavity1 = 0;
        private double m_dYield_Cavity1 = 0;

        private int m_nTotal_Cavity2 = 0;
        private int m_nOK_Cavity2 = 0;
        private int m_nNG_Cavity2 = 0;
        private double m_dYield_Cavity2 = 0;
        #endregion

        #region Constructor
        public SumCameraViewModel(Dispatcher dispatcher, UcSumCameraView sumCameraView)
        {
            _dispatcher = dispatcher;
            _sumCameraView = sumCameraView;

            // cavity 1
            _sumCameraView.buffTopCam1_Frame1.CameraIndex = 0;
            _sumCameraView.buffTopCam1_Frame1.ModeView = emModeView.Color;
            _sumCameraView.buffTopCam1_Frame1.CameraName = "[Top Cam 1 - Ring]";
            _sumCameraView.buffTopCam1_Frame1.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam1_Frame1.ShowDetail += BuffTopCam1_Frame1_ShowDetail;

            _sumCameraView.buffTopCam1_Frame2.CameraIndex = 0;
            _sumCameraView.buffTopCam1_Frame2.ModeView = emModeView.Color;
            _sumCameraView.buffTopCam1_Frame2.CameraName = "[Top Cam 1 - 4Bar]";
            _sumCameraView.buffTopCam1_Frame2.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam1_Frame2.ShowDetail += BuffTopCam1_Frame2_ShowDetail;

            _sumCameraView.buffSideCam1.CameraIndex = 2;
            _sumCameraView.buffSideCam1.ModeView = emModeView.Color;
            _sumCameraView.buffSideCam1.CameraName = "[Side Cam 1]";
            _sumCameraView.buffSideCam1.ShowDetail += BuffSideCam1_ShowDetail;

            // cavity 2
            _sumCameraView.buffTopCam2_Frame1.CameraIndex = 1;
            _sumCameraView.buffTopCam2_Frame1.ModeView = emModeView.Color;
            _sumCameraView.buffTopCam2_Frame1.CameraName = "[Top Cam 2 - Ring]";
            _sumCameraView.buffTopCam2_Frame1.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam2_Frame1.ShowDetail += BuffTopCam2_Frame1_ShowDetail;

            _sumCameraView.buffTopCam2_Frame2.CameraIndex = 1;
            _sumCameraView.buffTopCam2_Frame2.ModeView = emModeView.Color;
            _sumCameraView.buffTopCam2_Frame2.CameraName = "[Top Cam 2 - 4Bar]";
            _sumCameraView.buffTopCam2_Frame2.SetParamsModeColor(Defines.FRAME_WIDTH_TOPCAM, Defines.FRAME_HEIGHT_TOPCAM);
            _sumCameraView.buffTopCam2_Frame2.ShowDetail += BuffTopCam2_Frame2_ShowDetail;

            _sumCameraView.buffSideCam2.CameraIndex = 3;
            _sumCameraView.buffSideCam2.ModeView = emModeView.Color;
            _sumCameraView.buffSideCam2.CameraName = "[Side Cam 2]";
            _sumCameraView.buffSideCam2.ShowDetail += BuffSideCam2_ShowDetail;

            this.LoadAllImageCmd = new LoadAllImageCmd();
            this.SelectSideCamFrameCmd = new SelectSideCamFrameCmd();
            this.GrabCavity1Cmd = new GrabCavity1Cmd();
            this.GrabCavity2Cmd = new GrabCavity2Cmd();
            this.GrabAllCmd = new GrabAllCmd();
            this.TestIOCmd = new TestIOCmd();

            SimulationThread.UpdateUI_SumCameraView += SimulationThread_UpdateUI_SumCameraView;
            InterfaceManager.InspectionCavity1Complete += new InterfaceManager.InspectionCavity1Complete_Handler(InspectionCavity1Complete);
            InterfaceManager.InspectionCavity2Complete += new InterfaceManager.InspectionCavity2Complete_Handler(InspectionCavity2Complete);
            InterfaceManager.InspectionTopCam1Complete += new InterfaceManager.InspectionTopCam1Complete_Handler(InspectTopCam1Completed);
            InterfaceManager.InspectionTopCam2Complete += new InterfaceManager.InspectionTopCam2Complete_Handler(InspectTopCam2Completed);

            InterfaceManager.GrabFrameSideCam1Complete += new InterfaceManager.GrabFrameSideCam1Complete_Handler(GrabFrameSideCam1Complete);
            InterfaceManager.GrabFrameSideCam2Complete += new InterfaceManager.GrabFrameSideCam2Complete_Handler(GrabFrameSideCam2Complete);
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
            ucShowDetail.buffVS.ModeView = emModeView.Color;
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
            ucShowDetail.buffVS.ModeView = emModeView.Color;
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
            ucShowDetail.buffVS.ModeView = emModeView.Color;
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
            ucShowDetail.buffVS.ModeView = emModeView.Color;
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
            ucShowDetail.buffVS.ModeView = emModeView.Color;
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
            ucShowDetail.buffVS.ModeView = emModeView.Color;
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
        private void InspectionCavity1Complete(int bSetting)
        {
            if (bSetting == 1)
                return;

            int nCoreIdx = 0;

            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);

            TopCamFrame1_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_TopCam.m_bStatusFrame1;
            UpdateResultView(SumCameraView.buffTopCam1_Frame1, TopCamFrame1_Cavity1, 0, 0, "TOP");

            // show result
            double[] arrDistResultTopCam = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]
                                           .m_sealingInspResult_TopCam.m_dArrDistResult_TopCam;
            int[] arrPosNG = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]
                                           .m_sealingInspResult_TopCam.m_nArrPosNG_TopCam;

            if (arrDistResultTopCam.Length > 0)
            {
                List<TopCamResult_MapToDataGrid_Model> lstDistResultTopCam = new List<TopCamResult_MapToDataGrid_Model>();
                for (int i = 0; i < arrDistResultTopCam.Length; i++)
                {
                    TopCamResult_MapToDataGrid_Model distRes = new TopCamResult_MapToDataGrid_Model();
                    distRes.Index = i + 1;
                    distRes.Dist_mm = arrDistResultTopCam[i].ToString("0.00");
                    distRes.DistRefer_mm = MainViewModel.Instance.DistRefer_TopCam_Ring_1 + "";
                    distRes.ToleranceMin_mm = MainViewModel.Instance.DistToleranceMin_TopCam_Ring_1 + "";
                    distRes.ToleranceMax_mm = MainViewModel.Instance.DistToleranceMax_TopCam_Ring_1 + "";
                    distRes.ColorSet = "DarkGreen";

                    lstDistResultTopCam.Add(distRes);
                }
                if (arrPosNG.Length > 0)
                {
                    for (int i = 0; i < arrPosNG.Length; i++)
                    {
                        if (arrPosNG[i] == 1)
                        {
                            lstDistResultTopCam[i].ColorSet = "IndianRed";
                        }
                    }
                }

                ListTopCamResult_MapToDataGrid_Cavity1 = lstDistResultTopCam;
            }

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
            SideCamFrame5_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                       m_sealingInspResult_SideCam.m_bStatusFrame5;
            SideCamFrame6_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                       m_sealingInspResult_SideCam.m_bStatusFrame6;
            SideCamFrame7_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                       m_sealingInspResult_SideCam.m_bStatusFrame7;
            SideCamFrame8_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                       m_sealingInspResult_SideCam.m_bStatusFrame8;
            SideCamFrame9_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                       m_sealingInspResult_SideCam.m_bStatusFrame9;
            SideCamFrame10_Cavity1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                       m_sealingInspResult_SideCam.m_bStatusFrame10;

            UpdateResultView(SumCameraView.buffSideCam1, SideCamFrame10_Cavity1, 0, 9, "SIDE");

            // check result final
            if (CheckStatusFinal(emInspectCavity.emInspectCavity_Cavity1) == 1)
            {
                lock (_lockObj)
                {
                    PLC_Wecon.IsJudgement_1_OKNG = true;
                    PLC_Wecon.IsInspect_1_Completed = true;
                }
                MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity1 = emInspectResult.InspectResult_OK;
                OK_Cavity1++;
            }
            else if (CheckStatusFinal(emInspectCavity.emInspectCavity_Cavity1) == 0)
            {
                lock (_lockObj)
                {
                    PLC_Wecon.IsJudgement_1_OKNG = false;
                    PLC_Wecon.IsInspect_1_Completed = true;
                }
                MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity1 = emInspectResult.InspectResult_NG;
                NG_Cavity1++;
            }
            else MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity1 = emInspectResult.InspectResult_UNKNOWN;

            Total_Cavity1++;
            Yield_Cavity1 = (OK_Cavity2 / Total_Cavity1) * 100;
        }
        private void InspectionCavity2Complete(int bSetting)
        {
            int nCoreIdx = 1;
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);

            TopCamFrame1_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_TopCam.m_bStatusFrame1;
            UpdateResultView(SumCameraView.buffTopCam2_Frame1, TopCamFrame1_Cavity2, 1, 0, "TOP");

            // show result
            double[] arrDistResultTopCam = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_TopCam.m_dArrDistResult_TopCam;
            int[] arrPosNG = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]
                                           .m_sealingInspResult_TopCam.m_nArrPosNG_TopCam;

            if (arrDistResultTopCam.Length > 0)
            {
                List<TopCamResult_MapToDataGrid_Model> lstDistResultTopCam = new List<TopCamResult_MapToDataGrid_Model>();
                for (int i = 0; i < arrDistResultTopCam.Length; i++)
                {
                    TopCamResult_MapToDataGrid_Model distRes = new TopCamResult_MapToDataGrid_Model();
                    distRes.Index = i + 1;
                    distRes.Dist_mm = arrDistResultTopCam[i].ToString("0.00");
                    distRes.DistRefer_mm = MainViewModel.Instance.DistRefer_TopCam_Ring_2 + "";
                    distRes.ToleranceMin_mm = MainViewModel.Instance.DistToleranceMin_TopCam_Ring_2 + "";
                    distRes.ToleranceMax_mm = MainViewModel.Instance.DistToleranceMax_TopCam_Ring_2 + "";

                    lstDistResultTopCam.Add(distRes);
                }
                if (arrPosNG.Length > 0)
                {
                    for (int i = 0; i < arrPosNG.Length; i++)
                    {
                        if (arrPosNG[i] == 1)
                        {
                            lstDistResultTopCam[i].ColorSet = "IndianRed";
                        }
                    }
                }

                ListTopCamResult_MapToDataGrid_Cavity2 = lstDistResultTopCam;
            }

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
            SideCamFrame5_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_SideCam.m_bStatusFrame5;
            SideCamFrame6_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_SideCam.m_bStatusFrame6;
            SideCamFrame7_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_SideCam.m_bStatusFrame7;
            SideCamFrame8_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_SideCam.m_bStatusFrame8;
            SideCamFrame9_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_SideCam.m_bStatusFrame9;
            SideCamFrame10_Cavity2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                      m_sealingInspResult_SideCam.m_bStatusFrame10;

            UpdateResultView(SumCameraView.buffSideCam2, SideCamFrame4_Cavity2, 1, 9, "SIDE");

            // check result final
            if (CheckStatusFinal(emInspectCavity.emInspectCavity_Cavity2) == 1)
            {
                lock (_lockObj)
                {
                    PLC_Wecon.IsJudgement_2_OKNG = true;
                    PLC_Wecon.IsInspect_2_Completed = true;
                }
                MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity2 = emInspectResult.InspectResult_OK;
                OK_Cavity2++;
            }
            else if (CheckStatusFinal(emInspectCavity.emInspectCavity_Cavity2) == 0)
            {
                lock (_lockObj)
                {
                    PLC_Wecon.IsJudgement_2_OKNG = false;
                    PLC_Wecon.IsInspect_2_Completed = true;
                }
                MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity2 = emInspectResult.InspectResult_NG;
                NG_Cavity2++;
            }
            else MainViewModel.Instance.RunVM.ResultVM.InspectionResultFinal_Cavity2 = emInspectResult.InspectResult_UNKNOWN;

            Total_Cavity2++;
            Yield_Cavity2 = (OK_Cavity2 / Total_Cavity2) * 100;
        }
        private void InspectTopCam1Completed(int bSetting)
        {
            if (bSetting == 1)
                return;

            lock (_lockObj)
            {
                PLC_Wecon.IsInspectTopCam_1_Completed = true;
            }
        }
        private void InspectTopCam2Completed(int bSetting)
        {
            if (bSetting == 1)
                return;

            lock (_lockObj)
            {
                PLC_Wecon.IsInspectTopCam_2_Completed = true;
            }
        }
        private void GrabFrameSideCam2Complete(int bSetting)
        {
            //if (bSetting == 1)
            //    return;

            //PLC_Wecon.WriteSingleCoil(0, IOManager_PLC_Wecon.GRABIMAGE_SIDECAM2_COMPLETED, true);
        }

        private void GrabFrameSideCam1Complete(int bSetting)
        {
            //if (bSetting == 1)
            //    return;

            //PLC_Wecon.WriteSingleCoil(0, IOManager_PLC_Wecon.GRABIMAGE_SIDECAM1_COMPLETED, true);
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

            if (nStatus == 1) bufferSimple.InspectResult = emInspectResult.InspectResult_OK;
            else if (nStatus == 0) bufferSimple.InspectResult = emInspectResult.InspectResult_NG;
            else bufferSimple.InspectResult = emInspectResult.InspectResult_UNKNOWN;
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
                       m_nSideCamFrame4_Cavity1 == 1 &&
                       m_nSideCamFrame5_Cavity1 == 1 &&
                       m_nSideCamFrame6_Cavity1 == 1 &&
                       m_nSideCamFrame7_Cavity1 == 1 &&
                       m_nSideCamFrame8_Cavity1 == 1 &&
                       m_nSideCamFrame9_Cavity1 == 1 &&
                       m_nSideCamFrame10_Cavity1 == 1 ) return 1;
                    else return 0;
                case emInspectCavity.emInspectCavity_Cavity2:
                    if (m_nTopCamFrame1_Cavity2 == 1 &&
                       m_nTopCamFrame2_Cavity2 == 1 &&
                       m_nSideCamFrame1_Cavity2 == 1 &&
                       m_nSideCamFrame2_Cavity2 == 1 &&
                       m_nSideCamFrame3_Cavity2 == 1 &&
                       m_nSideCamFrame4_Cavity2 == 1 &&
                       m_nSideCamFrame5_Cavity2 == 1 &&
                       m_nSideCamFrame6_Cavity2 == 1 &&
                       m_nSideCamFrame7_Cavity2 == 1 &&
                       m_nSideCamFrame8_Cavity2 == 1 &&
                       m_nSideCamFrame9_Cavity2 == 1 &&
                       m_nSideCamFrame10_Cavity2 == 1) return 1;
                    else return 0;
                default: return -1;
            }
        }
        #endregion

        #region Properties
        public UcSumCameraView SumCameraView { get { return _sumCameraView; } }

        public List<TopCamResult_MapToDataGrid_Model> ListTopCamResult_MapToDataGrid_Cavity1
        {
            get => m_listTopCamResult_MapToDataGrid_Cavity1;
            set
            {
                SetProperty(ref m_listTopCamResult_MapToDataGrid_Cavity1, value);
            }
        }
        public List<TopCamResult_MapToDataGrid_Model> ListTopCamResult_MapToDataGrid_Cavity2
        {
            get => m_listTopCamResult_MapToDataGrid_Cavity2;
            set
            {
                SetProperty(ref m_listTopCamResult_MapToDataGrid_Cavity2, value);
            }
        }

        public int Total_Cavity1
        {
            get => m_nTotal_Cavity1;
            set { SetProperty(ref m_nTotal_Cavity1, value); }
        }
        public int OK_Cavity1
        {
            get => m_nOK_Cavity1;
            set { SetProperty(ref m_nOK_Cavity1, value); }
        }
        public int NG_Cavity1
        {
            get => m_nNG_Cavity1;
            set { SetProperty(ref m_nNG_Cavity1, value); }
        }
        public double Yield_Cavity1
        {
            get => m_dYield_Cavity1;
            set { SetProperty(ref m_dYield_Cavity1, value); }
        }

        public int Total_Cavity2
        {
            get => m_nTotal_Cavity2;
            set { SetProperty(ref m_nTotal_Cavity2, value); }
        }
        public int OK_Cavity2
        {
            get => m_nOK_Cavity2;
            set { SetProperty(ref m_nOK_Cavity2, value); }
        }
        public int NG_Cavity2
        {
            get => m_nNG_Cavity2;
            set { SetProperty(ref m_nNG_Cavity2, value); }
        }
        public double Yield_Cavity2
        {
            get => m_dYield_Cavity2;
            set { SetProperty(ref m_dYield_Cavity2, value); }
        }

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
        public int SideCamFrame5_Cavity1
        {
            get => m_nSideCamFrame5_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame5_Cavity1, value)) { }
            }
        }
        public int SideCamFrame6_Cavity1
        {
            get => m_nSideCamFrame6_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame6_Cavity1, value)) { }
            }
        }
        public int SideCamFrame7_Cavity1
        {
            get => m_nSideCamFrame7_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame7_Cavity1, value)) { }
            }
        }
        public int SideCamFrame8_Cavity1
        {
            get => m_nSideCamFrame8_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame8_Cavity1, value)) { }
            }
        }
        public int SideCamFrame9_Cavity1
        {
            get => m_nSideCamFrame9_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame9_Cavity1, value)) { }
            }
        }
        public int SideCamFrame10_Cavity1
        {
            get => m_nSideCamFrame10_Cavity1;
            set
            {
                if (SetProperty(ref m_nSideCamFrame10_Cavity1, value)) { }
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
        public int SideCamFrame5_Cavity2
        {
            get => m_nSideCamFrame5_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame5_Cavity2, value)) { }
            }
        }
        public int SideCamFrame6_Cavity2
        {
            get => m_nSideCamFrame6_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame6_Cavity2, value)) { }
            }
        }
        public int SideCamFrame7_Cavity2
        {
            get => m_nSideCamFrame7_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame7_Cavity2, value)) { }
            }
        }
        public int SideCamFrame8_Cavity2
        {
            get => m_nSideCamFrame8_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame8_Cavity2, value)) { }
            }
        }
        public int SideCamFrame9_Cavity2
        {
            get => m_nSideCamFrame9_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame9_Cavity2, value)) { }
            }
        }
        public int SideCamFrame10_Cavity2
        {
            get => m_nSideCamFrame10_Cavity2;
            set
            {
                if (SetProperty(ref m_nSideCamFrame10_Cavity2, value)) { }
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
