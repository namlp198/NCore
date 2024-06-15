using SealingInspectGUI.Commons;
using SealingInspectGUI.Manager;
using SealingInspectGUI.ViewModels;
using SealingInspectGUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace SealingInspectGUI.Command.Cmd
{
    public class TestIOCmd : CommandBase
    {
        public TestIOCmd() { }
        public override void Execute(object parameter)
        {
            string btnName = parameter as string;
            if (string.Compare(btnName, "btnShowTestIOView") == 0)
            {
                TestIOView view = new TestIOView();
                view.Show();
            }
            else if (string.Compare(btnName, "btnInspSimul") == 0)
            {
                //int nCavityIdx = 0;
                //InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bLOCK_PROCESS = 1;
                //InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                //            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);
                //nCavityIdx++;
                //InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx].m_bLOCK_PROCESS = 1;
                //InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspProcessorDll.SetSealingInspectSimulationIO(nCavityIdx,
                //            ref InterfaceManager.Instance.m_sealingInspectProcessorManager.m_sealingInspect_Simulation_IO[nCavityIdx]);

                //if (MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_1.IsConnected)
                //    MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_1.InspectStart();

                //if (MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_2.IsConnected)
                //    MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_2.InspectStart();


                //if (MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_1.IsConnected)
                //    MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_1.StartThreadPlcWecon1();

                //if (MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_2.IsConnected)
                //    MainViewModel.Instance.RunVM.SumCamVM.PLC_Wecon_2.StartThreadPlcWecon1();

            }
        }
    }
}
