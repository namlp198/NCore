using Microsoft.Win32;
using NVisionInspectGUI.Manager;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class LoadImageCmd : CommandBase
    {
        public LoadImageCmd()
        {
        }
        public override void Execute(object parameter)
        {
            InterfaceManager.Instance.m_simulationThread.LoadImage();
        }
    }
}
