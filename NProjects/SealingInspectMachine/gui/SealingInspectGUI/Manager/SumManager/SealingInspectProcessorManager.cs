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
            m_sealingInspProcessorDll.RegCallBackInspectCavity1CompleteFunc(InterfaceManager.Instance.CallbackInsCavity1CompleteFunc);
            m_sealingInspProcessorDll.RegCallBackInspectCavity2CompleteFunc(InterfaceManager.Instance.CallbackInsCavity2CompleteFunc);
            m_sealingInspProcessorDll.RegCallBackInspectTopCam1CompleteFunc(InterfaceManager.Instance.CallbackInsTopCam1CompleteFunc);
            m_sealingInspProcessorDll.RegCallBackInspectTopCam2CompleteFunc(InterfaceManager.Instance.CallbackInsTopCam2CompleteFunc);
            m_sealingInspProcessorDll.Initialize();
        }

        public void Destroy()
        {
            m_sealingInspProcessorDll.DeleteSealingInspectProcessor();
        }
    }
}
