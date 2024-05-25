using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.Manager.Class;
using SealingInspectGUI.Models;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SealingInspectGUI.ViewModels
{
    public enum ECameraList
    {
        [Description("Top Cam 1")]
        TopCam1,
        [Description("Top Cam 2")]
        TopCam2,
        [Description("Side Cam 1")]
        SideCam1,
        [Description("Side Cam 2")]
        SideCam2
    }
    public class SettingViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSettingView _settingView;

        private bool _bStreamming = false;
        private bool m_bUseSoftwareTrigger = false;
        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private string[] m_arrFrameOfTOP = new string[Defines.MAX_IMAGE_BUFFER_TOPCAM] { "1", "2" };
        private string[] m_arrFrameOfSIDE = new string[Defines.MAX_IMAGE_BUFFER_SIDECAM] { "1", "2", "3", "4" };

        private List<string> m_cameraLst = new List<string>();
        private List<string> m_frameList = new List<string>();
        private string m_strCameraSelected = string.Empty;
        private string m_strFrameSelected = string.Empty;
        private ECameraList m_cameraSelected = new ECameraList();
        private int m_nBuffIdx = 0;
        private int m_nFrame = 0;

        private List<SettingsMapToDataGridModel> m_systemSettingsModels = new List<SettingsMapToDataGridModel>();
        private List<SettingsMapToDataGridModel> m_lightSettingModel1 = new List<SettingsMapToDataGridModel>();
        private List<SettingsMapToDataGridModel> m_lightSettingModel2 = new List<SettingsMapToDataGridModel>();

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
            _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_HEIGHT_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

            this.LoadImageCmd = new LoadImageCmd();
            this.ContinuousGrabCmd = new ContinuousGrabCmd();
            this.SoftwareTriggerHikCamCmd = new SoftwareTriggerHikCamCmd();
            this.SaveSettingCmd = new SaveSettingCmd();

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;

            m_cameraStreamingController = new CameraStreamingController(_settingView.buffVSSettings.FrameWidth,
                                                                        _settingView.buffVSSettings.FrameHeight,
                                                                        _settingView.buffVSSettings,
                                                                        _settingView.buffVSSettings.ModeView);
        }

        private async void SimulationThread_UpdateUI()
        {
            if (CameraSelected == ECameraList.TopCam1 ||
                CameraSelected == ECameraList.TopCam2)
                _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_TOP(m_nBuffIdx, m_nFrame - 1);

            else if (CameraSelected == ECameraList.SideCam1 ||
                CameraSelected == ECameraList.SideCam2)
                _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_SIDE(m_nBuffIdx, m_nFrame - 1);

            await _settingView.buffVSSettings.UpdateImage();
        }
        #endregion

        #region Properties
        public UcSettingView SettingView { get { return _settingView; } }

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
        public List<string> FrameList
        {
            get => m_frameList;
            set
            {
                if (SetProperty(ref m_frameList, value))
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
                    m_cameraStreamingController.StopSingleGrab(CameraType.Hik);

                    if (string.Compare("Top Cam 1", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.TopCam1;
                        _settingView.buffVSSettings.CameraIndex = 0; // 0: Index of Top Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOP_CAM, Defines.FRAME_HEIGHT_TOP_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;
                    }
                    else if (string.Compare("Side Cam 1", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam1;
                        _settingView.buffVSSettings.CameraIndex = 2; // 2: Index of Side Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;
                    }
                    else if (string.Compare("Top Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.TopCam2;
                        _settingView.buffVSSettings.CameraIndex = 1; // 1: Index of Top Cam 2
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOP_CAM, Defines.FRAME_HEIGHT_TOP_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;
                    }
                    else if (string.Compare("Side Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam2;
                        _settingView.buffVSSettings.CameraIndex = 3; // 3: Index of Side Cam 1
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;
                    }

                    UpdateParamSoftwareTrigger();
                }
            }
        }
        public string StrFrameSelected
        {
            get => m_strFrameSelected;
            set
            {
                if (SetProperty(ref m_strFrameSelected, value))
                {
                    int.TryParse(m_strFrameSelected, out m_nFrame);
                }
            }
        }
        public int Frame
        {
            get => m_nFrame;
            set
            {
                if (SetProperty(ref m_nFrame, value))
                {

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
                        case ECameraList.TopCam1:
                        case ECameraList.SideCam1:
                            m_nBuffIdx = 0;
                            break;
                        case ECameraList.TopCam2:
                        case ECameraList.SideCam2:
                            m_nBuffIdx = 1;
                            break;
                    }
                }
            }
        }
        public int BuffIdx
        {
            get => m_nBuffIdx;
            set
            {
                if (SetProperty(ref m_nBuffIdx, value))
                {

                }
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
        public bool IsStreamming
        {
            get => _bStreamming;
            set
            {
                if (SetProperty(ref _bStreamming, value))
                {
                    if (_bStreamming)
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
        public bool UseSoftwareTrigger
        {
            get => m_bUseSoftwareTrigger;
            set
            {
                if (SetProperty(ref m_bUseSoftwareTrigger, value))
                {
                    UpdateParamSoftwareTrigger();
                }
            }
        }
        public List<SettingsMapToDataGridModel> SystemSettingsModels
        {
            get => m_systemSettingsModels;
            set
            {
                if (SetProperty(ref m_systemSettingsModels, value))
                {

                }
            }
        }
        public List<SettingsMapToDataGridModel> LightSettingModel1
        {
            get => m_lightSettingModel1;
            set
            {
                if (SetProperty(ref m_lightSettingModel1, value))
                {

                }
            }
        }
        public List<SettingsMapToDataGridModel> LightSettingModel2
        {
            get => m_lightSettingModel2;
            set
            {
                if (SetProperty(ref m_lightSettingModel2, value))
                {

                }
            }
        }
        #endregion

        #region Methods
        private void UpdateParamSoftwareTrigger()
        {
            if (m_bUseSoftwareTrigger)
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerMode.TriggerMode_External);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerSource.TriggerSource_Software);
            }
            else
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerMode.TriggerMode_Internal);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerSource.TriggerSource_Hardware);
            }
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

        public void LoadSystemSettings()
        {
            List<SettingsMapToDataGridModel> sysSettingLst = new List<SettingsMapToDataGridModel>();
            int nPropertyCount = typeof(CSealingInspectSystemSetting).GetFields().Count();
            string value = string.Empty;
            // Don't care to CSealingInspectLightSetting: nPropertyCount - 1
            for (int i = 0; i < nPropertyCount - 1; i++)
            {
                SettingsMapToDataGridModel sysSetting = new SettingsMapToDataGridModel();
                sysSetting.Index = i + 1;
                sysSetting.Params = GetParamNameAndValue_SystemSetting(i, ref value);
                sysSetting.Value = value;
                sysSettingLst.Add(sysSetting);
            }
            SystemSettingsModels = sysSettingLst;

            LoadLightSettings();
        }
        private void LoadLightSettings()
        {
            int nPropertyCount = typeof(CSealingInspectLightSetting).GetFields().Count();
            string value = string.Empty;
            for (int lightSettingIdx = 0; lightSettingIdx < Defines.NUMBER_OF_LIGHT_CONTROLLER; lightSettingIdx++)
            {
                List<SettingsMapToDataGridModel> lightSettingLst = new List<SettingsMapToDataGridModel>();
                for (int i = 1; i <= nPropertyCount; i++)
                {
                    SettingsMapToDataGridModel lightSetting = new SettingsMapToDataGridModel();
                    lightSetting.Index = i;
                    lightSetting.Params = GetParamNameAndValue_LightSetting(i, ref value, lightSettingIdx);
                    lightSetting.Value = value;
                    lightSettingLst.Add(lightSetting);
                }
                if (lightSettingIdx == 0)
                    LightSettingModel1 = lightSettingLst;
                else if (lightSettingIdx == 1)
                    LightSettingModel2 = lightSettingLst;
            }
        }
        private string GetParamNameAndValue_SystemSetting(int idx, ref string value)
        {
            switch (idx)
            {
                case 0:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC1;
                    return "IP PLC1";
                case 1:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC2;
                    return "IP PLC2";
                case 2:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC1;
                    return "PORT PLC1";
                case 3:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC2;
                    return "PORT PLC2";
                case 4:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController1;
                    return "IP Light Controller 1";
                case 5:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController2;
                    return "IP Light Controller 2";
                case 6:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController1;
                    return "PORT Light Controller 1";
                case 7:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController2;
                    return "PORT Light Controller 2";
                case 8:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bSaveFullImage + "";
                    return "Save Full Image (0: No, 1: Yes)";
                case 9:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bSaveDefectImage + "";
                    return "Save Defect Image (0: No, 1: Yes)";

                case 10:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bShowDetailImage + "";
                    return "Show Detail Image (0: No, 1: Yes)";

                case 11:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bSimulation + "";
                    return "Simulation (0: No, 1: Yes)";

                case 12:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_bByPass + "";
                    return "By Pass (0: No, 1: Yes)";

                case 13:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sFullImagePath;
                    return "Full Image Path";

                case 14:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sDefectImagePath;
                    return "Defect Image Path";

                case 15:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sModelName;
                    return "Model Name";
            }
            return "";
        }
        private string GetParamNameAndValue_LightSetting(int idx, ref string value, int lightSettingIdx)
        {
            switch (idx)
            {
                case 1:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH1;
                    return "CH1";
                case 2:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH2;
                    return "CH2";
                case 3:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH3;
                    return "CH3";
                case 4:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH4;
                    return "CH4";
                case 5:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH5;
                    return "CH5";
                case 6:
                    value = InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_LightSettings[lightSettingIdx].m_sCH6;
                    return "CH6";
            }
            return value;
        }
        public void SetValue_SystemSetting()
        {
            for (int i = 0; i < SystemSettingsModels.Count; i++)
            {
                string value = SystemSettingsModels[i].Value;
                switch (i)
                {
                    case 0:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC1 = value;
                        break;
                    case 1:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPPLC2 = value;
                        break;
                    case 2:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC1 = value;
                        break;
                    case 3:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortPLC2 = value;
                        break;
                    case 4:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController1 = value;
                        break;
                    case 5:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sIPLightController2 = value;
                        break;
                    case 6:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController1 = value;
                        break;
                    case 7:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sPortLightController2 = value;
                        break;
                    case 8:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bSaveFullImage);
                        break;
                    case 9:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bSaveDefectImage);
                        break;
                    case 10:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bShowDetailImage);
                        break;
                    case 11:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bSimulation);
                        break;
                    case 12:
                        int.TryParse(value, out InterfaceManager.Instance.m_sealingInspectProcessorManager.
                                     m_sealingInspectSysSetting.m_bByPass);
                        break;
                    case 13:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sFullImagePath = value;
                        break;
                    case 14:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sDefectImagePath = value;
                        break;
                    case 15:
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.
                            m_sealingInspectSysSetting.m_sModelName = value;
                        break;
                }
            }
        }
        public void SetValue_LightSetting(int lightIdx)
        {
            if(lightIdx == 0)
            {
                for (int i = 0; i < LightSettingModel1.Count; i++)
                {
                    string value = LightSettingModel1[i].Value;
                    switch (i)
                    {
                        case 0:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH1 = value;
                            break;
                        case 1:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH2 = value;
                            break;
                        case 2:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH3 = value;
                            break;
                        case 3:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH4 = value;
                            break;
                        case 4:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH5 = value;
                            break;
                        case 5:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH6 = value;
                            break;
                    }
                }
            }
            else if(lightIdx == 1) 
            {
                for (int i = 0; i < LightSettingModel2.Count; i++)
                {
                    string value = LightSettingModel2[i].Value;
                    switch (i)
                    {
                        case 0:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH1 = value;
                            break;
                        case 1:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH2 = value;
                            break;
                        case 2:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH3 = value;
                            break;
                        case 3:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH4 = value;
                            break;
                        case 4:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH5 = value;
                            break;
                        case 5:
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                           m_sealingInspectSysSetting.m_LightSettings[lightIdx].m_sCH6 = value;
                            break;
                    }
                }
            }
        }
        #endregion

        #region Command
        public ICommand LoadImageCmd { get; }
        public ICommand ContinuousGrabCmd { get; }
        public ICommand SoftwareTriggerHikCamCmd { get; }
        public ICommand SaveSettingCmd { get; }
        #endregion
    }
}
