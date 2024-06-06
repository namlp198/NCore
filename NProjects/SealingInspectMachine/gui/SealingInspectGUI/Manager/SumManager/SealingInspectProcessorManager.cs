using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager.Class;
using SealingInspectGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SealingInspectGUI.Manager.SumManager
{
    public class SealingInspectProcessorManager
    {
        public SealingInspectProcessorDll m_sealingInspProcessorDll = new SealingInspectProcessorDll();
        public CSealingInspectResult[] m_sealingInspectResult = new CSealingInspectResult[Defines.NUMBER_OF_SET_INSPECT];
        public CSealingInspect_Simulation_IO[] m_sealingInspect_Simulation_IO = new CSealingInspect_Simulation_IO[Defines.NUMBER_OF_SET_INSPECT];
        public CSealingInspectSystemSetting m_sealingInspectSysSetting = new CSealingInspectSystemSetting();
        public CSealingInspectRecipe m_sealingInspectRecipe = new CSealingInspectRecipe();
        public void Initialize()
        {
            m_sealingInspProcessorDll.RegCallBackInspectCompleteFunc(InterfaceManager.Instance.CallbackInsCompleteFunc);
            m_sealingInspProcessorDll.RegCallBackInspectTopCamCompleteFunc(InterfaceManager.Instance.CallbackInsTopCamCompleteFunc);
            m_sealingInspProcessorDll.Initialize();
        }

        public void Destroy()
        {
            m_sealingInspProcessorDll.DeleteSealingInspectProcessor();
        }
    }
}
