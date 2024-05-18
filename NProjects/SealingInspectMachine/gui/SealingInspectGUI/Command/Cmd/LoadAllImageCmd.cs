using Microsoft.Win32;
using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SealingInspectGUI.Command.Cmd
{
    public class LoadAllImageCmd : CommandBase
    {
        public LoadAllImageCmd()
        {
        }
        public override void Execute(object parameter)
        {
            InterfaceManager.Instance.m_simulationThread.LoadAllImage();
        }
    }
}
