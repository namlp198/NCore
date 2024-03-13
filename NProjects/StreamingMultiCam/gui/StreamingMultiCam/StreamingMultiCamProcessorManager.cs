using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingMultiCam
{
    internal class StreamingMultiCamProcessorManager
    {
        public StreamingMultiCamProcessorDll m_streamingMultiCamProcessor = new StreamingMultiCamProcessorDll();

        public void Initialize()
        {
            m_streamingMultiCamProcessor.Initialize();
        }

        public void Release()
        {
            m_streamingMultiCamProcessor.DeleteStreamingMultiCamProcessor();
        }
    }
}
