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
    public class LoginCmd : CommandBase
    {
        LoginViewModel m_loginViewModel;
        public LoginCmd(LoginViewModel loginViewModel) 
        { 
            m_loginViewModel = loginViewModel;
        }
        public override void Execute(object parameter)
        {
            if (parameter == null)
                return;

            string btnName = parameter as string;

            if (btnName.Equals("btnLogin"))
            {
                if(m_loginViewModel.CheckInfoLogin())
                    m_loginViewModel.LoginView.Close();
            }
            else if(string.Compare(btnName, "btnClose") == 0)
            {
                m_loginViewModel.LoginView.Close();
            }
        }
    }
}
