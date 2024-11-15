﻿using Npc.Foundation.Base;
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
using System.Windows.Documents;
using System.Windows;

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
        private eUserLevel m_userLevel = eUserLevel.UserLevel_Operator;
        private bool m_bInspRunning = false;
        private bool m_bEnableSetting = false;
        private string m_sRecipeName = string.Empty;
        private string m_displayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_backward.png";
        private string m_displayImage_LoginStatusPath = "/NpcCore.Wpf;component/Resources/Images/account_2.png";

        private double m_dDistRefer_TopCam_Ring_1 = 0.0;
        private double m_dDistToleranceMin_TopCam_Ring_1 = 0.0;
        private double m_dDistToleranceMax_TopCam_Ring_1 = 0.0;
        private double m_dDistRefer_TopCam_Ring_2 = 0.0;
        private double m_dDistToleranceMin_TopCam_Ring_2 = 0.0;
        private double m_dDistToleranceMax_TopCam_Ring_2 = 0.0;
        #endregion

        #region Properties
        public emMachineMode MachineMode
        {
            get => m_machineMode;
            set
            {
                if (SetProperty(ref m_machineMode, value))
                {

                }
            }
        }
        public eUserLevel UserLevel
        {
            get => m_userLevel;
            set
            {
                if (SetProperty(ref m_userLevel, value))
                {
                    if((m_userLevel == eUserLevel.UserLevel_Admin || m_userLevel == eUserLevel.UserLevel_SuperAdmin) && m_bInspRunning == false)
                    {
                        EnableSetting = true;
                        _mainView.btnSelectRecipe.Opacity = 1.0;
                        _mainView.btnSettings.Opacity = 1.0;
                    }
                    else if(m_userLevel == eUserLevel.UserLevel_Operator)
                    {
                        EnableSetting = false;
                        _mainView.btnSelectRecipe.Opacity = 0.3;
                        _mainView.btnSettings.Opacity = 0.3;
                    }
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

        public string DisplayImage_LoginStatusPath
        {
            get => m_displayImage_LoginStatusPath;
            set
            {
                if (SetProperty(ref m_displayImage_LoginStatusPath, value))
                {

                }
            }
        }

        public bool InspectRunning
        {
            get => m_bInspRunning;
            set
            {
                if (SetProperty(ref m_bInspRunning, value))
                {
                    if(m_bInspRunning == true)
                    {
                        EnableSetting = false;
                        _mainView.btnSelectRecipe.Opacity = 0.3;
                        _mainView.btnSettings.Opacity = 0.3;
                    }
                    else if(m_bInspRunning == false && (m_userLevel == eUserLevel.UserLevel_Admin || m_userLevel == eUserLevel.UserLevel_SuperAdmin))
                    {
                        EnableSetting = true;
                        _mainView.btnSelectRecipe.Opacity = 1.0;
                        _mainView.btnSettings.Opacity = 1.0;
                    }
                    else if(m_bInspRunning == false && m_userLevel == eUserLevel.UserLevel_Operator)
                    {
                        EnableSetting = false;
                        _mainView.btnSelectRecipe.Opacity = 0.3;
                        _mainView.btnSettings.Opacity = 0.3;
                    }
                }
            }
        }
        public bool EnableSetting
        {
            get => m_bEnableSetting;
            set
            {
                if(SetProperty(ref m_bEnableSetting, value))
                {

                }
            }
        }
        public string RecipeName
        {
            get => m_sRecipeName;
            set
            {
                if (SetProperty(ref m_sRecipeName, value))
                {
                   
                }
            }
        }

        public double DistRefer_TopCam_Ring_1
        {
            get => m_dDistRefer_TopCam_Ring_1;
            set => m_dDistRefer_TopCam_Ring_1 = value;
        }
        public double DistToleranceMin_TopCam_Ring_1
        {
            get => m_dDistToleranceMin_TopCam_Ring_1;
            set => m_dDistToleranceMin_TopCam_Ring_1 = value;
        }
        public double DistToleranceMax_TopCam_Ring_1
        {
            get => m_dDistToleranceMax_TopCam_Ring_1;
            set => m_dDistToleranceMax_TopCam_Ring_1 = value;
        }
        public double DistRefer_TopCam_Ring_2
        {
            get => m_dDistRefer_TopCam_Ring_2;
            set => m_dDistRefer_TopCam_Ring_2 = value;
        }
        public double DistToleranceMin_TopCam_Ring_2
        {
            get => m_dDistToleranceMin_TopCam_Ring_2;
            set => m_dDistToleranceMin_TopCam_Ring_2 = value;
        }
        public double DistToleranceMax_TopCam_Ring_2
        {
            get => m_dDistToleranceMax_TopCam_Ring_2;
            set => m_dDistToleranceMax_TopCam_Ring_2 = value;
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
            this.ShowRecipeListCmd = new ShowRecipeListCmd();
            this.ShowLoginViewCmd = new ShowLoginViewCmd();
            this.InitializeCmd = new InitializeCmd();

            InterfaceManager.Instance.m_sealingInspectProcessorManager.Initialize();

            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.
                LoadSystemSettings(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting);
            SettingVM.LoadSystemSettings();
            RecipeName = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_sModelName;

            int nUseSimulation = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_bSimulation;
            if(nUseSimulation == 1)
            {
                RunVM.SumCamVM.SumCameraView.stackManual.Visibility = System.Windows.Visibility.Visible;
                RunVM.SumCamVM.SumCameraView.btnInspSimul.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                RunVM.SumCamVM.SumCameraView.stackManual.Visibility = System.Windows.Visibility.Hidden;
                RunVM.SumCamVM.SumCameraView.btnInspSimul.Visibility = System.Windows.Visibility.Hidden;
            }

            InterfaceManager.Instance.m_sealingInspectProcessorManager.
                m_sealingInspProcessorDll.LoadRecipe(ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe);
            SettingVM.LoadRecipe();

            #region Set TriggerMode and TriggerSource
            int nUseHardwareTrigger_SideCam1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[0].m_recipeFrame1.m_bUseHardwareTrigger;
            if(nUseHardwareTrigger_SideCam1 == 1)
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(2, 1);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(2, 2);
            }
            else if(nUseHardwareTrigger_SideCam1 == 0)
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(2, 0);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(2, 2);
            }

            int nUseHardwareTrigger_SideCam2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.m_sealingInspRecipe_SideCam[1].m_recipeFrame1.m_bUseHardwareTrigger;
            if (nUseHardwareTrigger_SideCam2 == 1)
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(3, 1);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(3, 2);
            }
            else if (nUseHardwareTrigger_SideCam2 == 0)
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerModeHikCam(3, 0);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetTriggerSourceHikCam(3, 2);
            }
            #endregion

            // add refer value and tolerance min max Top Cam
            m_dDistRefer_TopCam_Ring_1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                m_sealingInspRecipe_TopCam[0].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer;
            m_dDistToleranceMin_TopCam_Ring_1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                m_sealingInspRecipe_TopCam[0].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min;
            m_dDistToleranceMax_TopCam_Ring_1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                m_sealingInspRecipe_TopCam[0].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max;
            m_dDistRefer_TopCam_Ring_2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                               m_sealingInspRecipe_TopCam[1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Refer;
            m_dDistToleranceMin_TopCam_Ring_2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                m_sealingInspRecipe_TopCam[1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Min;
            m_dDistToleranceMax_TopCam_Ring_2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectRecipe.
                                                m_sealingInspRecipe_TopCam[1].m_recipeFrame1.m_dDistanceMeasurementTolerance_Max;


            if (InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_bSimulation == 0)
            {
                // start inspect with third param set is 1: on SIMULATOR mode, if don't use SIMULATOR, pass 0 value for this param
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStart(1, emInspectCavity.emInspectCavity_Cavity1, 0);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStart(1, emInspectCavity.emInspectCavity_Cavity2, 0);

                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetInspectStatus(0, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectStatus[0]);
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.GetInspectStatus(1, ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectStatus[1]);

                bool bInspStatus_Cav1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectStatus[0].m_bInspRunning == 1 ? true : false;
                bool bInspStatus_Cav2 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectStatus[1].m_bInspRunning == 1 ? true : false;
                if (bInspStatus_Cav1 == true || bInspStatus_Cav2 == true)
                {
                    InspectRunning = true;
                }
                else
                {
                    InspectRunning = false;
                }

                // Run PLC
                string ipPlc = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_sIPPLC1;
                string ipLightController1 = InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_sIPLightController1;
                int nPortLightController1 = 0;
                int.TryParse(InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_sPortLightController1, out nPortLightController1);

                RunVM.SumCamVM.PLC_Wecon = new Manager.Class.IOManager_PLC_Wecon(ipPlc, 0);
                RunVM.SumCamVM.PLC_Wecon.Initialize();
                RunVM.SumCamVM.PLC_Wecon.StartThreadPlcWecon1();
                RunVM.SumCamVM.PLC_Wecon.StartThreadPlcWecon2();

                RunVM.SumCamVM.LightController_PD3 = new Manager.SumManager.Lighting_Controller_CSS_PD3(ipLightController1, nPortLightController1);
                if (!RunVM.SumCamVM.LightController_PD3.Ping_IP())
                {
                    MessageBox.Show("Can not connect to Light Controller");
                }
                else
                {
                    RunVM.SumCamVM.LightController_PD3.Set_4_Light_0();
                }
            }
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

        #endregion

        #region Command
        public ICommand SelectRunViewCmd { get; }
        public ICommand SelectSettingViewCmd { get; }
        public ICommand SelectMachineModeCmd { get; }
        public ICommand ShowRecipeListCmd { get; }
        public ICommand ShowLoginViewCmd { get; }
        public ICommand InitializeCmd { get; }
        #endregion
    }
}
