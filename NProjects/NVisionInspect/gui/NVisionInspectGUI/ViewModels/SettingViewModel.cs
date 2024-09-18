using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.Command.Cmd;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.Models.Recipe;
using NVisionInspectGUI.Views.UcViews;
using Kis.Toolkit;
using NVisionInspectGUI.Commons;
using System.Xml;
using System.ComponentModel;
using System.Reflection;
using NVisionInspectGUI.Manager.Class;
using DocumentFormat.OpenXml.Bibliography;
using LSIS.Driver.Core.DataTypes;
using System.Threading;
using System.Windows;
using NCore.Wpf.BufferViewerSettingPRO;
using Npc.Foundation.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace NVisionInspectGUI.ViewModels
{
    public enum EnImageSource
    {
        [Description("Image")]
        FromToImage,
        [Description("Camera")]
        FromToCamera
    }
    public class SettingViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSettingView m_ucSettingView;
        private XmlManagement m_xmlManagement = new XmlManagement();
        private CameraStreamingController m_cameraStreamingController = null;
        private CNVisionInspectRecipe_PropertyGrid m_NVisionInspectRecipe_PropertyGrid = new CNVisionInspectRecipe_PropertyGrid();
        private CNVisionInspectSystemSetting_PropertyGrid m_NVisionInspectSystemSetting_PropertyGrid = new CNVisionInspectSystemSetting_PropertyGrid();

        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam1Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam2Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam3Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam4Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam5Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam6Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam7Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private CNVisionInspectCameraSetting_PropertyGrid m_NVICam8Setting_PropGrid = new CNVisionInspectCameraSetting_PropertyGrid();

        private Plc_Delta_Model m_plcDeltaModel = new Plc_Delta_Model();

        private int m_nROIIdx = -1;
        private int m_nCameraCount = 0;

        private List<string> m_cameraLst = new List<string>();
        private List<string> m_lstImageSource = new List<string>();
        private List<string> m_lstROI = new List<string>();
        private List<int> m_lstNumberOfCamBrand = new List<int>();

        private string m_strDisplayImagePath_Live = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private string m_strDisplayImagePath_StartAcq = "/NpcCore.Wpf;component/Resources/Images/btn_start_50.png";
        private string m_strCameraSelected = string.Empty;
        private string m_strROISelected = string.Empty;

        private bool m_bStreamming = false;
        private bool m_bStartedAcq = false;
        private bool m_bSelectedCamera = false;

        private EnImageSource m_fromImageSource = EnImageSource.FromToCamera;
        private emCameraBrand m_cameraBrandSelected = emCameraBrand.CameraBrand_Hik;

        #endregion

        #region Constructor
        public SettingViewModel(Dispatcher dispatcher, UcSettingView settingView)
        {
            _dispatcher = dispatcher;
            m_ucSettingView = settingView;

            m_ucSettingView.buffSettingPRO.CameraIndex = 99;
            m_ucSettingView.buffSettingPRO.ModeView = NCore.Wpf.BufferViewerSettingPRO.EnModeView.Color;
            m_ucSettingView.buffSettingPRO.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
            InterfaceManager.LocatorTrainComplete += new InterfaceManager.LocatorTrainComplete_Handler(LocatorTrainComplete);
            InterfaceManager.AlarmEvent += new InterfaceManager.Alarm_Handler(AlarmHandlerFunc);

            this.SaveRecipeCmd = new SaveRecipeCmd();
            this.SaveCamSettingCmd = new SaveCamSettingCmd();
            this.SaveSysSettingCmd = new SaveSysSettingCmd();
            this.SavePlcSettingCmd = new SavePlcSettingCmd();
            this.SaveLightSettingCmd = new SaveLightSettingCmd();
            this.SaveImageCmd = new SaveImageCmd();
            this.SelectROICmd = new SelectROICmd();
            this.SingleGrabCmd = new SingleGrabCmd();
            this.StartAcquisitionCmd = new StartAcquisitionCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.LoadImageCmd = new LoadImageCmd();
            this.LocateCmd = new LocateCmd();
            this.InspectCmd = new InspectCmd();
            this.ReadCodeCmd = new ReadCodeCmd();
            this.NextPrevCmd = new NextPrevImageCmd();

            m_xmlManagement.Load(Defines.StartupProgPath + "\\VisionSettings\\Settings\\PlcSettings.config");

            m_lstImageSource = EnumUtil.GetEnumDescriptionToListString<EnImageSource>();
            SettingView.cbbImageSource.SelectionChanged += CbbImageSource_SelectionChanged;
            SettingView.cbbImageSource.SelectedIndex = 0;

            #region IMPLEMENT EVENTS SETTING

            SettingView.buffSettingPRO.SelectCameraChanged += BuffSettingPRO_SelectCameraChanged;
            SettingView.buffSettingPRO.SelectFrameChanged += BuffSettingPRO_SelectFrameChanged;
            SettingView.buffSettingPRO.SelectTriggerModeChanged += BuffSettingPRO_SelectTriggerModeChanged;
            SettingView.buffSettingPRO.SelectTriggerSourceChanged += BuffSettingPRO_SelectTriggerSourceChanged;
            SettingView.buffSettingPRO.SetExposureTime += BuffSettingPRO_SetExposureTime;
            SettingView.buffSettingPRO.SetAnalogGain += BuffSettingPRO_SetAnalogGain;
            SettingView.buffSettingPRO.TrainLocator += BuffSettingPRO_TrainLocator;
            SettingView.buffSettingPRO.ROISelectionDone += BuffSettingPRO_ROISelectionDone;

            #endregion

            m_cameraStreamingController = new CameraStreamingController(SettingView.buffSettingPRO);
        }

        private void CbbImageSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            if (string.Compare(cbb.SelectedItem.ToString(), "Image") == 0)
            {
                FromImageSource = EnImageSource.FromToImage;
            }
            else if (string.Compare(cbb.SelectedItem.ToString(), "Camera") == 0)
            {
                FromImageSource = EnImageSource.FromToCamera;
            }
        }
        private void BuffSettingPRO_SelectCameraChanged(object sender, RoutedEventArgs e)
        {
            if (SettingView.buffSettingPRO.CameraName == null)
                return;

            int nCamIdx = SettingView.buffSettingPRO.CameraIndex;

            if (nCamIdx == -1 || nCamIdx == 99)
            {
                IsSelectedCamera = false;
                return;
            }

            IsSelectedCamera = true;
            int nNumberOfROI = 0;

            switch (nCamIdx)
            {
                case 0:
                    nNumberOfROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nNumberOfROI;
                    break;
                case 1:
                    nNumberOfROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nNumberOfROI;
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
            }

            List<string> lstROI = new List<string>();
            for (int i = 0; i < nNumberOfROI; i++)
            {
                lstROI.Add("ROI " + (i + 1));
            }

            ROIList = lstROI;

            // fine camera brand selecting
            string strCamSelected = SettingView.buffSettingPRO.CameraSelected;
            int nPos = strCamSelected.IndexOf("-") + 2;
            int nLength = strCamSelected.Length - nPos;

            string strCamBrand = strCamSelected.Substring(nPos, nLength);

            if (strCamBrand.IsNullOrEmpty())
                return;

            if(string.Compare(strCamBrand, "Hik") == 0)
            {
                CameraBrandSelected = emCameraBrand.CameraBrand_Hik;
            }
            else if(string.Compare(strCamBrand, "Basler") == 0)
            {
                CameraBrandSelected = emCameraBrand.CameraBrand_Basler;
            }
            else if (string.Compare(strCamBrand, "Jai") == 0)
            {
                CameraBrandSelected = emCameraBrand.CameraBrand_Jai;
            }

            int nWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameWidth;
            int nHeight= InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameHeight;
            SettingView.buffSettingPRO.SetParamsModeColor(nWidth, nHeight);
        }
        private void BuffSettingPRO_SelectFrameChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void BuffSettingPRO_SelectTriggerSourceChanged(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetTriggerSource(
                MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex,
                (int)MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.TriggerSource);
        }
        private void BuffSettingPRO_SelectTriggerModeChanged(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetTriggerMode(
                MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex,
                (int)MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.TriggerMode);
        }
        private void BuffSettingPRO_SetAnalogGain(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetAnalogGain(
               SettingView.buffSettingPRO.CameraIndex, SettingView.buffSettingPRO.AnalogGain);
        }
        private void BuffSettingPRO_SetExposureTime(object sender, System.Windows.RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetExposureTime(
                SettingView.buffSettingPRO.CameraIndex, SettingView.buffSettingPRO.ExposureTime);
        }
        private void BuffSettingPRO_TrainLocator(object sender, RoutedEventArgs e)
        {
            int nCamIdx = SettingView.buffSettingPRO.CameraIndex;
            int nBuff = nCamIdx;
            int nFrame = 0;

            if (nCamIdx < 0)
                return;

            switch (nCamIdx)
            {
                case 0:
                    // Rect outside
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterX = (int)SettingView.buffSettingPRO.RectOutSide.X;
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterY = (int)SettingView.buffSettingPRO.RectOutSide.Y;
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Width = (int)SettingView.buffSettingPRO.RectOutSide.Width;
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Height = (int)SettingView.buffSettingPRO.RectOutSide.Height;

                    // Rect inner
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerX = (int)SettingView.buffSettingPRO.RectInSide.X;
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerY = (int)SettingView.buffSettingPRO.RectInSide.Y;
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Width = (int)SettingView.buffSettingPRO.RectInSide.Width;
                    NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Height = (int)SettingView.buffSettingPRO.RectInSide.Height;
                    break;
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
            }

            SaveRecipeCmd.Execute(nCamIdx);

            switch (FromImageSource)
            {
                case EnImageSource.FromToImage:
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LocatorToolSimulator_Train(nBuff, nFrame);
                    break;
                case EnImageSource.FromToCamera:
                    InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LocatorTool_Train(nCamIdx);
                    break;
                default:
                    break;
            }
        }
        private async void BuffSettingPRO_ROISelectionDone(object sender, RoutedEventArgs e)
        {
            int nCamIdx = SettingView.buffSettingPRO.CameraIndex;
            int nROIIdx = ROIIdx;
            int nFrom = (int)FromImageSource;

            if (nCamIdx == -1)
                return;

            if (nROIIdx == -1)
                return;

            switch (nCamIdx)
            {
                case 0:
                    switch (ROIIdx)
                    {
                        case 1:
                            if (NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1UseOffset == false)
                            {
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_X = (int)SettingView.buffSettingPRO.ROISelected.X;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Y = (int)SettingView.buffSettingPRO.ROISelected.Y;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Width = (int)SettingView.buffSettingPRO.ROISelected.Width;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_Height = (int)SettingView.buffSettingPRO.ROISelected.Height;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI1_AngleRotate = SettingView.buffSettingPRO.AngleRotate;
                            }
                            break;
                        case 2:
                            if (NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2UseOffset == false)
                            {
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_X = (int)SettingView.buffSettingPRO.ROISelected.X;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Y = (int)SettingView.buffSettingPRO.ROISelected.Y;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Width = (int)SettingView.buffSettingPRO.ROISelected.Width;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_Height = (int)SettingView.buffSettingPRO.ROISelected.Height;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI2_AngleRotate = SettingView.buffSettingPRO.AngleRotate;
                            }
                            break;
                        case 3:
                            if (NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3UseOffset == false)
                            {
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_X = (int)SettingView.buffSettingPRO.ROISelected.X;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Y = (int)SettingView.buffSettingPRO.ROISelected.Y;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Width = (int)SettingView.buffSettingPRO.ROISelected.Width;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_Height = (int)SettingView.buffSettingPRO.ROISelected.Height;
                                NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid.ROI3_AngleRotate = SettingView.buffSettingPRO.AngleRotate;
                            }
                            break;
                    }
                    break;
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
            }

            // save recipe
            SaveRecipeCmd.Execute(nCamIdx);

            // reload recipe
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadRecipe(CameraCount, ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe);
            LoadRecipe(CameraCount);

            // update PropertyGrid UI
            UpdatePropertyGridUI(nCamIdx);

            // select ROI
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SelectROI(nCamIdx, nROIIdx, nFrom);

            SettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(nCamIdx, 0);
            await SettingView.buffSettingPRO.UpdateImage();

            // show ROI Image
            SettingView.buffSettingPRO.LoadBmpImageAsync(NVICam1Setting_PropGrid.ROIsPath + "ROI_" + ROIIdx + ".png");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load System Setting
        /// </summary>
        public void LoadSystemSettings()
        {
            {
                CNVisionInspectSystemSetting_PropertyGrid cNVisionInspSystemSetting_PropertyGrid = new CNVisionInspectSystemSetting_PropertyGrid();

                cNVisionInspSystemSetting_PropertyGrid.NumberOfInspectionCamera = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_nNumberOfInspectionCamera;
                cNVisionInspSystemSetting_PropertyGrid.Simulation = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.ByPass = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bByPass == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.TestMode = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bTestMode == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.ModelName = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelName;
                cNVisionInspSystemSetting_PropertyGrid.ModelList = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelList;

                NVisionInspectSystemSettingsPropertyGrid = cNVisionInspSystemSetting_PropertyGrid;
            }
            m_lstNumberOfCamBrand.Clear();
            List<int> lstCamBrandCount = new List<int>();

            string strCameras = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sCameras;
            int nHikCamCount = 0;
            int nBaslerCamCount = 0;

            int nPosHikCam = strCameras.IndexOf("Hik") + 4;
            int nPosBaslerCam= strCameras.IndexOf("Basler") + 7;

            string sPosHikCamCount = strCameras.Substring(nPosHikCam, 1);
            string sPosBaslerCamCount = strCameras.Substring(nPosBaslerCam, 1);

            int.TryParse(sPosHikCamCount, out nHikCamCount);
            int.TryParse(sPosBaslerCamCount, out nBaslerCamCount);

            lstCamBrandCount.Add(nHikCamCount);
            lstCamBrandCount.Add(nBaslerCamCount);

            NumberOfCamBrandList = lstCamBrandCount;
        }
        /// <summary>
        /// Load Plc Settings
        /// </summary>
        public void LoadPlcSettings()
        {
            Plc_Delta_Model plc_Delta_Model_PropertyGrid = new Plc_Delta_Model();

            XmlNode settingNode = m_xmlManagement.SelectSingleNode("//PlcSettings");
            if (settingNode != null)
            {
                XmlNodeList nodeList = settingNode.ChildNodes;
                if (nodeList.Count > 0)
                {
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                plc_Delta_Model_PropertyGrid.PlcCOM = nodeList[i].InnerText;
                                if (MainViewModel.Instance.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.PlcCOM = nodeList[i].InnerText;
                                    MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.PlcCOM = nodeList[i].InnerText;
                                }
                                break;
                            case 1:
                                plc_Delta_Model_PropertyGrid.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                if (MainViewModel.Instance.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                    MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                }
                                break;
                            case 2:
                                plc_Delta_Model_PropertyGrid.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                if (MainViewModel.Instance.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                    MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                }
                                break;
                            case 3:
                                plc_Delta_Model_PropertyGrid.RegisterTriggerDelay = nodeList[i].InnerText;
                                if (MainViewModel.Instance.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterTriggerDelay = nodeList[i].InnerText;
                                    MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay = nodeList[i].InnerText;
                                }
                                break;
                            case 4:
                                plc_Delta_Model_PropertyGrid.RegisterOutput1Delay = nodeList[i].InnerText;
                                if (MainViewModel.Instance.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterOutput1Delay = nodeList[i].InnerText;
                                    MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay = nodeList[i].InnerText;
                                }
                                break;
                        }
                    }
                    PlcDeltaModelPropertyGrid = plc_Delta_Model_PropertyGrid;
                }
            }
        }
        /// <summary>
        /// Load All Cameras Setting
        /// </summary>
        public void LoadCamerasSetting(int nCamIdx)
        {
            CNVisionInspectCameraSetting_PropertyGrid cameraSetting_PropertyGrid = new CNVisionInspectCameraSetting_PropertyGrid();
            cameraSetting_PropertyGrid.IsSaveFullImage = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bSaveFullImage == 1 ? true : false;
            cameraSetting_PropertyGrid.IsSaveDefectImage = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bSaveDefectImage == 1 ? true : false;
            cameraSetting_PropertyGrid.IsShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_bShowGraphics == 1 ? true : false;
            cameraSetting_PropertyGrid.Channels = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nChannels;
            cameraSetting_PropertyGrid.FrameWidth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameWidth;
            cameraSetting_PropertyGrid.FrameHeight = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameHeight;
            cameraSetting_PropertyGrid.FrameDepth = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nFrameDepth;
            cameraSetting_PropertyGrid.MaxFrameCount = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nMaxFrameCount;
            cameraSetting_PropertyGrid.CameraName = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sCameraName;
            cameraSetting_PropertyGrid.InterfaceType = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sInterfaceType;
            cameraSetting_PropertyGrid.SensorType = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSensorType;
            cameraSetting_PropertyGrid.Manufacturer = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sManufacturer;
            cameraSetting_PropertyGrid.SerialNumber = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSerialNumber;
            cameraSetting_PropertyGrid.Model = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sModel;
            cameraSetting_PropertyGrid.FullImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sFullImagePath;
            cameraSetting_PropertyGrid.DefectImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sDefectImagePath;
            cameraSetting_PropertyGrid.TemplateImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sTemplateImagePath;
            cameraSetting_PropertyGrid.ROIsPath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sROIsPath;

            switch (nCamIdx)
            {
                case 0:
                    NVICam1Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
                case 1:
                    NVICam2Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
                case 2:
                    NVICam3Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
                case 3:
                    NVICam4Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
                case 4:
                    NVICam5Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
                case 5:
                    NVICam6Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
                case 6:
                    NVICam7Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
                case 7:
                    NVICam8Setting_PropGrid = cameraSetting_PropertyGrid;
                    break;
            }
        }
        public void SetAllParamPlcDelta()
        {
            string regTriggerDelay = MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay.Trim();
            short triggerDelay = (short)MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay;
            string regOutput1Delay = MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay.Trim();
            short output1Delay = (short)MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay;
            SetParameterPlcDelta(triggerDelay, regTriggerDelay);
            SetParameterPlcDelta(output1Delay, regOutput1Delay);
        }
        private void SetParameterPlcDelta(short value, string register)
        {
            if (register != null)
            {
                int indexReg = 0;
                int.TryParse(register.Substring(register.IndexOf("D") + 1), out indexReg);
                MainViewModel.Instance.Plc_Delta_DVP.StartAddressRegister += (uint)indexReg;

                MainViewModel.Instance.Plc_Delta_DVP.SetParameterToSingleRegister(value);

                MainViewModel.Instance.Plc_Delta_DVP.StartAddressRegister = 4096; // reset to init value
            }
        }
        public void SavePlcSettings()
        {

            XmlNode plcCOMNode = m_xmlManagement.SelectSingleNode("//PlcSettings//PLC_COM");
            plcCOMNode.InnerText = PlcDeltaModelPropertyGrid.PlcCOM;
            XmlNode triggerDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//TriggerDelay");
            triggerDelayNode.InnerText = PlcDeltaModelPropertyGrid.TriggerDelay.ToString();
            XmlNode signalNGDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//SignalNGDelay");
            signalNGDelayNode.InnerText = PlcDeltaModelPropertyGrid.SignalNGDelay.ToString();
            XmlNode registerTriggerDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//RegisterTriggerDelay");
            registerTriggerDelayNode.InnerText = PlcDeltaModelPropertyGrid.RegisterTriggerDelay;
            XmlNode registerOutput1DelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//RegisterOutput1Delay");
            registerOutput1DelayNode.InnerText = PlcDeltaModelPropertyGrid.RegisterOutput1Delay;

            if (MainViewModel.Instance.Plc_Delta_DVP != null)
            {
                MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.PlcCOM = PlcDeltaModelPropertyGrid.PlcCOM;
                MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay = PlcDeltaModelPropertyGrid.TriggerDelay;
                MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay = PlcDeltaModelPropertyGrid.SignalNGDelay;
                MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay = PlcDeltaModelPropertyGrid.RegisterTriggerDelay;
                MainViewModel.Instance.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay = PlcDeltaModelPropertyGrid.RegisterOutput1Delay;
            }

            m_xmlManagement.Save(Defines.StartupProgPath + "\\Settings\\PlcSettings.config");
            SetAllParamPlcDelta();
        }
        // Load Recipe
        public void LoadRecipe(int nCamCount)
        {
            if (nCamCount < 0)
                return;

            CNVisionInspectRecipe_PropertyGrid NVIRecipe_PropGrid = new CNVisionInspectRecipe_PropertyGrid();

            for (int nCamIdx = 0; nCamIdx < nCamCount; nCamIdx++)
            {
                switch (nCamIdx)
                {
                    case 0:
                        {
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.UseReadCode = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bUseReadCode == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.UseInkjetCharactersInspect = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bUseInkjetCharactersInspect == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.UseRotateROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bUseRotateROI == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.MaxCodeCount = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nMaxCodeCount;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.NumberOfROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nNumberOfROI;
                            // Params Template Matching
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterX = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterX;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_OuterY = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_OuterY;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Width;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_Outer_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Outer_Height;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerX = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerX;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_InnerY = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_InnerY;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Width;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateROI_Inner_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateROI_Inner_Height;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateCoordinatesX = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesX;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateCoordinatesY = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nTemplateCoordinatesY;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateAngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_dTemplateAngleRotate;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateMatchingRate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_dTemplateMatchingRate;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.TemplateShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bTemplateShowGraphics == 1 ? true : false;
                            // ROI 1
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_X;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Y;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Width;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Height;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_Offset_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Offset_X;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_Offset_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_Offset_Y;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_AngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_AngleRotate;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1UseOffset = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI1UseOffset == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1UseLocator = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI1UseLocator == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1ShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI1ShowGraphics == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Min;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_GrayThreshold_Max;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_PixelCount_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Min;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI1_PixelCount_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI1_PixelCount_Max;

                            // ROI 2
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_X;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Y;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Width;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Height;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_Offset_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Offset_X;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_Offset_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_Offset_Y;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_AngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_AngleRotate;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2UseOffset = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI2UseOffset == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2UseLocator = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI2UseLocator == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2ShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI2ShowGraphics == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Min;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_GrayThreshold_Max;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_PixelCount_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Min;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI2_PixelCount_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI2_PixelCount_Max;

                            // ROI 3
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_X;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Y;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Width;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Height;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_Offset_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Offset_X;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_Offset_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_Offset_Y;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_AngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_AngleRotate;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3UseOffset = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI3UseOffset == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3UseLocator = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI3UseLocator == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3ShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_bROI3ShowGraphics == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Min;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_GrayThreshold_Max;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_PixelCount_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Min;
                            NVIRecipe_PropGrid.RecipeCam1_PropertyGrid.ROI3_PixelCount_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam1.m_nROI3_PixelCount_Max;
                        }
                        break;
                    case 1:
                        {
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.UseReadCode = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bUseReadCode == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.UseInkjetCharactersInspect = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bUseInkjetCharactersInspect == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.UseRotateROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bUseRotateROI == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.MaxCodeCount = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nMaxCodeCount;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.NumberOfROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nNumberOfROI;
                            // Params Template Matching 
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_OuterX = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_OuterX;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_OuterY = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_OuterY;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_Outer_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Outer_Width;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_Outer_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Outer_Height;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_InnerX = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_InnerX;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_InnerY = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_InnerY;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_Inner_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Inner_Width;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateROI_Inner_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateROI_Inner_Height;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateCoordinatesX = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateCoordinatesX;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateCoordinatesY = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nTemplateCoordinatesY;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateAngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_dTemplateAngleRotate;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateMatchingRate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_dTemplateMatchingRate;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.TemplateShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bTemplateShowGraphics == 1 ? true : false;
                            // ROI 1                    
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_X;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Y;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Width;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Height;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_Offset_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Offset_X;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_Offset_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_Offset_Y;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_AngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_AngleRotate;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1UseOffset = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI1UseOffset == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1UseLocator = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI1UseLocator == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1ShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI1ShowGraphics == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_GrayThreshold_Min;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_GrayThreshold_Max;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_PixelCount_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_PixelCount_Min;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI1_PixelCount_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI1_PixelCount_Max;

                            // ROI 2                    
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_X;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Y;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Width;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Height;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_Offset_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Offset_X;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_Offset_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_Offset_Y;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_AngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_AngleRotate;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2UseOffset = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI2UseOffset == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2UseLocator = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI2UseLocator == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2ShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI2ShowGraphics == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_GrayThreshold_Min;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_GrayThreshold_Max;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_PixelCount_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_PixelCount_Min;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI2_PixelCount_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI2_PixelCount_Max;

                            // ROI 3                    
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_X;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Y;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_Width = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Width;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_Height = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Height;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_Offset_X = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Offset_X;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_Offset_Y = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_Offset_Y;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_AngleRotate = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_AngleRotate;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3UseOffset = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI3UseOffset == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3UseLocator = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI3UseLocator == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3ShowGraphics = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_bROI3ShowGraphics == 1 ? true : false;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_GrayThreshold_Min;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_GrayThreshold_Max;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_PixelCount_Min = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_PixelCount_Min;
                            NVIRecipe_PropGrid.RecipeCam2_PropertyGrid.ROI3_PixelCount_Max = InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe.m_NVisionInspRecipe_Cam2.m_nROI3_PixelCount_Max;
                        }
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
                }
            }

            NVisionInspectRecipePropertyGrid = NVIRecipe_PropGrid;
        }
        public async void SimulationThread_UpdateUI(int nBuff, int nFrame)
        {
            m_ucSettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetSimulatorBuffer(nBuff, nFrame);

            await m_ucSettingView.buffSettingPRO.UpdateImage();
        }
        private async void InspectionComplete(int nCamIdx, int bSetting)
        {
            if (bSetting == 1)
            {
                int nCoreIdx = nCamIdx;
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult);

                SettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(0, 0);
                await SettingView.buffSettingPRO.UpdateImage();

                switch (nCamIdx)
                {
                    case 1:
                        if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_bResultStatus == 1)
                        {
                            SettingView.buffSettingPRO.InspectResult = EnInspectResult.InspectResult_OK;
                        }
                        else
                        {
                            SettingView.buffSettingPRO.InspectResult = EnInspectResult.InspectResult_NG;
                        }

                        string resStr = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult.m_NVisionInspRes_Cam1.m_sResultString;
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
        private async void LocatorTrainComplete(int nCamIdx)
        {
            int nBuff = nCamIdx;
            int nFrame = 0;

            SettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(nBuff, nFrame);
            await SettingView.buffSettingPRO.UpdateImage();

            // show Template Image
            SettingView.buffSettingPRO.LoadBmpImageAsync(NVICam1Setting_PropGrid.TemplateImagePath + "template.png");

            // reload recipe
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.LoadRecipe(CameraCount, ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectRecipe);
            LoadRecipe(CameraCount);

            // update PropertyGrid UI
            UpdatePropertyGridUI(nCamIdx);
        }
        private void AlarmHandlerFunc(string alarm)
        {
            MessageBox.Show(alarm);
        }
        private void UpdatePropertyGridUI(int nCamIdx)
        {
            // update property UI
            switch (nCamIdx)
            {
                case 0:
                    SettingView.propGridRecipe_Cam1.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam1_PropertyGrid;
                    break;
                case 1:
                    SettingView.propGridRecipe_Cam2.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam2_PropertyGrid;
                    break;
                case 2:
                    SettingView.propGridRecipe_Cam3.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam3_PropertyGrid;
                    break;
                case 3:
                    SettingView.propGridRecipe_Cam4.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam4_PropertyGrid;
                    break;
                case 4:
                    SettingView.propGridRecipe_Cam5.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam5_PropertyGrid;
                    break;
                case 5:
                    SettingView.propGridRecipe_Cam6.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam6_PropertyGrid;
                    break;
                case 6:
                    SettingView.propGridRecipe_Cam7.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam7_PropertyGrid;
                    break;
                case 7:
                    SettingView.propGridRecipe_Cam8.SelectedObject = NVisionInspectRecipePropertyGrid.RecipeCam8_PropertyGrid;
                    break;
            }

        }
        #endregion

        #region Properties
        public UcSettingView SettingView { get { return m_ucSettingView; } }
        public List<string> CameraList
        {
            get => m_cameraLst;
            set
            {
                if (SetProperty(ref m_cameraLst, value))
                {

                }
            }
        }
        public List<string> FromImageSourceList
        {
            get => m_lstImageSource;
            set
            {
                if (SetProperty(ref m_lstImageSource, value))
                {

                }
            }
        }
        public List<string> ROIList
        {
            get => m_lstROI;
            set
            {
                if (SetProperty(ref m_lstROI, value))
                {

                }
            }
        }
        public List<int> NumberOfCamBrandList
        {
            get => m_lstNumberOfCamBrand;
            set
            {
                if (SetProperty(ref m_lstNumberOfCamBrand, value))
                {

                }
            }
        }
        public CNVisionInspectRecipe_PropertyGrid NVisionInspectRecipePropertyGrid
        {
            get => m_NVisionInspectRecipe_PropertyGrid;
            set => m_NVisionInspectRecipe_PropertyGrid = value;
        }
        public CNVisionInspectSystemSetting_PropertyGrid NVisionInspectSystemSettingsPropertyGrid
        {
            get => m_NVisionInspectSystemSetting_PropertyGrid;
            set => m_NVisionInspectSystemSetting_PropertyGrid = value;
        }

        public CNVisionInspectCameraSetting_PropertyGrid NVICam1Setting_PropGrid
        {
            get => m_NVICam1Setting_PropGrid;
            set => m_NVICam1Setting_PropGrid = value;
        }
        public CNVisionInspectCameraSetting_PropertyGrid NVICam2Setting_PropGrid
        {
            get => m_NVICam2Setting_PropGrid;
            set => m_NVICam2Setting_PropGrid = value;
        }
        public CNVisionInspectCameraSetting_PropertyGrid NVICam3Setting_PropGrid
        {
            get => m_NVICam3Setting_PropGrid;
            set => m_NVICam3Setting_PropGrid = value;
        }
        public CNVisionInspectCameraSetting_PropertyGrid NVICam4Setting_PropGrid
        {
            get => m_NVICam4Setting_PropGrid;
            set => m_NVICam4Setting_PropGrid = value;
        }
        public CNVisionInspectCameraSetting_PropertyGrid NVICam5Setting_PropGrid
        {
            get => m_NVICam5Setting_PropGrid;
            set => m_NVICam5Setting_PropGrid = value;
        }
        public CNVisionInspectCameraSetting_PropertyGrid NVICam6Setting_PropGrid
        {
            get => m_NVICam6Setting_PropGrid;
            set => m_NVICam6Setting_PropGrid = value;
        }
        public CNVisionInspectCameraSetting_PropertyGrid NVICam7Setting_PropGrid
        {
            get => m_NVICam7Setting_PropGrid;
            set => m_NVICam7Setting_PropGrid = value;
        }
        public CNVisionInspectCameraSetting_PropertyGrid NVICam8Setting_PropGrid
        {
            get => m_NVICam8Setting_PropGrid;
            set => m_NVICam8Setting_PropGrid = value;
        }

        public CameraStreamingController CameraStreamingController
        {
            get => m_cameraStreamingController;
            set => m_cameraStreamingController = value;
        }
        public Plc_Delta_Model PlcDeltaModelPropertyGrid
        {
            get => m_plcDeltaModel;
            set => m_plcDeltaModel = value;
        }
        public string DisplayImagePath_Live
        {
            get => m_strDisplayImagePath_Live;
            set
            {
                if (SetProperty(ref m_strDisplayImagePath_Live, value))
                {

                }
            }
        }
        public string DisplayImagePath_StartAcq
        {
            get => m_strDisplayImagePath_StartAcq;
            set
            {
                if (SetProperty(ref m_strDisplayImagePath_StartAcq, value))
                {

                }
            }
        }
        public string ROISelected
        {
            get => m_strROISelected;
            set
            {
                if (SetProperty(ref m_strROISelected, value))
                {
                    ROIIdx = SettingView.cbbROI.SelectedIndex + 1;
                }
            }
        }
        public bool IsStreamming
        {
            get => m_bStreamming;
            set
            {
                if (SetProperty(ref m_bStreamming, value))
                {
                    if (m_bStreamming)
                    {
                        DisplayImagePath_Live = "/NpcCore.Wpf;component/Resources/Images/btn_stop_all_50.png";
                    }
                    else
                    {
                        DisplayImagePath_Live = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
                    }
                }
            }
        }
        public bool IsStartedAcq
        {
            get => m_bStartedAcq;
            set
            {
                if (SetProperty(ref m_bStartedAcq, value))
                {
                    if (m_bStartedAcq)
                    {
                        DisplayImagePath_StartAcq = "/NpcCore.Wpf;component/Resources/Images/SideBarView/ic_hw_stop_s.png";
                    }
                    else
                    {
                        DisplayImagePath_StartAcq = "/NpcCore.Wpf;component/Resources/Images/btn_start_50.png";
                    }
                }
            }
        }
        public bool IsSelectedCamera
        {
            get => m_bSelectedCamera;
            set
            {
                if (!SetProperty(ref m_bSelectedCamera, value))
                {

                }
            }
        }
        public int ROIIdx
        {
            get => m_nROIIdx;
            set
            {
                if (SetProperty(ref m_nROIIdx, value))
                {

                }
            }
        }
        public int CameraCount
        {
            get => m_nCameraCount;
            set
            {
                if (SetProperty(ref m_nCameraCount, value))
                {

                }
            }
        }
        public EnImageSource FromImageSource
        {
            get => m_fromImageSource;
            set
            {
                if (SetProperty(ref m_fromImageSource, value))
                {
                    //switch(m_fromImageSource)
                    //{
                    //    case EnImageSource.FromToImage:
                    //        SettingView.btnContinuousGrab.IsEnabled = false;
                    //        SettingView.btnSingleGrab.IsEnabled = false;
                    //        SettingView.btnLoadImage.IsEnabled = true;

                    //        SettingView.btnContinuousGrab.Opacity = 0.3;
                    //        SettingView.btnSingleGrab.Opacity = 0.3;
                    //        SettingView.btnLoadImage.Opacity = 1.0;
                    //        break;
                    //    case EnImageSource.FromToCamera:
                    //        SettingView.btnContinuousGrab.IsEnabled = true;
                    //        SettingView.btnSingleGrab.IsEnabled = true;
                    //        SettingView.btnLoadImage.IsEnabled = false;

                    //        SettingView.btnContinuousGrab.Opacity = 1.0;
                    //        SettingView.btnSingleGrab.Opacity = 1.0;
                    //        SettingView.btnLoadImage.Opacity = 0.3;
                    //        break;
                    //}
                }
            }
        }
        public emCameraBrand CameraBrandSelected
        {
            get => m_cameraBrandSelected;
            set
            {
                if (SetProperty(ref m_cameraBrandSelected, value))
                {
                    
                }
            }
        }
        #endregion

        #region Commands
        public ICommand SaveLightSettingCmd { get; }
        public ICommand SavePlcSettingCmd { get; }
        public ICommand SaveCamSettingCmd { get; }
        public ICommand SaveSysSettingCmd { get; }
        public ICommand SaveRecipeCmd { get; }
        public ICommand SaveImageCmd { get; }
        public ICommand SingleGrabCmd { get; }
        public ICommand SelectROICmd { get; }
        public ICommand StartAcquisitionCmd { get; }
        public ICommand ContinuousGrabCmd { get; }
        public ICommand LoadImageCmd { get; }
        public ICommand LocateCmd { get; }
        public ICommand InspectCmd { get; }
        public ICommand ReadCodeCmd { get; }
        public ICommand NextPrevCmd { get; }

        #endregion
    }
}
