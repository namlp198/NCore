﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.Views.UcViews;
using NVisionInspectGUI.Commons;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.Manager.Class;
using NVisionInspectGUI.ViewModels;
using OpenXMLParser;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System.Threading;
using NCore.Wpf.BufferViewerSimple;
using NVisionInspectGUI.Views.CamView;

namespace NVisionInspectGUI.ViewModels
{
    public class Sum3CameraViewModel : ViewModelBase, ISumCamVM
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcSum3CameraView m_ucSum3CameraView;
        private int m_nIndex = 1;
        #endregion

        #region Constructor
        public Sum3CameraViewModel(Dispatcher dispatcher, UcSum3CameraView sum3CameraView)
        {
            _dispatcher = dispatcher;
            m_ucSum3CameraView = sum3CameraView;

            int nWidthCam1 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameWidth;
            int nHeightCam1 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameHeight;
            m_ucSum3CameraView.buffCam1.CameraIndex = 0;
            m_ucSum3CameraView.buffCam1.ModeView = ModeView.Color;
            m_ucSum3CameraView.buffCam1.CameraName = "[Cam 1]";
            m_ucSum3CameraView.buffCam1.SetParamsModeColor(nWidthCam1, nHeightCam1);
            m_ucSum3CameraView.buffCam1.ShowDetail += BuffCam1_ShowDetail;

            int nWidthCam2 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameWidth;
            int nHeightCam2 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameHeight;
            m_ucSum3CameraView.buffCam2.CameraIndex = 1;
            m_ucSum3CameraView.buffCam2.ModeView = ModeView.Color;
            m_ucSum3CameraView.buffCam2.CameraName = "[Cam 2]";
            m_ucSum3CameraView.buffCam2.SetParamsModeColor(nWidthCam2, nHeightCam2);
            m_ucSum3CameraView.buffCam2.ShowDetail += BuffCam2_ShowDetail;

            int nWidthCam3 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameWidth;
            int nHeightCam3 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameHeight;
            m_ucSum3CameraView.buffCam3.CameraIndex = 2;
            m_ucSum3CameraView.buffCam3.ModeView = ModeView.Color;
            m_ucSum3CameraView.buffCam3.CameraName = "[Cam 3]";
            m_ucSum3CameraView.buffCam3.SetParamsModeColor(nWidthCam3, nHeightCam3);
            m_ucSum3CameraView.buffCam3.ShowDetail += BuffCam3_ShowDetail;

            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
        }
        #endregion

        #region Properties
        public UcSum3CameraView Sum3CameraView { get { return m_ucSum3CameraView; } }
        #endregion

        #region Methods
        private void InspectionComplete(int nCamIdx, int bSetting)
        {
            if (bSetting == 0)
            {
                int nCoreIdx = nCamIdx;
               
                switch (nCamIdx)
                {
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                }
            }
        }

        private async void BuffCam1_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum3CameraView.buffCam1.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum3CameraView.buffCam1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum3CameraView.buffCam1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum3CameraView.buffCam1.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum3CameraView.buffCam1.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum3CameraView.buffCam2.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum3CameraView.buffCam2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum3CameraView.buffCam2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum3CameraView.buffCam2.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum3CameraView.buffCam2.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam3_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum3CameraView.buffCam3.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum3CameraView.buffCam3.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum3CameraView.buffCam3.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum3CameraView.buffCam3.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum3CameraView.buffCam3.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        #endregion
    }
}
