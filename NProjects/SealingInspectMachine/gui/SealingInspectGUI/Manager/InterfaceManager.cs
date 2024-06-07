using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager.Class;
using SealingInspectGUI.Manager.SumManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Manager
{
    public class InterfaceManager
    {
        public delegate void InspectionCavity1Complete_Handler(int bSetting);
        public static event InspectionCavity1Complete_Handler InspectionCavity1Complete;

        public delegate void InspectionCavity2Complete_Handler(int bSetting);
        public static event InspectionCavity2Complete_Handler InspectionCavity2Complete;

        public delegate void InspectionTopCam1Complete_Handler(int bSetting);
        public static event InspectionTopCam1Complete_Handler InspectionTopCam1Complete;

        public delegate void InspectionTopCam2Complete_Handler(int bSetting);
        public static event InspectionTopCam2Complete_Handler InspectionTopCam2Complete;

        public SimulationThread m_simulationThread = new SimulationThread();
        public SealingInspectProcessorManager m_sealingInspectProcessorManager = new SealingInspectProcessorManager();

        #region Singleton
        private static InterfaceManager _instance;
        public static InterfaceManager Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new InterfaceManager();
                return _instance;
            }
            private set { }
        }
        #endregion

        public void CallbackInsCavity1CompleteFunc(int bSetting)
        {
            InspectionCavity1Complete(bSetting);

            //InspectionComplete_All_Check();
        }
        public void CallbackInsCavity2CompleteFunc(int bSetting)
        {
            InspectionCavity2Complete(bSetting);

            //InspectionComplete_All_Check();
        }
        public void CallbackInsTopCam1CompleteFunc(int bSetting)
        {
            InspectionTopCam1Complete(bSetting);
        }
        public void CallbackInsTopCam2CompleteFunc(int bSetting)
        {
            InspectionTopCam2Complete(bSetting);
        }

        object m_csInspectionComplete = new object();
        public delegate void Inspection_All_Complete_Handler();
        public static event Inspection_All_Complete_Handler Inspection_All_Complete;
        public void InspectionComplete_All_Check()
        {
            Inspection_All_Complete?.Invoke();
        }
    }
}
