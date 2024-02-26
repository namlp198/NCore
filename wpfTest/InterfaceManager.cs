using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wfTestTaskProcessor
{
    internal class InterfaceManager
    {
        public ImageProcessorManager m_imageProcessorManager = new ImageProcessorManager();

        public SimulationThread m_simulationThread = new SimulationThread();

        private static InterfaceManager m_Instance = null;
        public static InterfaceManager Instance 
        { 
            get
            {
                if (m_Instance == null)
                    m_Instance = new InterfaceManager();
                return m_Instance;
            }
            private set { }
        }
    }
}
