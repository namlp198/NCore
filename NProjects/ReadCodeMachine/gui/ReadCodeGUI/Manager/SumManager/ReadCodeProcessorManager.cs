using ReadCodeGUI.Commons;
using ReadCodeGUI.Manager.Class;
using ReadCodeGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadCodeGUI.Manager.SumManager
{
    public class ReadCodeProcessorManager
    {
        public ReadCodeProcessorDll m_readCodeProcessorDll = new ReadCodeProcessorDll();
        public CReadCodeResult[] m_readCodeResult = new CReadCodeResult[Defines.NUMBER_OF_SET_INSPECT];
        public CReadCodeSystemSetting m_readCodeSysSettings = new CReadCodeSystemSetting();
        public CReadCodeRecipe m_readCodeRecipe = new CReadCodeRecipe();

        public void Initialize()
        {
            m_readCodeProcessorDll.RegCallBackInspectCompleteFunc(InterfaceManager.Instance.CallbackInsCompleteFunc);
            m_readCodeProcessorDll.Initialize();
        }

        public void Destroy()
        {
            m_readCodeProcessorDll.DeleteReadCodeProcessor();
        }
    }
}
