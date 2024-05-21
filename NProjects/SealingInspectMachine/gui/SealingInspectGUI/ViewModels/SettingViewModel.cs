using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.Manager.Class;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string[] m_arrFrameOfTOP = new string[Defines.MAX_IMAGE_BUFFER_TOP / 2] { "1", "2" };
        private string[] m_arrFrameOfSIDE = new string[Defines.MAX_IMAGE_BUFFER_SIDE / 2] { "1", "2", "3", "4" };

        private List<string> m_cameraLst = new List<string>();
        private List<string> m_frameList = new List<string>();
        private string m_strCameraSelected = string.Empty;
        private string m_strFrameSelected = string.Empty;
        private ECameraList m_cameraSelected = new ECameraList();
        private int m_nBuffIdx = 0;
        private int m_nFrame = 0;

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

            SimulationThread.UpdateUI += SimulationThread_UpdateUI;

            m_cameraStreamingController = new CameraStreamingController(_settingView.buffVSSettings.FrameWidth,
                                                                        _settingView.buffVSSettings.FrameHeight,
                                                                        _settingView.buffVSSettings,
                                                                        _settingView.buffVSSettings.ModeView);
        }

        private void SimulationThread_UpdateUI()
        {
            if (CameraSelected == ECameraList.TopCam1 ||
                CameraSelected == ECameraList.TopCam2)
                _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_TOP(m_nBuffIdx, m_nFrame - 1);

            else if (CameraSelected == ECameraList.SideCam1 ||
                CameraSelected == ECameraList.SideCam2)
                _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_SIDE(m_nBuffIdx, m_nFrame - 1);
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

        #endregion

        #region Methods
        private void UpdateParamSoftwareTrigger()
        {
            if (m_bUseSoftwareTrigger)
            {
                InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerModeHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerMode.TriggerMode_External);
                InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerSourceHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerSource.TriggerSource_Software);
            }
            else
            {
                InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerModeHikCam(_settingView.buffVSSettings.CameraIndex,
                                                                                      (int)eTriggerMode.TriggerMode_Internal);
                InterfaceManager.Instance.m_sealingInspProcessor.SetTriggerSourceHikCam(_settingView.buffVSSettings.CameraIndex,
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
        #endregion

        #region Command
        public ICommand LoadImageCmd { get; }
        public ICommand ContinuousGrabCmd { get; }
        public ICommand SoftwareTriggerHikCamCmd { get; }
        #endregion
    }
}
