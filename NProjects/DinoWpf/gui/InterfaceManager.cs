using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoWpf
{
    internal class InterfaceManager
    {
        public TempInspectProcessorManager TempInspProcessorManager = new TempInspectProcessorManager();

        private static InterfaceManager _instance = null;
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
    }
}
