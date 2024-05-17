using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Manager;
using SealingInspectGUI.Views;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace SealingInspectGUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region SingleTon
        private static MainViewModel _instance;
        public static MainViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region Constructor
        public MainViewModel(Dispatcher dispatcher, MainView mainView, 
                             RunViewModel runVM, SettingViewModel settingVM)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _mainView = mainView;
            _runVM = runVM;
            _settingVM = settingVM;

            this.SelectRunViewCmd = new SelectRunViewCmd();
            this.SelectSettingViewCmd = new SelectSettingViewCmd();

            InterfaceManager.Instance.m_sealingInspProcessor.Initialize();
        }

        ~MainViewModel()
        {
            InterfaceManager.Instance.m_sealingInspProcessor.DeleteSealingInspectProcessor();
        }
        #endregion

        #region AllViewModel
        private RunViewModel _runVM;
        public RunViewModel RunVM { get => _runVM; private set { } }

        private SettingViewModel _settingVM;
        public SettingViewModel SettingVM { get => _settingVM; private set { } }

        #endregion

        #region variables
        private readonly Dispatcher _dispatcher;
        private MainView _mainView;
        public MainView MainView { get => _mainView; private set { } }
        #endregion

        #region Command
        public ICommand SelectRunViewCmd { get; }
        public ICommand SelectSettingViewCmd { get; }

        #endregion
    }
}
