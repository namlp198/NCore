using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using Npc.Foundation.Base;
using ReadCodeGUI.Command.Cmd;
using ReadCodeGUI.Manager;
using ReadCodeGUI.Models;
using ReadCodeGUI.Views.UcViews;
using Kis.Toolkit;
using ReadCodeGUI.Commons;
using System.Xml;
using System.ComponentModel;
using System.Reflection;
using ReadCodeGUI.Manager.Class;
using DocumentFormat.OpenXml.Bibliography;
using LSIS.Driver.Core.DataTypes;
using System.Threading;
using NCore.Wpf.BufferViewerSimple;
using System.Windows;

namespace ReadCodeGUI.ViewModels
{
    public enum ECameraList
    {
        [Description("Cam 1")]
        Cam1
    }
    public class SettingViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSettingView _settingView;
        private XmlManagement m_xmlManagement = new XmlManagement();
        public CameraStreamingController m_cameraStreamingController = null;
        private CReadCodeRecipe_PropertyGrid m_readCodeRecipe_PropertyGrid = new CReadCodeRecipe_PropertyGrid();
        private CReadCodeSystemSetting_PropertyGrid m_readCodeSystemSetting_PropertyGrid = new CReadCodeSystemSetting_PropertyGrid();
        private Plc_Delta_Model m_plcDeltaModel = new Plc_Delta_Model();

        private List<SystemSettingsMapToDataGridModel> m_sysSettingsMapToDataGridModels = new List<SystemSettingsMapToDataGridModel>();
        private List<RecipeMapToDataGridModel> m_recipeMapToDataGridModels = new List<RecipeMapToDataGridModel>();
        private List<PlcSettingsMapToDataGridModel> m_plcSettingsMapToDGModel = new List<PlcSettingsMapToDataGridModel>();
        private List<string> m_cameraLst = new List<string>();

        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private string m_strCameraSelected = string.Empty;

        private bool m_bStreamming = false;

        private ECameraList m_cameraSelected = new ECameraList();

        #endregion

        #region Constructor
        public SettingViewModel(Dispatcher dispatcher, UcSettingView settingView)
        {
            _dispatcher = dispatcher;
            _settingView = settingView;

            CameraList = GetEnumDescriptionToListString();

            _settingView.buffSetting.CameraIndex = 99;
            _settingView.buffSetting.ModeView = NCore.Wpf.BufferViewerSetting.EnModeView.Color;
            _settingView.buffSetting.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);

            this.SaveRecipeCmd = new SaveRecipeCmd();
            this.SaveSettingCmd = new SaveSettingCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.SingleGrabCmd = new SingleGrabCmd();
            this.LoadImageCmd = new LoadImageCmd();
            this.InspectSimulationCmd = new InspectSimulationCmd();

            m_xmlManagement.Load(Defines.StartupProgPath + "\\Settings\\PlcSettings.config");

            m_cameraStreamingController = new CameraStreamingController(_settingView.buffSetting.FrameWidth,
                                                                        _settingView.buffSetting.FrameHeight,
                                                                        _settingView.buffSetting,
                                                                        _settingView.buffSetting.ModeView);

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
            InterfaceManager.LocatorTrained += new InterfaceManager.LocatorTrained_Handler(LocatorTrained);

            #region IMPLEMENT EVENTS SETTING

            SettingView.buffSetting.SelectCameraChanged += BuffSetting_SelectCameraChanged;
            SettingView.buffSetting.SelectFrameChanged += BuffSetting_SelectFrameChanged;
            SettingView.buffSetting.SelectTriggerModeChanged += BuffSetting_SelectTriggerModeChanged;
            SettingView.buffSetting.SelectTriggerSourceChanged += BuffSetting_SelectTriggerSourceChanged;
            SettingView.buffSetting.SetExposureTime += BuffSetting_SetExposureTime;
            SettingView.buffSetting.SetAnalogGain += BuffSetting_SetAnalogGain;

            SettingView.buffSetting.ContinuousGrab += BuffSetting_ContinuousGrab;
            SettingView.buffSetting.SingleGrab += BuffSetting_SingleGrab;
            SettingView.buffSetting.LoadImage += BuffSetting_LoadImage;
            SettingView.buffSetting.SaveImage += BuffSetting_SaveImage;

            SettingView.buffSetting.SelectLocatorTool += BuffSetting_SelectLocatorTool;
            SettingView.buffSetting.SelectReadCodeTool += BuffSetting_SelectReadCodeTool;
            SettingView.buffSetting.SelectInspect += BuffSetting_SelectInspect;

            #endregion
        }

        private void BuffSetting_SelectInspect(object sender, RoutedEventArgs e)
        {

        }

        private void BuffSetting_SelectReadCodeTool(object sender, RoutedEventArgs e)
        {

        }

        private async void BuffSetting_SelectLocatorTool(object sender, RoutedEventArgs e)
        {
            bool bRes = false;
            if (SettingView.buffSetting.UseLoadImage)
            {
                bRes = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.LocatorToolSimulator_Train(0, 0);
            }
            else
            {
                bRes = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.LocatorTool_Train(SettingView.buffSetting.CameraIndex);
            }

            SettingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetResultBuffer(0, 0);
            await SettingView.buffSetting.UpdateImage();

            //InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.ReloadRecipe();
            //LoadRecipe();
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
            if (SettingView.buffSetting.IsStreamming == false)
                await m_cameraStreamingController.ContinuousGrab(Manager.Class.CameraType.Basler);
            else
                await m_cameraStreamingController.StopGrab(Manager.Class.CameraType.Basler);
        }

        private void BuffSetting_SelectFrameChanged(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BuffSetting_SelectTriggerSourceChanged(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SetTriggerSource(
                MainViewModel.Instance.SettingVM.SettingView.buffSetting.CameraIndex,
                (int)MainViewModel.Instance.SettingVM.SettingView.buffSetting.TriggerSource);
        }

        private void BuffSetting_SelectTriggerModeChanged(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SetTriggerMode(
                MainViewModel.Instance.SettingVM.SettingView.buffSetting.CameraIndex,
                (int)MainViewModel.Instance.SettingVM.SettingView.buffSetting.TriggerMode);
        }

        private void BuffSetting_SaveImage(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SaveImage(0);
        }

        private void BuffSetting_SelectCameraChanged(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("select camera changed");
        }

        private void BuffSetting_SetAnalogGain(object sender, RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SetAnalogGain(
               SettingView.buffSetting.CameraIndex, SettingView.buffSetting.AnalogGain);
        }

        private void BuffSetting_SetExposureTime(object sender, System.Windows.RoutedEventArgs e)
        {
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.SetExposureTime(
                SettingView.buffSetting.CameraIndex, SettingView.buffSetting.ExposureTime);
        }
        #endregion

        #region Methods

        // Load System Setting
        public void LoadSystemSettings()
        {
            {
                CReadCodeSystemSetting_PropertyGrid cReadCodeSystemSetting_PropertyGrid = new CReadCodeSystemSetting_PropertyGrid();
                cReadCodeSystemSetting_PropertyGrid.m_bSaveFullImage = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveFullImage == 1 ? true : false;
                cReadCodeSystemSetting_PropertyGrid.m_bSaveDefectImage = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveDefectImage == 1 ? true : false;
                cReadCodeSystemSetting_PropertyGrid.m_bShowDetailImage = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bShowDetailImage == 1 ? true : false;
                cReadCodeSystemSetting_PropertyGrid.m_bSimulation = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSimulation == 1 ? true : false;
                cReadCodeSystemSetting_PropertyGrid.m_bByPass = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bByPass == 1 ? true : false;
                cReadCodeSystemSetting_PropertyGrid.m_bTestMode = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bTestMode == 1 ? true : false;
                cReadCodeSystemSetting_PropertyGrid.m_sFullImagePath = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sFullImagePath;
                cReadCodeSystemSetting_PropertyGrid.m_sDefectImagePath = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sDefectImagePath;
                cReadCodeSystemSetting_PropertyGrid.m_sTemplateImagePath = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sTemplateImagePath;
                cReadCodeSystemSetting_PropertyGrid.m_sModelName = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sModelName;

                ReadCodeSystemSettingsPropertyGrid = cReadCodeSystemSetting_PropertyGrid;
            }

            List<SystemSettingsMapToDataGridModel> sysSettingLst = new List<SystemSettingsMapToDataGridModel>();
            int nPropertyCount = typeof(CReadCodeSystemSetting).GetFields().Count();
            string value = string.Empty;
            // Don't care to CSealingInspectLightSetting: nPropertyCount - 1
            for (int i = 0; i < nPropertyCount; i++)
            {
                SystemSettingsMapToDataGridModel sysSetting = new SystemSettingsMapToDataGridModel();
                sysSetting.Index = i + 1;
                sysSetting.Params = GetParamNameAndValue_SystemSetting(i, ref value);
                sysSetting.Value = value;
                sysSettingLst.Add(sysSetting);
            }
            SystemSettingsMapToDataGridModels = sysSettingLst;
        }
        private string GetParamNameAndValue_SystemSetting(int idx, ref string value)
        {
            switch (idx)
            {
                case 0:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveFullImage + "";
                    return "Save Full Image (0: No, 1: Yes)";
                case 1:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveDefectImage + "";
                    return "Save Defect Image (0: No, 1: Yes)";
                case 2:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bShowDetailImage + "";
                    return "Show Detail Image (0: No, 1: Yes)";
                case 3:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSimulation + "";
                    return "Simulation (0: No, 1: Yes)";
                case 4:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bByPass + "";
                    return "By Pass (0: No, 1: Yes)";
                case 5:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sFullImagePath;
                    return "Full Image Path";
                case 6:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sDefectImagePath;
                    return "Defect Image Path";
                case 7:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sModelName;
                    return "Model Name";
                case 8:
                    value = InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bTestMode + "";
                    return "Test Mode (0: No, 1: Yes)";
            }
            return "";
        }
        public void SetValue_SystemSetting()
        {
            for (int i = 0; i < SystemSettingsMapToDataGridModels.Count; i++)
            {
                string value = SystemSettingsMapToDataGridModels[i].Value;
                switch (i)
                {
                    case 0:
                        int.TryParse(value, out InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveFullImage);
                        break;
                    case 1:
                        int.TryParse(value, out InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSaveDefectImage);
                        break;
                    case 2:
                        int.TryParse(value, out InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bShowDetailImage);
                        break;
                    case 3:
                        int.TryParse(value, out InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bSimulation);
                        break;
                    case 4:
                        int.TryParse(value, out InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bByPass);
                        break;
                    case 5:
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sFullImagePath = value;
                        break;
                    case 6:
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sDefectImagePath = value;
                        break;
                    case 7:
                        InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_sModelName = value;
                        break;
                    case 8:
                        int.TryParse(value, out InterfaceManager.Instance.m_processorManager.m_readCodeSysSettings.m_bTestMode);
                        break;
                }
            }
        }

        // Load Plc Settings
        public void LoadPlcSettings()
        {
            List<PlcSettingsMapToDataGridModel> plcSettings = new List<PlcSettingsMapToDataGridModel>();
            Plc_Delta_Model plc_Delta_Model_PropertyGrid = new Plc_Delta_Model();

            XmlNode settingNode = m_xmlManagement.SelectSingleNode("//PlcSettings");
            if (settingNode != null)
            {
                XmlNodeList nodeList = settingNode.ChildNodes;
                if (nodeList.Count > 0)
                {
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        PlcSettingsMapToDataGridModel plcSetting = new PlcSettingsMapToDataGridModel();
                        switch (i)
                        {
                            case 0:
                                plc_Delta_Model_PropertyGrid.PlcCOM = nodeList[i].InnerText;
                                plcSetting.Params = "Plc COM";
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.PlcCOM = nodeList[i].InnerText;
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.PlcCOM = nodeList[i].InnerText;
                                }
                                break;
                            case 1:
                                plc_Delta_Model_PropertyGrid.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                plcSetting.Params = "Trigger Delay (ms)";
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay = int.Parse(nodeList[i].InnerText);
                                }
                                break;
                            case 2:
                                plc_Delta_Model_PropertyGrid.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                plcSetting.Params = "Signal NG Delay (ms)";
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay = int.Parse(nodeList[i].InnerText);
                                }
                                break;
                            case 3:
                                plc_Delta_Model_PropertyGrid.RegisterTriggerDelay = nodeList[i].InnerText;
                                plcSetting.Params = "Register Trigger Delay";
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterTriggerDelay = nodeList[i].InnerText;
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay = nodeList[i].InnerText;
                                }
                                break;
                            case 4:
                                plc_Delta_Model_PropertyGrid.RegisterOutput1Delay = nodeList[i].InnerText;
                                plcSetting.Params = "Register Output 1 Delay";
                                if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                                {
                                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterOutput1Delay = nodeList[i].InnerText;
                                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay = nodeList[i].InnerText;
                                }
                                break;
                        }
                        plcSetting.Index = i + 1;
                        plcSetting.Value = nodeList[i].InnerText;
                        plcSettings.Add(plcSetting);
                    }
                    PlcDeltaModelPropertyGrid = plc_Delta_Model_PropertyGrid;
                }
            }

            PlcSettingsMapToDGModels = plcSettings;
            //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.Initialize();

            //MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.Initialize();
            //SetAllParamPlcDelta();
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
            for (int i = 0; i < PlcSettingsMapToDGModels.Count; i++)
            {
                string value = PlcSettingsMapToDGModels[i].Value;
                switch (i)
                {
                    case 0:
                        XmlNode plcCOMNode = m_xmlManagement.SelectSingleNode("//PlcSettings//PLC_COM");
                        plcCOMNode.InnerText = value;
                        if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                        {
                            //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.PlcCOM = value;
                            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.PlcCOM = value;
                        }
                        break;
                    case 1:
                        XmlNode triggerDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//TriggerDelay");
                        triggerDelayNode.InnerText = value;

                        if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                        {
                            //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.TriggerDelay = int.Parse(value);
                            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.TriggerDelay = int.Parse(value);
                        }
                        break;
                    case 2:
                        XmlNode signalNGDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//SignalNGDelay");
                        signalNGDelayNode.InnerText = value;
                        if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                        {
                            //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.SignalNGDelay = int.Parse(value);
                            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.SignalNGDelay = int.Parse(value);
                        }
                        break;
                    case 3:
                        XmlNode registerTriggerDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//RegisterTriggerDelay");
                        registerTriggerDelayNode.InnerText = value;
                        if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                        {
                            //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterTriggerDelay = value;
                            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterTriggerDelay = value;
                        }
                        break;
                    case 4:
                        XmlNode registerOutput1DelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//RegisterOutput1Delay");
                        registerOutput1DelayNode.InnerText = value;
                        if (MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP != null)
                        {
                            //MainViewModel.Instance.RunVM.SumCamVM.Plc_LS.PlcLSModel.RegisterOutput1Delay = value;
                            MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.PlcDeltaModel.RegisterOutput1Delay = value;
                        }
                        break;
                }
            }

            m_xmlManagement.Save(Defines.StartupProgPath + "\\Settings\\PlcSettings.config");
            SetAllParamPlcDelta();
        }

        // Load Recipe
        public void LoadRecipe()
        {
            CReadCodeRecipe_PropertyGrid readCodeRecipe_PropertyGrid = new CReadCodeRecipe_PropertyGrid();
            {
                readCodeRecipe_PropertyGrid.UseReadCode = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_bUseReadCode == 1 ? true : false;
                readCodeRecipe_PropertyGrid.UseInkjetCharactersInspect = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_bUseInkjetCharactersInspect == 1 ? true : false;
                readCodeRecipe_PropertyGrid.UseRotateROI = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_bUseRotateROI == 1 ? true : false;
                readCodeRecipe_PropertyGrid.MaxCodeCount = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nMaxCodeCount;
                // Params Template Matching
                readCodeRecipe_PropertyGrid.TemplateROI_OuterX = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateROI_OuterX;
                readCodeRecipe_PropertyGrid.TemplateROI_OuterY = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateROI_OuterY;
                readCodeRecipe_PropertyGrid.TemplateROI_Outer_Width = InterfaceManager.Instance.m_processorManager.
                                                     m_readCodeRecipe.m_nTemplateROI_Outer_Width;
                readCodeRecipe_PropertyGrid.TemplateROI_Outer_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateROI_Outer_Height;
                readCodeRecipe_PropertyGrid.TemplateROI_InnerX = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateROI_InnerX;
                readCodeRecipe_PropertyGrid.TemplateROI_InnerY = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateROI_InnerY;
                readCodeRecipe_PropertyGrid.TemplateROI_Inner_Width = InterfaceManager.Instance.m_processorManager.
                                                     m_readCodeRecipe.m_nTemplateROI_Inner_Width;
                readCodeRecipe_PropertyGrid.TemplateROI_Inner_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateROI_Inner_Height;
                readCodeRecipe_PropertyGrid.TemplateCoordinatesX = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateCoordinatesX;
                readCodeRecipe_PropertyGrid.TemplateCoordinatesY = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nTemplateCoordinatesY;
                readCodeRecipe_PropertyGrid.TemplateAngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_dTemplateAngleRotate;
                readCodeRecipe_PropertyGrid.TemplateMatchingRate = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_dTemplateMatchingRate;
                readCodeRecipe_PropertyGrid.TemplateShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_bTemplateShowGraphics == 1 ? true : false;
                // ROI 1
                readCodeRecipe_PropertyGrid.ROI1_OffsetX = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_OffsetX;
                readCodeRecipe_PropertyGrid.ROI1_OffsetY = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_OffsetY;
                readCodeRecipe_PropertyGrid.ROI1_Width = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_Width;
                readCodeRecipe_PropertyGrid.ROI1_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_Height;
                readCodeRecipe_PropertyGrid.ROI1_AngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_AngleRotate;
                readCodeRecipe_PropertyGrid.ROI1_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_GrayThreshold_Min;
                readCodeRecipe_PropertyGrid.ROI1_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_GrayThreshold_Max;
                readCodeRecipe_PropertyGrid.ROI1_PixelCount_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_PixelCount_Min;
                readCodeRecipe_PropertyGrid.ROI1_PixelCount_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI1_PixelCount_Max;
                readCodeRecipe_PropertyGrid.ROI1ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_bROI1ShowGraphics == 1 ? true : false;
                // ROI 2
                readCodeRecipe_PropertyGrid.ROI2_OffsetX = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_OffsetX;
                readCodeRecipe_PropertyGrid.ROI2_OffsetY = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_OffsetY;
                readCodeRecipe_PropertyGrid.ROI2_Width = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_Width;
                readCodeRecipe_PropertyGrid.ROI2_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_Height;
                readCodeRecipe_PropertyGrid.ROI2_AngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_AngleRotate;
                readCodeRecipe_PropertyGrid.ROI2_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_GrayThreshold_Min;
                readCodeRecipe_PropertyGrid.ROI2_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_GrayThreshold_Max;
                readCodeRecipe_PropertyGrid.ROI2_PixelCount_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_PixelCount_Min;
                readCodeRecipe_PropertyGrid.ROI2_PixelCount_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI2_PixelCount_Max;
                readCodeRecipe_PropertyGrid.ROI2ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_bROI2ShowGraphics == 1 ? true : false;
                // ROI 3
                readCodeRecipe_PropertyGrid.ROI3_OffsetX = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_OffsetX;
                readCodeRecipe_PropertyGrid.ROI3_OffsetY = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_OffsetY;
                readCodeRecipe_PropertyGrid.ROI3_Width = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_Width;
                readCodeRecipe_PropertyGrid.ROI3_Height = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_Height;
                readCodeRecipe_PropertyGrid.ROI3_AngleRotate = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_AngleRotate;
                readCodeRecipe_PropertyGrid.ROI3_GrayThreshold_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_GrayThreshold_Min;
                readCodeRecipe_PropertyGrid.ROI3_GrayThreshold_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_GrayThreshold_Max;
                readCodeRecipe_PropertyGrid.ROI3_PixelCount_Min = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_PixelCount_Min;
                readCodeRecipe_PropertyGrid.ROI3_PixelCount_Max = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_nROI3_PixelCount_Max;
                readCodeRecipe_PropertyGrid.ROI3ShowGraphics = InterfaceManager.Instance.m_processorManager.
                                                      m_readCodeRecipe.m_bROI3ShowGraphics == 1 ? true : false;
            }
            ReadCodeRecipePropertyGrid = readCodeRecipe_PropertyGrid;

            List<RecipeMapToDataGridModel> recipeModels = new List<RecipeMapToDataGridModel>();

            int nPropertyCount = typeof(CReadCodeRecipe).GetFields().Count();

            for (int i = 0; i < nPropertyCount; i++)
            {
                RecipeMapToDataGridModel recipe = new RecipeMapToDataGridModel();
                switch (i)
                {
                    case 0:
                        recipe.Index = i + 1;
                        recipe.Params = "Use Read Code Tool (1: Use, 0: Not Use)";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseReadCode + "";
                        break;
                    case 1:
                        recipe.Index = i + 1;
                        recipe.Params = "Use Inkjet Characters Inspect Tool (1: Use, 0: Not Use)";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseInkjetCharactersInspect + "";
                        break;
                    case 2:
                        recipe.Index = i + 1;
                        recipe.Params = "Use Rotate ROI (1: Use, 0: Not Use)";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_bUseRotateROI + "";
                        break;
                    case 3:
                        recipe.Index = i + 1;
                        recipe.Params = "Max Code Count";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nMaxCodeCount + "";
                        break;
                    // Template Matching
                    case 4:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Outer X";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_OuterX + "";
                        break;
                    case 5:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Outer Y";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_OuterY + "";
                        break;
                    case 6:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Outer Width";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Outer_Width + "";
                        break;
                    case 7:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Outer Height";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Outer_Height + "";
                        break;
                    case 8:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Inner X";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_InnerX + "";
                        break;
                    case 9:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Inner Y";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_InnerY + "";
                        break;
                    case 10:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Inner Width";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Inner_Width + "";
                        break;
                    case 11:
                        recipe.Index = i + 1;
                        recipe.Params = "Template ROI Inner Height";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateROI_Inner_Height + "";
                        break;
                    case 12:
                        recipe.Index = i + 1;
                        recipe.Params = "Template Coordinates X";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateCoordinatesX + "";
                        break;
                    case 13:
                        recipe.Index = i + 1;
                        recipe.Params = "Template Coordinates Y";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nTemplateCoordinatesY + "";
                        break;
                    case 14:
                        recipe.Index = i + 1;
                        recipe.Params = "Template Angle Rotate";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_dTemplateAngleRotate + "";
                        break;
                    // ROI1
                    case 15:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Offset X";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_OffsetX + "";
                        break;
                    case 16:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Offset Y";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_OffsetY + "";
                        break;
                    case 17:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Width";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_Width + "";
                        break;
                    case 18:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Height";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_Height + "";
                        break;
                    case 19:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Angle Rotate";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_AngleRotate + "";
                        break;
                    case 20:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Gray Threshold Min";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_GrayThreshold_Min + "";
                        break;
                    case 21:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Gray Threshold Max";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_GrayThreshold_Max + "";
                        break;
                    case 22:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Pixel Count Min";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_PixelCount_Min + "";
                        break;
                    case 23:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI1 Pixel Count Max";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI1_PixelCount_Max + "";
                        break;
                    // ROI2
                    case 24:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Offset X";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_OffsetX + "";
                        break;
                    case 25:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Offset Y";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_OffsetY + "";
                        break;
                    case 26:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Width";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_Width + "";
                        break;
                    case 27:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Height";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_Height + "";
                        break;
                    case 28:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Angle Rotate";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_AngleRotate + "";
                        break;
                    case 29:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Gray Threshold Min";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_GrayThreshold_Min + "";
                        break;
                    case 30:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Gray Threshold Max";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_GrayThreshold_Max + "";
                        break;
                    case 31:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Pixel Count Min";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_PixelCount_Min + "";
                        break;
                    case 32:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI2 Pixel Count Max";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI2_PixelCount_Max + "";
                        break;
                    // ROI3
                    case 33:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Offset X";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_OffsetX + "";
                        break;
                    case 34:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Offset Y";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_OffsetY + "";
                        break;
                    case 35:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Width";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_Width + "";
                        break;
                    case 36:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Height";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_Height + "";
                        break;
                    case 37:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Angle Rotate";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_AngleRotate + "";
                        break;
                    case 38:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Gray Threshold Min";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_GrayThreshold_Min + "";
                        break;
                    case 39:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Gray Threshold Max";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_GrayThreshold_Max + "";
                        break;
                    case 40:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Pixel Count Min";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_PixelCount_Min + "";
                        break;
                    case 41:
                        recipe.Index = i + 1;
                        recipe.Params = "ROI3 Pixel Count Max";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nROI3_PixelCount_Max + "";
                        break;
                }
                recipeModels.Add(recipe);
            }
            RecipeMapToDataGridModels = recipeModels;
        }
        private string GetEnumDescription(Enum enumObj)
        {
            FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
            if (fieldInfo != null)
            {
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray != null && attribArray.Length > 0 && attribArray[0] is DescriptionAttribute attrib)
                {
                    return attrib.Description;
                }
            }
            return enumObj.ToString();
        }
        private List<string> GetEnumDescriptionToListString()
        {
            List<string> modeTestString = new List<string>();
            List<ECameraList> modeTests = Enum.GetValues(typeof(ECameraList))
                                           .Cast<ECameraList>()
                                           .ToList();

            foreach (var item in modeTests)
            {
                string s = GetEnumDescription(item);
                //if (s.Equals("Null"))
                //    continue;
                modeTestString.Add(s);
            }

            return modeTestString;
        }
        private async void SimulationThread_UpdateUI()
        {
            _settingView.buffSetting.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetSimulatorBuffer(0, 0);

            await _settingView.buffSetting.UpdateImage();
        }

        private async void InspectionComplete(int bSetting)
        {
            if (bSetting == 1)
            {
                InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetInspectionResult(0, ref InterfaceManager.Instance.m_processorManager.m_readCodeResult[0]);

                SettingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetResultBuffer(0, 0);
                await SettingView.buffVSSettings.UpdateImage();

                if (InterfaceManager.Instance.m_processorManager.m_readCodeResult[0].m_bResultStatus == 1)
                {
                    SettingView.buffVSSettings.InspectResult = emInspectResult.InspectResult_OK;
                }
                else
                {
                    SettingView.buffVSSettings.InspectResult = emInspectResult.InspectResult_NG;
                }

                string resStr = InterfaceManager.Instance.m_processorManager.m_readCodeResult[0].m_sResultString;
            }
        }

        private async void LocatorTrained(int bSetting)
        {
            if (bSetting == 1)
            {
                SettingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetResultBuffer(0, 0);
                await SettingView.buffSetting.UpdateImage();

                InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.ReloadRecipe();
                LoadRecipe();
            }
        }
        #endregion

        #region Properties
        public UcSettingView SettingView { get { return _settingView; } }
        public List<SystemSettingsMapToDataGridModel> SystemSettingsMapToDataGridModels
        {
            get => m_sysSettingsMapToDataGridModels;
            set
            {
                if (SetProperty(ref m_sysSettingsMapToDataGridModels, value))
                {

                }
            }
        }
        public List<RecipeMapToDataGridModel> RecipeMapToDataGridModels
        {
            get => m_recipeMapToDataGridModels;
            set
            {
                if (SetProperty(ref m_recipeMapToDataGridModels, value))
                {

                }
            }
        }
        public List<PlcSettingsMapToDataGridModel> PlcSettingsMapToDGModels
        {
            get => m_plcSettingsMapToDGModel;
            set
            {
                if (SetProperty(ref m_plcSettingsMapToDGModel, value)) { }
            }
        }
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
        public CReadCodeRecipe_PropertyGrid ReadCodeRecipePropertyGrid
        {
            get => m_readCodeRecipe_PropertyGrid;
            set => m_readCodeRecipe_PropertyGrid = value;
        }
        public CReadCodeSystemSetting_PropertyGrid ReadCodeSystemSettingsPropertyGrid
        {
            get => m_readCodeSystemSetting_PropertyGrid;
            set => m_readCodeSystemSetting_PropertyGrid = value;
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
                        CameraSelected = ECameraList.Cam1;
                        _settingView.buffVSSettings.CameraIndex = 0;
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);

                    }
                }
            }
        }
        public ECameraList CameraSelected
        {
            get => m_cameraSelected;
            set
            {
                if (SetProperty(ref m_cameraSelected, value))
                {
                    switch (m_cameraSelected)
                    {

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
                        _settingView.cbbCameraList.IsEnabled = false;

                    }
                    else
                    {
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
                        _settingView.cbbCameraList.IsEnabled = true;

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
