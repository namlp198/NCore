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
        public delegate void WriteLog_Handler(string log);
        public static event WriteLog_Handler WriteLogEvent;

        public delegate void Alarm_Handler(string alarm);
        public static event Alarm_Handler AlarmEvent;

        public delegate void InspectionComplete_Handler(int bSetting);
        public static event InspectionComplete_Handler InspectionComplete;

        public delegate void LocatorTrainComplete_Handler(int nCamIdx);
        public static event LocatorTrainComplete_Handler LocatorTrainComplete;

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

        public void CallbackLocatorTrainedFunc(int nCamIdx)
        {
            LocatorTrainComplete?.Invoke(nCamIdx);
        }

        public void CallbackWriteLogFunc(string log)
        {
            WriteLogEvent?.Invoke(log);
        }

        public void CallbackAlarmFunc(string alarm)
        {
            AlarmEvent?.Invoke(alarm);
        }
    }
}
