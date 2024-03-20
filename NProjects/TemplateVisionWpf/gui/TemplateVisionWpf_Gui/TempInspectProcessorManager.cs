using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateVisionWpf_Gui
{
    internal class TempInspectProcessorManager
    {
        public TempInspectProcessorDll TempInspProcessorDll = new TempInspectProcessorDll();

        public void Initialize()
        {
            TempInspProcessorDll.Initialize();
        }
        public void Destroy()
        {
            TempInspProcessorDll.DeleteTempInspectProcessor();
        }
    }
}
