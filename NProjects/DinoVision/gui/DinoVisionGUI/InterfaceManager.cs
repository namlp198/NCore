using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DinoVisionGUI
{
    public class InterfaceManager
    {
        public JigInspectProcessorManager JigInspProcessorManager = new JigInspectProcessorManager();

        public delegate void InspectionComplete_Handler();
        public static event InspectionComplete_Handler InspectionComplete;

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

        public void CallbackInsCompleteFunc()
        {
            InspectionComplete();
        }
    }
}
