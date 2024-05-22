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
        public delegate void InspectionComplete_Handler(emInspectCavity eInspCavity);
        public static event InspectionComplete_Handler InspectionComplete;

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

        public void CallbackInsCompleteFunc(emInspectCavity eInspCavity)
        {
            InspectionComplete(eInspCavity);

            InspectionComplete_All_Check(eInspCavity);
        }

        object m_csInspectionComplete = new object();
        public delegate void Inspection_All_Complete_Handler();
        public static event Inspection_All_Complete_Handler Inspection_All_Complete;
        public void InspectionComplete_All_Check(emInspectCavity eInspCav)
        {
            Inspection_All_Complete?.Invoke();
        }
    }
}
