using SealingInspectGUI.Commons;
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
    public class SelectMachineModeCmd : CommandBase
    {
        public SelectMachineModeCmd() { }
        public override void Execute(object parameter)
        {
            string machineMode = parameter as string;
            if(MainViewModel.Instance.MachineMode == Commons.emMachineMode.MachineMode_Auto &&
                string.Compare(machineMode.ToUpper(), "MANUAL") == 0)
            {
                if(MessageBox.Show("Change to Manual Mode?", "Alarm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_bSimulation == 0)
                    {
                        // start inspect with third param set is 1: on SIMULATOR mode, if don't use SIMULATOR, pass 0 value for this param
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStop(emInspectCavity.emInspectCavity_Cavity1);
                        InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStop(emInspectCavity.emInspectCavity_Cavity2);
                        MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_1.StopThreadWecon();
                    }

                    MainViewModel.Instance.MachineMode = Commons.emMachineMode.MachineMode_Manual;
                    MainViewModel.Instance.MainView.tbMachineMode.Text = "RUN";
                    MainViewModel.Instance.MainView.tbShowMachineMode.Text = "[MANUAL MODE]"; 
                    MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.stackManual.IsEnabled = true;
                    MainViewModel.Instance.DisplayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_forward.png";
                }
            }
            else
            {
                if (InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspectSysSetting.m_bSimulation == 0)
                {
                    // start inspect with third param set is 1: on SIMULATOR mode, if don't use SIMULATOR, pass 0 value for this param
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStart(1, emInspectCavity.emInspectCavity_Cavity1, 0);
                    InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.InspectStart(1, emInspectCavity.emInspectCavity_Cavity2, 0);
                    MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_1.StartThreadWecon();
                }

                MainViewModel.Instance.MachineMode = Commons.emMachineMode.MachineMode_Auto;
                MainViewModel.Instance.MainView.tbMachineMode.Text = "MANUAL";
                MainViewModel.Instance.MainView.tbShowMachineMode.Text = "[AUTO MODE]";
                MainViewModel.Instance.DisplayImage_MachineModePath = "/NpcCore.Wpf;component/Resources/Images/arrow_backward.png";
                MainViewModel.Instance.RunVM.SumCamVM.SumCameraView.stackManual.IsEnabled = false;
            }
        }
    }
}
