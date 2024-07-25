using ReadCodeGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadCodeGUI.Command.Cmd
{
    public class SelectSettingViewCmd : CommandBase
    {
        public SelectSettingViewCmd()
        {
            
        }
        public override void Execute(object parameter)
        {
            MainViewModel.Instance.MainView.contentMain.Content = MainViewModel.Instance.SettingVM.SettingView;
        }
    }
}
