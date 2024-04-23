using DinoVisionGUI.Command;
using DinoVisionGUI.Models;
using DinoVisionGUI.Views;
using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NCore.Wpf.UcZoomBoxViewer;

namespace DinoVisionGUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private MainView _mainView;
        private double m_dOpacity = 0.2d;
        private int _cameraIdSelected = -1;
        private CJigInspectConfigurations m_JigInspConfigModel = new CJigInspectConfigurations();
        //IOManagement_PLC_Pana m_IOManagement = new IOManagement_PLC_Pana();
         
        #endregion

        #region All ViewModel
        public SettingsViewModel SettingsViewModel { get; private set; }
        public LoginViewModel LoginViewModel { get; private set; }
        #endregion

        #region SingleTon
        public MainView MainView { get { return _mainView; } private set { } }

        private static MainViewModel _instance;
        public static MainViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region Constructor
        public MainViewModel(Dispatcher dispatcher, MainView mainView,
                             SettingsViewModel settingsViewModel,
                             LoginViewModel loginViewModel)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _mainView = mainView;
            
            // activate Inspect Mode
            // 0: Inspect mode, 1: Live mode, 2: Manual mode, 3: Simulator mode
            //_mainView.ucZoomBoxViewer.MachineModeSelected = _mainView.ucZoomBoxViewer.MachineModeList[2];

            this.SettingsViewModel = settingsViewModel;
            this.LoginViewModel = loginViewModel;

            this.ShowSettingsViewCmd = new ShowSettingsViewCmd(this);
            this.ShowLoginViewCmd = new ShowLoginViewCmd(this);
        }

        #region Properties
        public CJigInspectConfigurations JigInspConfigModel
        {
            get { return m_JigInspConfigModel; }
            set { m_JigInspConfigModel = value; }
        }
        #endregion

        #region event View
        private void SwitchMachineModeHandler(object sender, RoutedEventArgs e)
        {
            switch (_mainView.ucZoomBoxViewer.MachineMode)
            {
                case NCore.Wpf.UcZoomBoxViewer.EMachineMode.EMachineMode_Inspect:
                    //await m_IOManagement.StartInspect();
                    break;
                case NCore.Wpf.UcZoomBoxViewer.EMachineMode.EMachineMode_LiveCam:
                case NCore.Wpf.UcZoomBoxViewer.EMachineMode.EMachineMode_ManualTest:
                case NCore.Wpf.UcZoomBoxViewer.EMachineMode.EMachineMode_Simulator:
                    //await m_IOManagement.StopInspect();
                    break;
            }
        }
        #endregion

        #region Commands
        public ICommand ShowSettingsViewCmd { get; }
        public ICommand InitializeCmd { get; }  
        public ICommand ShowLoginViewCmd { get; }
        #endregion

        #endregion
    }
}
