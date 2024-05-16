using SealingInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealingInspectGUI.Command.Cmd
{
    public class SelectRunViewCmd : CommandBase
    {
        public SelectRunViewCmd()
        {
            
        }
        public override void Execute(object parameter)
        {
            MainViewModel.Instance.MainView.contentMain.Content = MainViewModel.Instance.RunVM.RunView;
        }
    }
}
