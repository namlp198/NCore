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

        #region variables
        private readonly Dispatcher _dispatcher;
        private MainView _mainView;
        public MainView MainView { get => _mainView; private set { } }

        private emMachineMode m_machineMode = emMachineMode.MachineMode_Auto;
        private string m_displayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_backward.png";
        #endregion

        #region Properties
        public emMachineMode MachineMode
        {
            get => m_machineMode;
            set
            {
                if(SetProperty(ref m_machineMode, value))
                {

                }
            }
        }
        public string DisplayImage_MachineModePath
        {
            get => m_displayImage_MachineModePath;
            set
            {
                if (SetProperty(ref m_displayImage_MachineModePath, value))
                {

                }
            }
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
            this.SelectMachineModeCmd = new SelectMachineModeCmd();

            InterfaceManager.Instance.m_sealingInspectProcessorManager.Initialize();
            InterfaceManager.InspectionComplete += new InterfaceManager.InspectionComplete_Handler(InspectionComplete);

            // start inspect with third param set is 1: on SIMULATOR mode, if don't use SIMULATOR, pass 0 value for this param
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStart(1, emInspectCavity.emInspectCavity_Cavity1, 1);
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStart(1, emInspectCavity.emInspectCavity_Cavity2, 1);
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
                    int nStatusFinal_1 = 0;
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                        GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);

                    bool bFrame1Status_Top1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame1 == 1 ? true : false;
                    CheckStatusFinal(bFrame1Status_Top1,ref nStatusFinal_1);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffTopCam1_Frame1, bFrame1Status_Top1, 0, 0, "TOP");

                    bool bFrame2Status_Top1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame2 == 1 ? true : false;
                    CheckStatusFinal(bFrame2Status_Top1, ref nStatusFinal_1);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffTopCam1_Frame2, bFrame2Status_Top1, 0, 1, "TOP");

                    bool bFrame1Status_Side1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame1 == 1 ? true : false;
                    CheckStatusFinal(bFrame1Status_Side1, ref nStatusFinal_1);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame1Status_Side1, 0, 0, "SIDE");

                    bool bFrame2Status_Side1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame2 == 1 ? true : false;
                    CheckStatusFinal(bFrame2Status_Side1, ref nStatusFinal_1);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame2Status_Side1, 0, 1, "SIDE");

                    bool bFrame3Status_Side1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame3 == 1 ? true : false;
                    CheckStatusFinal(bFrame3Status_Side1, ref nStatusFinal_1);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame3Status_Side1, 0, 2, "SIDE");

                    bool bFrame4Status_Side1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame4 == 1 ? true : false;
                    CheckStatusFinal(bFrame4Status_Side1, ref nStatusFinal_1);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam1, bFrame4Status_Side1, 0, 3, "SIDE");

                    if (nStatusFinal_1 > 0) _runVM.ResultVM.InspectionResultFinal_Cavity1 = EInspectResult.InspectResult_NG;
                    else _runVM.ResultVM.InspectionResultFinal_Cavity1 = EInspectResult.InspectResult_OK;

                    break;
                case emInspectCavity.emInspectCavity_Cavity2:
                    nCoreIdx = 1;
                    int nStatusFinal_2 = 0;
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                        GetInspectionResult(nCoreIdx, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx]);

                    bool bFrame1Status_Top2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame1 == 1 ? true : false;
                    CheckStatusFinal(bFrame1Status_Top2, ref nStatusFinal_2);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffTopCam2_Frame1, bFrame1Status_Top2, 1, 0, "TOP");

                    bool bFrame2Status_Top2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_TopCam.m_bStatusFrame2 == 1 ? true : false;
                    CheckStatusFinal(bFrame2Status_Top2, ref nStatusFinal_2);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffTopCam2_Frame2, bFrame2Status_Top2, 1, 1, "TOP");

                    bool bFrame1Status_Side2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                              m_sealingInspResult_SideCam.m_bStatusFrame1 == 1 ? true : false;
                    CheckStatusFinal(bFrame1Status_Side2, ref nStatusFinal_2);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam2, bFrame1Status_Side2, 1, 0, "SIDE");

                    bool bFrame2Status_Side2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame2 == 1 ? true : false;
                    CheckStatusFinal(bFrame2Status_Side2, ref nStatusFinal_2);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam2, bFrame2Status_Side2, 1, 1, "SIDE");

                    bool bFrame3Status_Side2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame3 == 1 ? true : false;
                    CheckStatusFinal(bFrame3Status_Side2, ref nStatusFinal_2);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam2, bFrame3Status_Side2, 1, 2, "SIDE");

                    bool bFrame4Status_Side2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectResult[nCoreIdx].
                                               m_sealingInspResult_SideCam.m_bStatusFrame4 == 1 ? true : false;
                    CheckStatusFinal(bFrame4Status_Side2, ref nStatusFinal_2);
                    UpdateResultView(_runVM.SumCamVM.SumCameraView.buffSideCam2, bFrame4Status_Side2, 1, 3, "SIDE");

                    if (nStatusFinal_2 > 0) _runVM.ResultVM.InspectionResultFinal_Cavity2 = EInspectResult.InspectResult_NG;
                    else _runVM.ResultVM.InspectionResultFinal_Cavity2 = EInspectResult.InspectResult_OK;

                    break;
            }
        }
        private async void UpdateResultView(BufferViewerSimple bufferSimple, bool b, int nBuff, int nFrame, string s)
        {
            if (string.Compare(s.ToUpper(), "TOP") == 0)
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_TOP(nBuff, nFrame);
            else if(string.Compare(s.ToUpper(), "SIDE") == 0)
                bufferSimple.BufferView = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetBufferImage_SIDE(nBuff, nFrame);
            await bufferSimple.UpdateImage();

            bufferSimple.InspectResult = (b == true) ? EInspectResult.InspectResult_OK : EInspectResult.InspectResult_NG;
        }
        private void CheckStatusFinal(bool bStatus, ref int nStatusFinal)
        {
            if (bStatus == false) nStatusFinal++;
        }
        #endregion

        #region Command
        public ICommand SelectRunViewCmd { get; }
        public ICommand SelectSettingViewCmd { get; }
        public ICommand SelectMachineModeCmd {  get; }
        #endregion
    }
}
