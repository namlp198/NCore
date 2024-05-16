using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Manager
{
    public class InterfaceManager
    {
        #region Singleton
        private static InterfaceManager _instance;
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
        #endregion
    }
}
