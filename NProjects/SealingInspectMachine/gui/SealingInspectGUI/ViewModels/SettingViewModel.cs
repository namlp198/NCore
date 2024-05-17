using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SealingInspectGUI.ViewModels
{
    public class SettingViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcSettingView _settingView;

        private bool _bStreamming = false;
        private bool m_bUseColor = true;

        private string _displayImagePath = "/NpcCore.Wpf;component/Resources/Images/live_camera.png";
        #endregion

        #region Constructor
        public SettingViewModel(Dispatcher dispatcher, UcSettingView settingView)
        {
            _dispatcher = dispatcher;
            _settingView = settingView;

            _settingView.buffVSSettings.CameraIndex = 99;

            if (m_bUseColor)
            {
                _settingView.buffVSSettings.ModeView = NCore.Wpf.BufferViewerSimple.ModeView.Color;
                _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);
            }
            else
            {
                _settingView.buffVSSettings.ModeView = NCore.Wpf.BufferViewerSimple.ModeView.Mono;
                _settingView.buffVSSettings.FrameWidth = Defines.FRAME_WIDTH;
                _settingView.buffVSSettings.FrameHeight = Defines.FRAME_HEIGHT;
            }

            this.LoadImageCmd = new LoadImageCmd();
            SimulationThread.UpdateUI += SimulationThread_UpdateUI;
        }

        private void SimulationThread_UpdateUI(int nBuff)
        {
            _dispatcher.BeginInvoke(new Action(async () =>
            {
                if (m_bUseColor)
                    _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_Color(0, 0);
                else
                    _settingView.buffVSSettings.BufferView = InterfaceManager.Instance.m_sealingInspProcessor.GetBufferImage_Mono(0, 0);
                await _settingView.buffVSSettings.UpdateImage();
            }));
        }
        #endregion

        #region Properties
        public UcSettingView SettingView { get { return _settingView; } }

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

        public bool UseColor
        {
            get => m_bUseColor;
            set
            {
                if (SetProperty(ref m_bUseColor, value))
                {
                    if(m_bUseColor)
                    {
                        _settingView.buffVSSettings.ModeView = NCore.Wpf.BufferViewerSimple.ModeView.Color;
                        _settingView.buffVSSettings.SetParamsModeColor(Defines.FRAME_WIDTH, Defines.FRAME_HEIGHT);
                    }
                    else
                    {
                        _settingView.buffVSSettings.ModeView = NCore.Wpf.BufferViewerSimple.ModeView.Mono;
                        _settingView.buffVSSettings.FrameWidth = Defines.FRAME_WIDTH;
                        _settingView.buffVSSettings.FrameHeight = Defines.FRAME_HEIGHT;
                    }
                }
            }
        }
      
        #endregion

        #region Command
        public ICommand LoadImageCmd { get; }
        #endregion
    }
}
