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
        LoginViewModel _loginViewModel;
        public LoginCmd(LoginViewModel loginViewModel) 
        { 
            _loginViewModel = loginViewModel;
        }
        public override void Execute(object parameter)
        {
            if (_loginViewModel.Admin.CompareTo(_loginViewModel.LoginView.txtUser.Text.Trim()) == 0
             && _loginViewModel.Password.CompareTo(_loginViewModel.Password.Trim()) == 0)
            {
                MainViewModel.Instance.UserLevel = Commons.emUserLevel.UserLevel_Admin;
                MainViewModel.Instance.MainView.tbLogin.Text = "LOGOUT";
                MainViewModel.Instance.DisplayImage_LoginStatusPath = "/NpcCore.Wpf;component/Resources/Images/logout.png";
                //MessageBox.Show("Login success!");
            }
            else if (_loginViewModel.SuperAdmin.CompareTo(_loginViewModel.LoginView.txtUser.Text.Trim()) == 0
                && _loginViewModel.SuperPassword.CompareTo(_loginViewModel.Password.Trim()) == 0)
            {
                MainViewModel.Instance.UserLevel = Commons.emUserLevel.UserLevel_SuperAdmin;
                MainViewModel.Instance.MainView.tbLogin.Text = "LOGOUT";
                MainViewModel.Instance.DisplayImage_LoginStatusPath = "/NpcCore.Wpf;component/Resources/Images/logout.png";
                //MessageBox.Show("Login success!");
            }

            else
            {
                MessageBox.Show("User or Password incorrect!");
            }

            _loginViewModel.LoginView.Close();
        }
    }
}
