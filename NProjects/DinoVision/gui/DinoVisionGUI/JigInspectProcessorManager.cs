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

        public void Initialize()
        {
            JigInspProcessorDll.Initialize();
        }
        public void Destroy()
        {
            JigInspProcessorDll.DeleteJigInspectProcessor();
        }
    }
}
