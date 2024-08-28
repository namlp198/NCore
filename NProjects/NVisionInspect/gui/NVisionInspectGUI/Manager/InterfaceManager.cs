using NVisionInspectGUI.Manager.SumManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Manager
{
    public class InterfaceManager
    {
        public delegate void InspectionComplete_Handler(int bSetting);
        public static event InspectionComplete_Handler InspectionComplete;

        public delegate void LocatorTrained_Handler();
        public static event LocatorTrained_Handler LocatorTrained;

        public NVisionInspectProcessorManager m_processorManager = new NVisionInspectProcessorManager();
        public SimulationThread m_simulationThread = new SimulationThread();

        #region Singleton
        private static InterfaceManager _instance;
        public static InterfaceManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new InterfaceManager();
                return _instance;
            }
            private set { }
        }
        #endregion

        public void CallbackInsCompleteFunc(int bSetting)
        {
            InspectionComplete(bSetting);
        }

        public void CallbackLocatorTrainedFunc()
        {
            LocatorTrained();
        }
    }
}
