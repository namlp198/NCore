using SealingInspectGUI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
{
    public class GrabCavity2Cmd : CommandBase
    {
        public GrabCavity2Cmd() { }
        public override void Execute(object parameter)
        {
            InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.TestInspectCavity2();
        }
    }
}
