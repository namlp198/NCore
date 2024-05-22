using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Commons;
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
using NCore.Wpf.BufferViewerSimple;
using SealingInspectGUI.Models;

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

            InterfaceManager.Instance.m_sealingInspectProcessorManager.Initialize();
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);
        }

        ~MainViewModel()
        {
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.DeleteSealingInspectProcessor();
        }
        #endregion

        #region AllViewModel
        private RunViewModel _runVM;
        public RunViewModel RunVM { get => _runVM; private set { } }

        private SettingViewModel _settingVM;
        public SettingViewModel SettingVM { get => _settingVM; private set { } }

        #endregion

        #region Methods
        private void InspectionComplete(emInspectCavity eInspCav)
        {
            int nCoreIdx = 0;
            switch (eInspCav)
            {
                case emInspectCavity.emInspectCavity_Cavity1:
                    nCoreIdx = 0;
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);
                    bool bFrame1Status_Top = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].m_sealingInspResult_TopCam.m_bStatusFrame1 == 1 ? true : false;
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffTopCam1_Frame1, bFrame1Status_Top, 0, 0, "TOP");
                    bool bFrame2Status_Top = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].m_sealingInspResult_TopCam.m_bStatusFrame2 == 1 ? true : false;
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffTopCam1_Frame2, bFrame2Status_Top, 0, 1, "TOP");
                    bool bFrame1Status_Side = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].m_sealingInspResult_SideCam.m_bStatusFrame1 == 1 ? true : false;
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame1Status_Side, 0, 0, "SIDE");
                    bool bFrame2Status_Side = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].m_sealingInspResult_SideCam.m_bStatusFrame2 == 1 ? true : false;
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame2Status_Side, 0, 1, "SIDE");
                    bool bFrame3Status_Side = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].m_sealingInspResult_SideCam.m_bStatusFrame3 == 1 ? true : false;
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame3Status_Side, 0, 2, "SIDE");
                    bool bFrame4Status_Side = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].m_sealingInspResult_SideCam.m_bStatusFrame4 == 1 ? true : false;
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame4Status_Side, 0, 3, "SIDE");
                    break;
                case emInspectCavity.emInspectCavity_Cavity2:
                    nCoreIdx = 1;
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);
                    break;
            }
        }
        private void UpdateResultView(BufferViewerSimple bufferSimple, bool b, int nBuff, int nFrame, string s)
        {
            if (string.Compare(s.ToUpper(), "TOP") == 0)
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_TOP(nBuff, nFrame);
            else
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_SIDE(nBuff, nFrame);
            bufferSimple.InspectResult = (b == true) ? EInspectResult.InspectResult_OK : EInspectResult.InspectResult_NG;
        }
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
