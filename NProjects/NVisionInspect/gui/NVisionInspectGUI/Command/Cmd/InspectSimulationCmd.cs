
using NVisionInspectGUI.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class InspectSimulationCmd : CommandBase
    {
        public InspectSimulationCmd() { }
        public override void Execute(object parameter)
        {
            InterfaceManager.Instance.m_processorManager.m_NVisionInspectProcessorDll.InspectStart(1, 1);
        }
    }
}
