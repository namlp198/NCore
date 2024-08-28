using ReadCodeGUI.Manager.SumManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadCodeGUI.Manager
{
    public class InterfaceManager
    {
        public delegate void InspectionComplete_Handler(int bSetting);
        public static event InspectionComplete_Handler InspectionComplete;

        public delegate void LocatorTrained_Handler(int bSetting);
        public static event LocatorTrained_Handler LocatorTrained;

        public ReadCodeProcessorManager m_processorManager = new ReadCodeProcessorManager();
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

        public void CallbackLocatorTrainedFunc(int bSetting)
        {
            LocatorTrained(bSetting);
        }
    }
}
