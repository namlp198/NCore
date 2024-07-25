using Microsoft.Win32;
using ReadCodeGUI.Manager;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReadCodeGUI.Command.Cmd
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
