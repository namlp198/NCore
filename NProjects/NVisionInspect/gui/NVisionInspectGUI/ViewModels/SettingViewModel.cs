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
        private CNVisionInspectCameraSetting_PropertyGrid m_NVisionInspectCamera1Setting_PropertyGrid = new CNVisionInspectCameraSetting_PropertyGrid();
        private Plc_Delta_Model m_plcDeltaModel = new Plc_Delta_Model();

        private List<string> m_cameraLst = new List<string>();
        private List<string> m_lstImageSource = new List<string>();
        private List<string> m_lstROI = new List<string>();

        private string m_strDisplayImagePath_Live = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private string m_strDisplayImagePath_StartAcq = "/NpcCore.Wpf;component/Resources/Images/btn_start_50.png";
        private string m_strCameraSelected = string.Empty;
        private string m_strROISelected = string.Empty;

        private bool m_bStreamming = false;
        private bool m_bStartedAcq = false;
        private bool m_bSelectedCamera = false;

        private EnImageSource m_fromImageSource = EnImageSource.FromToCamera;

        #endregion

        #region Constructor
        public SettingViewModel(Dispatcher dispatcher, UcSettingView settingView)
        {
            _dispatcher = dispatcher;
            m_ucSettingView = settingView;

            m_ucSettingView.buffSettingPRO.CameraIndex = 99;
            m_ucSettingView.buffSettingPRO.ModeView = NCore.Wpf.BufferViewerSettingPRO.EnModeView.Color;
            m_ucSettingView.buffSettingPRO.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);

            this.SaveRecipeCmd = new SaveRecipeCmd();
            this.SaveSettingCmd = new SaveSettingCmd();
            this.SaveImageCmd = new SaveImageCmd();
            this.SelectROICmd = new SelectROICmd();
            this.SingleGrabCmd = new SingleGrabCmd();
            this.StartAcquisitionCmd = new StartAcquisitionCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.LoadImageCmd = new LoadImageCmd();
            this.LocateCmd = new LocateCmd();
            this.InspectCmd = new InspectCmd();
            this.ReadCodeCmd = new ReadCodeCmd();

            m_xmlManagement.Load(Defines.StartupProgPath + "\\VisionSettings\\Settings\\PlcSettings.config");

            m_lstImageSource = EnumUtil.GetEnumDescriptionToListString<EnImageSource>();
            SettingView.cbbImageSource.SelectionChanged += CbbImageSource_SelectionChanged;
            SettingView.cbbImageSource.SelectedIndex = 0;

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
            InterfaceManager.LocatorTrained += new InterfaceManager.LocatorTrained_Handler(LocatorTrained);

            #region IMPLEMENT EVENTS SETTING

            SettingView.buffSettingPRO.SelectCameraChanged += BuffSettingPRO_SelectCameraChanged;
            SettingView.buffSettingPRO.SelectFrameChanged += BuffSettingPRO_SelectFrameChanged;
            SettingView.buffSettingPRO.SelectTriggerModeChanged += BuffSettingPRO_SelectTriggerModeChanged;
            SettingView.buffSettingPRO.SelectTriggerSourceChanged += BuffSettingPRO_SelectTriggerSourceChanged;
            SettingView.buffSettingPRO.SetExposureTime += BuffSettingPRO_SetExposureTime;
            SettingView.buffSettingPRO.SetAnalogGain += BuffSettingPRO_SetAnalogGain;
            SettingView.buffSettingPRO.TrainLocator += BuffSettingPRO_TrainLocator;

            #endregion

            m_cameraStreamingController = new CameraStreamingController(SettingView.buffSettingPRO);
        }

        private void CbbImageSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox cbb = (ComboBox)sender;
            if(string.Compare(cbb.SelectedItem.ToString(), "Image") == 0)
            {
                FromImageSource = EnImageSource.FromToImage;
            }
            else if(string.Compare(cbb.SelectedItem.ToString(), "Camera") == 0)
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
            int nNumberOfROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nNumberOfROI;

            List<string> lstROI = new List<string>();
            for(int i = 0; i < nNumberOfROI; i++)
            {
                lstROI.Add("ROI " + (i + 1));
            }

            ListROI = lstROI;
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
            NVisionInspectRecipePropertyGrid.TemplateROI_OuterX = (int)SettingView.buffSettingPRO.RectOutSide.X;
            NVisionInspectRecipePropertyGrid.TemplateROI_OuterY = (int)SettingView.buffSettingPRO.RectOutSide.Y;
            NVisionInspectRecipePropertyGrid.TemplateROI_Outer_Width = (int)SettingView.buffSettingPRO.RectOutSide.Width;
            NVisionInspectRecipePropertyGrid.TemplateROI_Outer_Height = (int)SettingView.buffSettingPRO.RectOutSide.Height;
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

                cNVisionInspSystemSetting_PropertyGrid.InspectCameraCount = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_nInspectCameraCount;
                cNVisionInspSystemSetting_PropertyGrid.Simulation = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.ByPass = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bByPass == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.TestMode = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bTestMode == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.RecipeName = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sRecipeName;

                NVisionInspectSystemSettingsPropertyGrid = cNVisionInspSystemSetting_PropertyGrid;
            }
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
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.PlcCOM = nodeList[i].InnerText;
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.PlcCOM = nodeList[i].InnerText;
                                }
                                break;
                            case 1:
                                plc_Delta_Model_PropertyGrid.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                }
                                break;
                            case 2:
                                plc_Delta_Model_PropertyGrid.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                }
                                break;
                            case 3:
                                plc_Delta_Model_PropertyGrid.RegisterTriggerDelay = nodeList[i].InnerText;
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterTriggerDelay = nodeList[i].InnerText;
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay = nodeList[i].InnerText;
                                }
                                break;
                            case 4:
                                plc_Delta_Model_PropertyGrid.RegisterOutput1Delay = nodeList[i].InnerText;
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterOutput1Delay = nodeList[i].InnerText;
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay = nodeList[i].InnerText;
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
            cameraSetting_PropertyGrid.NumberOfROI = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_nNumberOfROI;
            cameraSetting_PropertyGrid.CameraName = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sCameraName;
            cameraSetting_PropertyGrid.InterfaceType = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sInterfaceType;
            cameraSetting_PropertyGrid.SensorType = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSensorType;
            cameraSetting_PropertyGrid.Manufacturer = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sManufacturer;
            cameraSetting_PropertyGrid.SerialNumber = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sSerialNumber;
            cameraSetting_PropertyGrid.Model = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sModel;
            cameraSetting_PropertyGrid.FullImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sFullImagePath;
            cameraSetting_PropertyGrid.DefectImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sDefectImagePath;
            cameraSetting_PropertyGrid.TemplateImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectCamSetting[nCamIdx].m_sTemplateImagePath;

            switch(nCamIdx)
            {
                case 0:
                    NVisionInspectCamera1SettingsPropertyGrid = cameraSetting_PropertyGrid;
                    break;
            }
        }
        public void SetAllParamPlcDelta()
        {
            string regTriggerDelay = MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay.Trim();
            short triggerDelay = (short)MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay;
            string regOutput1Delay = MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay.Trim();
            short output1Delay = (short)MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay;
            SetParameterPlcDelta(triggerDelay, regTriggerDelay);
            SetParameterPlcDelta(output1Delay, regOutput1Delay);
        }
        private void SetParameterPlcDelta(short value, string register)
        {
            if (register != null)
            {
                int indexReg = 0;
                int.TryParse(register.Substring(register.IndexOf("D") + 1), out indexReg);
                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressRegister += (uint)indexReg;

                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.SetParameterToSingleRegister(value);

                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressRegister = 4096; // reset to init value
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

            if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
            {
                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.PlcCOM = PlcDeltaModelPropertyGrid.PlcCOM;
                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay = PlcDeltaModelPropertyGrid.TriggerDelay;
                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay = PlcDeltaModelPropertyGrid.SignalNGDelay;
                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay = PlcDeltaModelPropertyGrid.RegisterTriggerDelay;
                MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay = PlcDeltaModelPropertyGrid.RegisterOutput1Delay;
            }

            m_xmlManagement.Save(Defines.StartupProgPath + "\\Settings\\PlcSettings.config");
            SetAllParamPlcDelta();
        }
        // Load Recipe
        public void LoadRecipe()
        {
            CNVisionInspectRecipe_PropertyGrid NVisionInspectRecipe_PropertyGrid = new CNVisionInspectRecipe_PropertyGrid();
            {
                NVisionInspectRecipe_PropertyGrid.UseReadCode = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bUseReadCode == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.UseInkjetCharactersInspect = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bUseInkjetCharactersInspect == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.UseRotateROI = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bUseRotateROI == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.MaxCodeCount = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nMaxCodeCount;
                // Params Template Matching
                NVisionInspectRecipe_PropertyGrid.TemplateROI_OuterX = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateROI_OuterX;
                NVisionInspectRecipe_PropertyGrid.TemplateROI_OuterY = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateROI_OuterY;
                NVisionInspectRecipe_PropertyGrid.TemplateROI_Outer_Width = InterfaceManager.Instance.m_processorManager.
                                                     m_NVisionInspectRecipe.m_nTemplateROI_Outer_Width;
                NVisionInspectRecipe_PropertyGrid.TemplateROI_Outer_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateROI_Outer_Height;
                NVisionInspectRecipe_PropertyGrid.TemplateROI_InnerX = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateROI_InnerX;
                NVisionInspectRecipe_PropertyGrid.TemplateROI_InnerY = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateROI_InnerY;
                NVisionInspectRecipe_PropertyGrid.TemplateROI_Inner_Width = InterfaceManager.Instance.m_processorManager.
                                                     m_NVisionInspectRecipe.m_nTemplateROI_Inner_Width;
                NVisionInspectRecipe_PropertyGrid.TemplateROI_Inner_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateROI_Inner_Height;
                NVisionInspectRecipe_PropertyGrid.TemplateCoordinatesX = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateCoordinatesX;
                NVisionInspectRecipe_PropertyGrid.TemplateCoordinatesY = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nTemplateCoordinatesY;
                NVisionInspectRecipe_PropertyGrid.TemplateAngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_dTemplateAngleRotate;
                NVisionInspectRecipe_PropertyGrid.TemplateMatchingRate = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_dTemplateMatchingRate;
                NVisionInspectRecipe_PropertyGrid.TemplateShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bTemplateShowGraphics == 1 ? true : false;
                // ROI 1
                NVisionInspectRecipe_PropertyGrid.ROI1_X = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_X;
                NVisionInspectRecipe_PropertyGrid.ROI1_Y = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_Y;
                NVisionInspectRecipe_PropertyGrid.ROI1_Width = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_Width;
                NVisionInspectRecipe_PropertyGrid.ROI1_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_Height;
                NVisionInspectRecipe_PropertyGrid.ROI1_AngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_AngleRotate;
                NVisionInspectRecipe_PropertyGrid.ROI1_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_GrayThreshold_Min;
                NVisionInspectRecipe_PropertyGrid.ROI1_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_GrayThreshold_Max;
                NVisionInspectRecipe_PropertyGrid.ROI1_PixelCount_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_PixelCount_Min;
                NVisionInspectRecipe_PropertyGrid.ROI1_PixelCount_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_PixelCount_Max;
                NVisionInspectRecipe_PropertyGrid.ROI1UseOffset = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI1UseOffset == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.ROI1UseLocator = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI1UseLocator == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.ROI1ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI1ShowGraphics == 1 ? true : false;
                // ROI 2
                NVisionInspectRecipe_PropertyGrid.ROI2_X = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_X;
                NVisionInspectRecipe_PropertyGrid.ROI2_Y = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_Y;
                NVisionInspectRecipe_PropertyGrid.ROI2_Width = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_Width;
                NVisionInspectRecipe_PropertyGrid.ROI2_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_Height;
                NVisionInspectRecipe_PropertyGrid.ROI2_AngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_AngleRotate;
                NVisionInspectRecipe_PropertyGrid.ROI2_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_GrayThreshold_Min;
                NVisionInspectRecipe_PropertyGrid.ROI2_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_GrayThreshold_Max;
                NVisionInspectRecipe_PropertyGrid.ROI2_PixelCount_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_PixelCount_Min;
                NVisionInspectRecipe_PropertyGrid.ROI2_PixelCount_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_PixelCount_Max;
                NVisionInspectRecipe_PropertyGrid.ROI2UseOffset = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI2UseOffset == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.ROI2UseLocator = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI2UseLocator == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.ROI2ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI2ShowGraphics == 1 ? true : false;
                // ROI 3
                NVisionInspectRecipe_PropertyGrid.ROI3_X = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_X;
                NVisionInspectRecipe_PropertyGrid.ROI3_Y = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_Y;
                NVisionInspectRecipe_PropertyGrid.ROI3_Width = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_Width;
                NVisionInspectRecipe_PropertyGrid.ROI3_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_Height;
                NVisionInspectRecipe_PropertyGrid.ROI3_AngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_AngleRotate;
                NVisionInspectRecipe_PropertyGrid.ROI3_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_GrayThreshold_Min;
                NVisionInspectRecipe_PropertyGrid.ROI3_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_GrayThreshold_Max;
                NVisionInspectRecipe_PropertyGrid.ROI3_PixelCount_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_PixelCount_Min;
                NVisionInspectRecipe_PropertyGrid.ROI3_PixelCount_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_PixelCount_Max;
                NVisionInspectRecipe_PropertyGrid.ROI3UseOffset = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI3UseOffset == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.ROI3UseLocator = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI3UseLocator == 1 ? true : false;
                NVisionInspectRecipe_PropertyGrid.ROI3ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI3ShowGraphics == 1 ? true : false;
            }
            NVisionInspectRecipePropertyGrid = NVisionInspectRecipe_PropertyGrid;
        }
        private async void SimulationThread_UpdateUI()
        {
            int nBuff = 0;
            int nFrame = 0;

            m_ucSettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetSimulatorBuffer(nBuff, nFrame);

            await m_ucSettingView.buffSettingPRO.UpdateImage();
        }
        private async void InspectionComplete(int bSetting)
        {
            if (bSetting == 1)
            {
                InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetInspectionResult(0, ref InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult[0]);

                SettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(0, 0);
                await SettingView.buffSettingPRO.UpdateImage();

                if (InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult[0].m_bResultStatus == 1)
                {
                    SettingView.buffSettingPRO.InspectResult = EnInspectResult.InspectResult_OK;
                }
                else
                {
                    SettingView.buffSettingPRO.InspectResult = EnInspectResult.InspectResult_NG;
                }

                string resStr = InterfaceManager.Instance.m_processorManager.m_NVisionInspectResult[0].m_sResultString;
            }
        }
        private async void LocatorTrained()
        {
            SettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetResultBuffer(0, 0);
            await SettingView.buffSettingPRO.UpdateImage();

            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.ReloadRecipe();
            LoadRecipe();
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
        public List<string> ListFromImageSource
        {
            get => m_lstImageSource;
            set
            {
                if (SetProperty(ref m_lstImageSource, value))
                {

                }
            }
        }
        public List<string> ListROI
        {
            get => m_lstROI;
            set
            {
                if (SetProperty(ref m_lstROI, value))
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
        public CNVisionInspectCameraSetting_PropertyGrid NVisionInspectCamera1SettingsPropertyGrid
        {
            get => m_NVisionInspectCamera1Setting_PropertyGrid;
            set => m_NVisionInspectCamera1Setting_PropertyGrid = value;
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
        public EnImageSource FromImageSource
        {
            get => m_fromImageSource;
            set
            {
                if(SetProperty(ref m_fromImageSource, value))
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
        #endregion

        #region Commands
        public ICommand SaveSettingCmd { get; }
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
        
        #endregion
    }
}
