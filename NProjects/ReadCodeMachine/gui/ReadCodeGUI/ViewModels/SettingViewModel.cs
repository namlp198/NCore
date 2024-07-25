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
        private List<SystemSettingsMapToDataGridModel> m_sysSettingsMapToDataGridModels = new List<SystemSettingsMapToDataGridModel>();
        private List<RecipeMapToDataGridModel> m_recipeMapToDataGridModels = new List<RecipeMapToDataGridModel>();
        private List<PlcSettingsMapToDataGridModel> m_plcSettingsMapToDGModel = new List<PlcSettingsMapToDataGridModel>();

        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private List<string> m_cameraLst = new List<string>();
        private bool m_bStreamming = false;
        public CameraStreamingController m_cameraStreamingController = null;
        #endregion

        #region Constructor
        public SettingViewModel(Dispatcher dispatcher, UcSettingView settingView)
        {
            _dispatcher = dispatcher;
            _settingView = settingView;

            CameraList = GetEnumDescriptionToListString();

            _settingView.buffVSSettings.CameraIndex = 99;
            _settingView.buffVSSettings.ModeView = NCore.Wpf.BufferViewerSimple.ModeView.Color;
            _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);

            this.SaveRecipeCmd = new SaveRecipeCmd();
            this.SaveSettingCmd = new SaveSettingCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.SingleGrabCmd = new SingleGrabCmd();
            this.LoadImageCmd = new LoadImageCmd();

            m_xmlManagement.Load(Defines.StartupProgPath + "\\Settings\\PlcSettings.config");

            m_cameraStreamingController = new CameraStreamingController(_settingView.buffVSSettings.FrameWidth,
                                                                        _settingView.buffVSSettings.FrameHeight,
                                                                        _settingView.buffVSSettings,
                                                                        _settingView.buffVSSettings.ModeView);

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
        }
        #endregion

        #region Methods

        // Load System Setting
        public void LoadSystemSettings()
        {
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
                                plcSetting.Params = "Plc COM";
                                break;
                            case 1:
                                plcSetting.Params = "Trigger Delay (ms)";
                                break;
                            case 2:
                                plcSetting.Params = "Signal NG Delay (ms)";
                                break;
                            case 3:
                                plcSetting.Params = "Register Trigger Delay";
                                break;
                            case 4:
                                plcSetting.Params = "Register Output 1 Delay";
                                break;
                        }
                        plcSetting.Index = i + 1;
                        plcSetting.Value = nodeList[i].InnerText;

                        plcSettings.Add(plcSetting);
                    }
                }
            }

            PlcSettingsMapToDGModels = plcSettings;
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
                        break;
                    case 1:
                        XmlNode triggerDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//TriggerDelay");
                        triggerDelayNode.InnerText = value;
                        break;
                    case 2:
                        XmlNode signalNGDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//SignalNGDelay");
                        signalNGDelayNode.InnerText = value;
                        break;
                    case 3:
                        XmlNode registerTriggerDelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//RegisterTriggerDelay");
                        registerTriggerDelayNode.InnerText = value;
                        break;
                    case 4:
                        XmlNode registerOutput1DelayNode = m_xmlManagement.SelectSingleNode("//PlcSettings//RegisterOutput1Delay");
                        registerOutput1DelayNode.InnerText = value;
                        break;
                }
            }

            m_xmlManagement.Save(Defines.StartupProgPath + "\\Settings\\PlcSettings.config");
        }

        // Load Recipe
        public void LoadRecipe()
        {
            List<RecipeMapToDataGridModel> recipeModels = new List<RecipeMapToDataGridModel>();

            int nPropertyCount = typeof(CReadCodeRecipe).GetFields().Count();

            for (int i = 0; i < nPropertyCount; i++)
            {
                RecipeMapToDataGridModel recipe = new RecipeMapToDataGridModel();
                switch (i)
                {
                    case 0:
                        recipe.Index = i + 1;
                        recipe.Params = "Max Code Count";
                        recipe.Value = InterfaceManager.Instance.m_processorManager.m_readCodeRecipe.m_nMaxCodeCount + "";
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
            _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.GetSimulatorBuffer(0, 0);

            await _settingView.buffVSSettings.UpdateImage();
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

        #endregion
    }
}
