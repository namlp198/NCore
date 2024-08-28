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
using DocumentFormat.OpenXml.Spreadsheet;

namespace NVisionInspectGUI.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSettingView m_ucSettingView;
        private XmlManagement m_xmlManagement = new XmlManagement();
        public CameraStreamingController m_cameraStreamingController = null;
        private CNVisionInspectRecipe_PropertyGrid m_NVisionInspectRecipe_PropertyGrid = new CNVisionInspectRecipe_PropertyGrid();
        private CNVisionInspectSystemSetting_PropertyGrid m_NVisionInspectSystemSetting_PropertyGrid = new CNVisionInspectSystemSetting_PropertyGrid();
        private Plc_Delta_Model m_plcDeltaModel = new Plc_Delta_Model();

        private List<string> m_cameraLst = new List<string>();

        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private string m_strCameraSelected = string.Empty;

        private bool m_bStreamming = false;

        #endregion

        #region Constructor
        public SettingViewModel(Dispatcher dispatcher, UcSettingView settingView)
        {
            _dispatcher = dispatcher;
            m_ucSettingView = settingView;

            m_ucSettingView.buffSettingPRO.CameraIndex = -1;
            m_ucSettingView.buffSettingPRO.ModeView = NCore.Wpf.BufferViewerSettingPRO.EnModeView.Color;
            m_ucSettingView.buffSettingPRO.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);

            this.SaveRecipeCmd = new SaveRecipeCmd();
            this.SaveSettingCmd = new SaveSettingCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.SingleGrabCmd = new SingleGrabCmd();
            this.LoadImageCmd = new LoadImageCmd();
            this.InspectSimulationCmd = new InspectSimulationCmd();

            m_xmlManagement.Load(Defines.StartupProgPath + "\\VisionSettings\\Settings\\PlcSettings.config");

            m_cameraStreamingController = new CameraStreamingController(m_ucSettingView.buffSettingPRO.FrameWidth,
                                                                        m_ucSettingView.buffSettingPRO.FrameHeight,
                                                                        m_ucSettingView.buffSettingPRO,
                                                                        m_ucSettingView.buffSettingPRO.ModeView);

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
            InterfaceManager.LocatorTrained += new InterfaceManager.LocatorTrained_Handler(LocatorTrained);

            #region IMPLEMENT EVENTS SETTING

            SettingView.buffSettingPRO.SelectCameraChanged += BuffSetting_SelectCameraChanged;
            SettingView.buffSettingPRO.SelectFrameChanged += BuffSetting_SelectFrameChanged;
            SettingView.buffSettingPRO.SelectTriggerModeChanged += BuffSetting_SelectTriggerModeChanged;
            SettingView.buffSettingPRO.SelectTriggerSourceChanged += BuffSetting_SelectTriggerSourceChanged;
            SettingView.buffSettingPRO.SetExposureTime += BuffSetting_SetExposureTime;
            SettingView.buffSettingPRO.SetAnalogGain += BuffSetting_SetAnalogGain;

            #endregion
        }

        private void BuffSetting_SelectInspect(object sender, RoutedEventArgs e)
        {

        }

        private void BuffSetting_SelectReadCodeTool(object sender, RoutedEventArgs e)
        {

        }

        private void BuffSetting_SelectCameraChanged(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("select camera changed");
        }
        private void BuffSetting_SelectFrameChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BuffSetting_LoadImage(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_simulationThread.LoadImage();
        }

        private void BuffSetting_SingleGrab(object sender, RoutedEventArgs e)
        {
            m_cameraStreamingController.SingleGrab();
        }

        private async void BuffSetting_ContinuousGrab(object sender, RoutedEventArgs e)
        {
            if (SettingView.buffSettingPRO.IsStreamming == false)
                await m_cameraStreamingController.ContinuousGrab(Manager.Class.CameraType.Basler);
            else
                await m_cameraStreamingController.StopGrab(Manager.Class.CameraType.Basler);
        }
        private void BuffSetting_SelectTriggerSourceChanged(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetTriggerSource(
                MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex,
                (int)MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.TriggerSource);
        }

        private void BuffSetting_SelectTriggerModeChanged(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetTriggerMode(
                MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.CameraIndex,
                (int)MainViewModel.Instance.SettingVM.SettingView.buffSettingPRO.TriggerMode);
        }

        private void BuffSetting_SaveImage(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SaveImage(0);
        }

        private void BuffSetting_SetAnalogGain(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetAnalogGain(
               SettingView.buffSettingPRO.CameraIndex, SettingView.buffSettingPRO.AnalogGain);
        }

        private void BuffSetting_SetExposureTime(object sender, System.Windows.RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.SetExposureTime(
                SettingView.buffSettingPRO.CameraIndex, SettingView.buffSettingPRO.ExposureTime);
        }
        #endregion

        #region Methods

        // Load System Setting
        public void LoadSystemSettings()
        {
            {
                CNVisionInspectSystemSetting_PropertyGrid cNVisionInspSystemSetting_PropertyGrid = new CNVisionInspectSystemSetting_PropertyGrid();
                cNVisionInspSystemSetting_PropertyGrid.m_bSaveFullImage = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSaveFullImage == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.m_bSaveDefectImage = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSaveDefectImage == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.m_bShowDetailImage = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bShowDetailImage == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.m_bSimulation = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bSimulation == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.m_bByPass = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bByPass == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.m_bTestMode = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_bTestMode == 1 ? true : false;
                cNVisionInspSystemSetting_PropertyGrid.m_sFullImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sFullImagePath;
                cNVisionInspSystemSetting_PropertyGrid.m_sDefectImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sDefectImagePath;
                cNVisionInspSystemSetting_PropertyGrid.m_sTemplateImagePath = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sTemplateImagePath;
                cNVisionInspSystemSetting_PropertyGrid.m_sModelName = InterfaceManager.Instance.m_processorManager.m_NVisionInspectSysSettings.m_sModelName;

                NVisionInspectSystemSettingsPropertyGrid = cNVisionInspSystemSetting_PropertyGrid;
            }
        }
        // Load Plc Settings
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
                NVisionInspectRecipe_PropertyGrid.ROI1_OffsetX = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_OffsetX;
                NVisionInspectRecipe_PropertyGrid.ROI1_OffsetY = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI1_OffsetY;
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
                NVisionInspectRecipe_PropertyGrid.ROI1ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI1ShowGraphics == 1 ? true : false;
                // ROI 2
                NVisionInspectRecipe_PropertyGrid.ROI2_OffsetX = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_OffsetX;
                NVisionInspectRecipe_PropertyGrid.ROI2_OffsetY = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI2_OffsetY;
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
                NVisionInspectRecipe_PropertyGrid.ROI2ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI2ShowGraphics == 1 ? true : false;
                // ROI 3
                NVisionInspectRecipe_PropertyGrid.ROI3_OffsetX = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_OffsetX;
                NVisionInspectRecipe_PropertyGrid.ROI3_OffsetY = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_nROI3_OffsetY;
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
                NVisionInspectRecipe_PropertyGrid.ROI3ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_NVisionInspectRecipe.m_bROI3ShowGraphics == 1 ? true : false;
            }
            NVisionInspectRecipePropertyGrid = NVisionInspectRecipe_PropertyGrid;
        }
        private async void SimulationThread_UpdateUI()
        {
            m_ucSettingView.buffSettingPRO.BufferView = InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.GetSimulatorBuffer(0, 0);

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
        public Plc_Delta_Model PlcDeltaModelPropertyGrid
        {
            get => m_plcDeltaModel;
            set => m_plcDeltaModel = value;
        }
        public string DisplayImagePath
        {
            get => _displayImagePath;
            set
            {
                if (SetProperty(ref _displayImagePath, value))
                {

                }
            }
        }
        public string StrCameraSelected
        {
            get => m_strCameraSelected;
            set
            {
                if (SetProperty(ref m_strCameraSelected, value))
                {
                    if (string.Compare("Cam 1", m_strCameraSelected) == 0)
                    {
                        m_ucSettingView.buffSettingPRO.CameraIndex = 0;
                        m_ucSettingView.buffSettingPRO.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);

                    }
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
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/btn_stop_all_50.png";
                        m_ucSettingView.cbbCameraList.IsEnabled = false;

                    }
                    else
                    {
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
                        m_ucSettingView.cbbCameraList.IsEnabled = true;

                    }
                }
            }
        }
        #endregion

        #region Commands
        public ICommand SaveSettingCmd { get; }
        public ICommand SaveRecipeCmd { get; }
        public ICommand ContinuousGrabCmd { get; }
        public ICommand SingleGrabCmd { get; }
        public ICommand LoadImageCmd { get; }
        public ICommand InspectSimulationCmd { get; }

        #endregion
    }
}
