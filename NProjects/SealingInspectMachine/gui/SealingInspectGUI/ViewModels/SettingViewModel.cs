using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
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
        [Description("Side Cam 1")]
        SideCam1,
        [Description("Top Cam 2")]
        TopCam2,
        [Description("Side Cam 2")]
        SideCam2
    }
    public class SettingViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSettingView _settingView;

        private bool _bStreamming = false;
        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        private string[] m_arrFrameOfTOP = new string[Defines.MAX_IMAGE_BUFFER_TOP / 2] { "1", "2" };
        private string[] m_arrFrameOfSIDE = new string[Defines.MAX_IMAGE_BUFFER_SIDE / 2] { "1", "2", "3", "4" };

        private List<string> m_cameraLst = new List<string>();
        private List<string> m_frameList = new List<string>();
        private string m_strCameraSelected = string.Empty;
        private string m_strFrameSelected = string.Empty;
        private ECameraList m_cameraSelected = new ECameraList();
        private int m_nBuffIdx = 0;

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
            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
        }

        private void SimulationThread_UpdateUI(int nBuff)
        {
            _dispatcher.BeginInvoke(new Action(async () =>
            {
                if (CameraSelected == ECameraList.TopCam1 ||
                    CameraSelected == ECameraList.TopCam2)
                    _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_TOP(m_nBuffIdx, 0);

                else if (CameraSelected == ECameraList.SideCam1 ||
                        CameraSelected == ECameraList.SideCam2)
                    _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_SIDE(m_nBuffIdx, 0);

                await _settingView.buffVSSettings.UpdateImage();
            }));
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
                    if (string.Compare("Top Cam 1", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.TopCam1;
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOP_CAM, Defines.FRAME_HEIGHT_TOP_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;
                    }
                    else if (string.Compare("Side Cam 1", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam1;
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;
                    }
                    else if (string.Compare("Top Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.TopCam2;
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_TOP_CAM, Defines.FRAME_HEIGHT_TOP_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfTOP);
                        FrameList = list;
                    }
                    else if (string.Compare("Side Cam 2", m_strCameraSelected) == 0)
                    {
                        CameraSelected = ECameraList.SideCam2;
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH_SIDE_CAM, Defines.FRAME_HEIGHT_SIDE_CAM);

                        List<string> list = new List<string>();
                        list.AddRange(m_arrFrameOfSIDE);
                        FrameList = list;
                    }
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
                    if (m_cameraSelected == ECameraList.TopCam1 ||
                        m_cameraSelected == ECameraList.SideCam1)
                    {
                        int.TryParse(m_strFrameSelected, out m_nBuffIdx);
                        m_nBuffIdx -= 1;
                    }
                    else if (m_cameraSelected == ECameraList.TopCam2 ||
                           m_cameraSelected == ECameraList.SideCam2)
                    {
                        int.TryParse(m_strFrameSelected, out m_nBuffIdx);
                        m_nBuffIdx = (2 * m_nBuffIdx) - 1;
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
                    }
                    else
                    {
                        DisplayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
                    }
                }
            }
        }

        #endregion

        #region Methods
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
        #endregion
    }
}
