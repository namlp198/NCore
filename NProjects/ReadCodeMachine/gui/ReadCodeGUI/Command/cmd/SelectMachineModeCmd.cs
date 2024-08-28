using ReadCodeGUI.Manager;
using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ReadCodeGUI.Command.Cmd
{
    public class SelectMachineModeCmd : CommandBase
    {
        public SelectMachineModeCmd() { }
        public override void Execute(object parameter)
        {
            string machineMode = parameter as string;
            if(MainViewModel.Instance.MachineMode == Commons.eMachineMode.MachineMode_Auto &&
                string.Compare(machineMode.ToUpper(), "MANUAL") == 0)
            {
                if(MessageBox.Show("Change to Manual Mode?", "Alarm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                   
                    MainViewModel.Instance.InspectRunning = false;
                    InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.InspectStop();

                    MainViewModel.Instance.MachineMode = Commons.eMachineMode.MachineMode_Manual;
                    MainViewModel.Instance.MainView.tbMachineMode.Text = "RUN";
                    MainViewModel.Instance.MainView.tbShowMachineMode.Text = "[MANUAL MODE]"; 
                    MainViewModel.Instance.DisplayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_forward.png";
                }
            }
            else
            {
                MainViewModel.Instance.InspectRunning = true;
                InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.InspectStart(1, 0);

                MainViewModel.Instance.MachineMode = Commons.eMachineMode.MachineMode_Auto;
                MainViewModel.Instance.MainView.tbMachineMode.Text = "MANUAL";
                MainViewModel.Instance.MainView.tbShowMachineMode.Text = "[AUTO MODE]";
                MainViewModel.Instance.DisplayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_backward.png";
            }
        }
    }
}
