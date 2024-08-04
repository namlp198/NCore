using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReadCodeGUI.Command.Cmd
{
    public class ContinuousGrabCmd : CommandBase
    {
        public ContinuousGrabCmd() { }
        public override void Execute(object parameter)
        {
            if (!MainViewModel.Instance.SettingVM.IsStreamming)
            {
                MainViewModel.Instance.SettingVM.SettingView.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM += 8; // Out Y4
                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.SetOutputPlc(true);
                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM = 2048; // reset output to init value

                    await MainViewModel.Instance.SettingVM.m_cameraStreamingController.ContinuousGrab(Manager.Class.CameraType.Basler);
                }));
            }
            else
            {
                MainViewModel.Instance.SettingVM.SettingView.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM += 8; // Out Y4
                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.SetOutputPlc(false);
                    MainViewModel.Instance.RunVM.SumCamVM.Plc_Delta_DVP.StartAddressBitM = 2048; // reset output to init value

                    await MainViewModel.Instance.SettingVM.m_cameraStreamingController.StopGrab(Manager.Class.CameraType.Basler);
                }));
            }
        }
    }
}
