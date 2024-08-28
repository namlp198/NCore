
using ReadCodeGUI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReadCodeGUI.Command.Cmd
{
    public class InspectSimulationCmd : CommandBase
    {
        public InspectSimulationCmd() { }
        public override void Execute(object parameter)
        {
            InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.InspectStart(1, 1);
        }
    }
}
