using SealingInspectGUI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Command.Cmd
{
    public class GrabAllCmd : CommandBase
    {
        public GrabAllCmd() { }
        public override void Execute(object parameter)
        {
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.TestInspectCavity1();
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.TestInspectCavity2();
        }
    }
}
