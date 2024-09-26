using NVisionInspectGUI.ViewModels;
using NVisionInspectGUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NVisionInspectGUI.Command.Cmd
{
    public class ShowLoginViewCmd : CommandBase
    {
        public ShowLoginViewCmd() { }
        public override void Execute(object parameter)
        {
            string s = parameter as string;
            if (s != null)
            {
                if(s.CompareTo("LOGIN") == 0)
                {
                    LoginView loginView = new LoginView();
                    loginView.ShowDialog();

                }
                else if(s.CompareTo("LOGOUT") == 0)
                {
                    MainViewModel.Instance.ROLE = Commons.emRole.Role_Operator;
                }
            }
        }
    }
}
