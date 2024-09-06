using NVisionInspectGUI.Command.Cmd;
using NVisionInspectGUI.Commons;
using NVisionInspectGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NVisionInspectGUI.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        LoginViewModel loginViewModel = null;
        public LoginView()
        {
            InitializeComponent();

            loginViewModel = new LoginViewModel(this.Dispatcher, this);
            this.DataContext = loginViewModel;
        }

        private void pwBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginCmd loginCmd = new LoginCmd(loginViewModel);
                loginCmd.Execute(this);
            }
        }
    }
}
