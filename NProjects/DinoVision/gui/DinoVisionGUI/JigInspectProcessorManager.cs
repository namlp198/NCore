using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    internal class JigInspectProcessorManager
    {
        public JigInspectProcessorDll JigInspProcessorDll = new JigInspectProcessorDll();
        public CJigInspectConfigurations JigInspConfigurations = new CJigInspectConfigurations();
        public CJigInspectResults JigInspResults = new CJigInspectResults();
        public void Initialize()
        {
            JigInspProcessorDll.RegCallBackInspectCompleteFunc(InterfaceManager.Instance.CallbackInsCompleteFunc);
            JigInspProcessorDll.Initialize();
        }
        public void Destroy()
        {
            JigInspProcessorDll.DeleteJigInspectProcessor();
        }
    }
}
