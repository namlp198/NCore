using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Command.Cmd
{
    public class TestIOResetAllCmd : CommandBase
    {
        public TestIOResetAllCmd() { }
        public override void Execute(object parameter)
        {
            for(int i = 0; i < Defines.NUMBER_OF_SET_INSPECT; i++)
            {
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[i].m_bRing = 0;
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[i].m_b4Bar = 0;
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[i].m_bFrame1 = 0;
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[i].m_bFrame2 = 0;
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[i].m_bFrame3 = 0;
                InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[i].m_bFrame4 = 0;
            }

            TestIOViewModel.Instance.UseCavity1_Ring = false;
            TestIOViewModel.Instance.UseCavity1_4Bar = false;
            TestIOViewModel.Instance.UseCavity1_Frame1 = false;
            TestIOViewModel.Instance.UseCavity1_Frame2 = false;
            TestIOViewModel.Instance.UseCavity1_Frame3 = false;
            TestIOViewModel.Instance.UseCavity1_Frame4 = false;
            TestIOViewModel.Instance.Cavity1_NG = false;
        }
    }
}
