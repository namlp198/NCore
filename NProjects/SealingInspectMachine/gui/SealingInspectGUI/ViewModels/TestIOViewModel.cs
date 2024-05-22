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
using System.Windows.Threading;

namespace SealingInspectGUI.ViewModels
{
    public class TestIOViewModel : ViewModelBase
    {
        #region SingleTon
        private static TestIOViewModel _instance;
        public static TestIOViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region variables
        private readonly Dispatcher _dispatcher;
        private TestIOView _testIOView;

        private bool m_bCavity1_Ring;
        private bool m_bCavity1_4Bar;
        private bool m_bCavity1_Frame1;
        private bool m_bCavity1_Frame2;
        private bool m_bCavity1_Frame3;
        private bool m_bCavity1_Frame4;
        private bool m_bCavity1_NG;
        #endregion

        #region Constructor
        public TestIOViewModel(Dispatcher dispatcher, TestIOView testIOView)
        {
            // construct a instance of TestIOViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _testIOView = testIOView;

            TestIOResetAllCmd = new TestIOResetAllCmd();
        }
        #endregion

        #region Properties
        public TestIOView TestIOView { get { return _testIOView; } }

        public bool UseCavity1_Ring
        {
            get => m_bCavity1_Ring;
            set
            {
                if (SetProperty(ref m_bCavity1_Ring, value)) 
                {
                    if (m_bCavity1_Ring)
                    {
                        int nCavityIdx = 0;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bRing = 1;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                    }
                }
            }
        }
        public bool UseCavity1_4Bar
        {
            get => m_bCavity1_4Bar;
            set
            {
                if (SetProperty(ref m_bCavity1_4Bar, value)) 
                {
                    if (m_bCavity1_4Bar)
                    {
                        int nCavityIdx = 0;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_b4Bar = 1;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                    }
                }
            }
        }

        public bool UseCavity1_Frame1
        {
            get => m_bCavity1_Frame1;
            set
            {
                if (SetProperty(ref m_bCavity1_Frame1, value)) 
                {
                    if (m_bCavity1_Frame1)
                    {
                        int nCavityIdx = 0;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bFrame1 = 1;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                    }
                }
            }
        }

        public bool UseCavity1_Frame2
        {
            get => m_bCavity1_Frame2;
            set
            {
                if (SetProperty(ref m_bCavity1_Frame2, value)) 
                {
                    if (m_bCavity1_Frame2)
                    {
                        if (m_bCavity1_Frame3)
                        {
                            int nCavityIdx = 0;
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bFrame2 = 1;
                            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                                ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                        }

                    }
                }
            }
        }
        public bool UseCavity1_Frame3
        {
            get => m_bCavity1_Frame3;
            set
            {
                if (SetProperty(ref m_bCavity1_Frame3, value)) 
                {
                    if (m_bCavity1_Frame3)
                    {
                        int nCavityIdx = 0;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bFrame3 = 1;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                    }
                }
            }
        }
        public bool UseCavity1_Frame4
        {
            get => m_bCavity1_Frame4;
            set
            {
                if (SetProperty(ref m_bCavity1_Frame4, value)) 
                {
                    if (m_bCavity1_Frame4)
                    {
                        int nCavityIdx = 0;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bFrame4 = 1;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                    }
                }
            }
        }
        public bool Cavity1_NG
        {
            get => m_bCavity1_NG;
            set
            {
                if(SetProperty(ref m_bCavity1_NG, value))
                {
                    int nCavityIdx = 0;
                    if (m_bCavity1_NG)
                    {   
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bNG = 1;
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                    }
                    //else
                    //{
                    //    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bNG = 0;
                    //    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                    //        ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                    //}
                }
            }
        }

        #endregion

        #region Commands
        public ICommand TestIOResetAllCmd { get; }
        #endregion
    }
}
