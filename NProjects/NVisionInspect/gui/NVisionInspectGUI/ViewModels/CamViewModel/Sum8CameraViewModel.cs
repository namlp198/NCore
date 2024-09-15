using System;
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
    public class Sum8CameraViewModel : ViewModelBase, ISumCamVM
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcSum8CameraView m_ucSum8CameraView;
        private int m_nIndex = 1;
        #endregion

        #region Constructor
        public Sum8CameraViewModel(Dispatcher dispatcher, UcSum8CameraView sum8CameraView)
        {
            _dispatcher = dispatcher;
            m_ucSum8CameraView = sum8CameraView;

            int nWidthCam1 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameWidth;
            int nHeightCam1 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[0].m_nFrameHeight;
            m_ucSum8CameraView.buffCam1.CameraIndex = 0;
            m_ucSum8CameraView.buffCam1.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam1.CameraName = "[Cam 1]";
            m_ucSum8CameraView.buffCam1.SetParamsModeColor(nWidthCam1, nHeightCam1);
            m_ucSum8CameraView.buffCam1.ShowDetail += BuffCam1_ShowDetail;

            int nWidthCam2 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameWidth;
            int nHeightCam2 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameHeight;
            m_ucSum8CameraView.buffCam2.CameraIndex = 1;
            m_ucSum8CameraView.buffCam2.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam2.CameraName = "[Cam 2]";
            m_ucSum8CameraView.buffCam2.SetParamsModeColor(nWidthCam2, nHeightCam2);
            m_ucSum8CameraView.buffCam2.ShowDetail += BuffCam2_ShowDetail;

            int nWidthCam3 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameWidth;
            int nHeightCam3 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameHeight;
            m_ucSum8CameraView.buffCam3.CameraIndex = 2;
            m_ucSum8CameraView.buffCam3.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam3.CameraName = "[Cam 3]";
            m_ucSum8CameraView.buffCam3.SetParamsModeColor(nWidthCam3, nHeightCam3);
            m_ucSum8CameraView.buffCam3.ShowDetail += BuffCam3_ShowDetail;

            int nWidthCam4 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[3].m_nFrameWidth;
            int nHeightCam4 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[3].m_nFrameHeight;
            m_ucSum8CameraView.buffCam4.CameraIndex = 3;
            m_ucSum8CameraView.buffCam4.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam4.CameraName = "[Cam 4]";
            m_ucSum8CameraView.buffCam4.SetParamsModeColor(nWidthCam4, nHeightCam4);
            m_ucSum8CameraView.buffCam4.ShowDetail += BuffCam4_ShowDetail;

            int nWidthCam5 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[4].m_nFrameWidth;
            int nHeightCam5 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[4].m_nFrameHeight;
            m_ucSum8CameraView.buffCam5.CameraIndex = 4;
            m_ucSum8CameraView.buffCam5.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam5.CameraName = "[Cam 5]";
            m_ucSum8CameraView.buffCam5.SetParamsModeColor(nWidthCam5, nHeightCam5);
            m_ucSum8CameraView.buffCam5.ShowDetail += BuffCam5_ShowDetail;

            int nWidthCam6 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[5].m_nFrameWidth;
            int nHeightCam6 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[5].m_nFrameHeight;
            m_ucSum8CameraView.buffCam6.CameraIndex = 5;
            m_ucSum8CameraView.buffCam6.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam6.CameraName = "[Cam 6]";
            m_ucSum8CameraView.buffCam6.SetParamsModeColor(nWidthCam6, nHeightCam6);
            m_ucSum8CameraView.buffCam6.ShowDetail += BuffCam6_ShowDetail;

            int nWidthCam7 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[6].m_nFrameWidth;
            int nHeightCam7 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[6].m_nFrameHeight;
            m_ucSum8CameraView.buffCam7.CameraIndex = 6;
            m_ucSum8CameraView.buffCam7.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam7.CameraName = "[Cam 7]";
            m_ucSum8CameraView.buffCam7.SetParamsModeColor(nWidthCam7, nHeightCam7);
            m_ucSum8CameraView.buffCam7.ShowDetail += BuffCam7_ShowDetail;

            int nWidthCam8 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[7].m_nFrameWidth;
            int nHeightCam8 = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[7].m_nFrameHeight;
            m_ucSum8CameraView.buffCam8.CameraIndex = 7;
            m_ucSum8CameraView.buffCam8.ModeView = ModeView.Color;
            m_ucSum8CameraView.buffCam8.CameraName = "[Cam 8]";
            m_ucSum8CameraView.buffCam8.SetParamsModeColor(nWidthCam8, nHeightCam8);
            m_ucSum8CameraView.buffCam8.ShowDetail += BuffCam8_ShowDetail;

            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
        }
        #endregion

        #region Properties
        public UcSum8CameraView Sum8CameraView { get { return m_ucSum8CameraView; } }
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
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam1.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam1.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam1.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam1.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam1.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam2_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[1].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam2.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam2.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam2.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam2.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam2.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam3_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[2].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam3.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam3.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam3.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam3.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam3.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam4_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[3].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[3].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam4.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam4.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam4.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam4.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam4.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam5_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[4].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[4].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam5.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam5.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam5.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam5.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam5.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam6_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[5].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[5].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam6.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam6.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam6.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam6.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam6.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam7_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[6].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[6].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam7.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam7.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam7.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam7.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam7.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }
        private async void BuffCam8_ShowDetail(object sender, System.Windows.RoutedEventArgs e)
        {
            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[7].m_nFrameWidth;
            int nHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[7].m_nFrameHeight;

            UcShowDetail ucShowDetail = new UcShowDetail();
            ucShowDetail.buffVS.CameraIndex = m_ucSum8CameraView.buffCam8.CameraIndex;
            ucShowDetail.buffVS.CameraName = m_ucSum8CameraView.buffCam8.CameraName;
            ucShowDetail.buffVS.ModeView = ModeView.Color;
            ucShowDetail.buffVS.SetParamsModeColor(nWidth, nHeight);
            MainViewModel.Instance.RunVM.RunView.contentCamView.Content = ucShowDetail;

            if (m_ucSum8CameraView.buffCam8.BufferView != IntPtr.Zero)
            {
                ucShowDetail.buffVS.BufferView = m_ucSum8CameraView.buffCam8.BufferView;
                ucShowDetail.buffVS.InspectResult = m_ucSum8CameraView.buffCam8.InspectResult;
                await ucShowDetail.buffVS.UpdateImage();
            }
        }

        #endregion
    }
}
