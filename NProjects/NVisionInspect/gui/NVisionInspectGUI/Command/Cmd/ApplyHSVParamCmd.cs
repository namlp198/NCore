using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVisionInspectGUI.Command.Cmd
{
    public class ApplyHSVParamCmd : CommandBase
    {
        public ApplyHSVParamCmd() { }
        public override void Execute(object parameter)
        {
            if (parameter == null)
                return;

            int nCamIdx = int.Parse(parameter.ToString());

            MainViewModel.Instance.SettingVM.SaveRecipeCmd.Execute(parameter);

            MainViewModel.Instance.SettingVM.LoadRecipe_FakeCam();

            MainViewModel.Instance.SettingVM.SettingView.propGridRecipe_FakeCam.SelectedObject = MainViewModel.Instance.SettingVM.NVisionInspectRecipeFakeCamPropertyGrid;

            MainViewModel.Instance.SettingVM.OpenPopupColorSpace = false;
        }
    }
}
