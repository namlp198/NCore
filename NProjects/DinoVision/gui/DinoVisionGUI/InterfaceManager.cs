using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    internal class InterfaceManager
    {
        public JigInspectProcessorManager JigInspProcessorManager = new JigInspectProcessorManager();

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
