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

                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM += 8; // M8
                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.SetOutputPlc(false);
                    //MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM = 2048; // reset output to init value

                    MainViewModel.Instance.MachineMode = Commons.eMachineMode.MachineMode_Manual;
                    MainViewModel.Instance.MainView.tbMachineMode.Text = "RUN";
                    MainViewModel.Instance.MainView.tbShowMachineMode.Text = "[MANUAL MODE]"; 
                    MainViewModel.Instance.DisplayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_forward.png";
                }
            }
            else
            {
                MainViewModel.Instance.InspectRunning = true;
                InterfaceManager.Instance.m_processorManager.m_readCodeProcessorDll.InspectStart(0);

                //MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM += 8; // M8
                //MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.SetOutputPlc(true);
                //MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM = 2048; // reset output to init value

                MainViewModel.Instance.MachineMode = Commons.eMachineMode.MachineMode_Auto;
                MainViewModel.Instance.MainView.tbMachineMode.Text = "MANUAL";
                MainViewModel.Instance.MainView.tbShowMachineMode.Text = "[AUTO MODE]";
                MainViewModel.Instance.DisplayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_backward.png";
            }
        }
    }
}
