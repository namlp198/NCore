using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingMultiCam
{
    internal class InterfaceManager
    {
        public StreamingMultiCamProcessorManager m_streamingMultiCamProcessorManager = new StreamingMultiCamProcessorManager();

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
