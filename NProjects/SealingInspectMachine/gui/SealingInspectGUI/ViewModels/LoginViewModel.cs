using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Npc.Foundation.Base;
using SealingInspectGUI.Command.Cmd;
using SealingInspectGUI.Commons;
using SealingInspectGUI.Views;

namespace SealingInspectGUI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region singleton
        private static LoginViewModel _instance;
        public static LoginViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion
        public delegate void LoginSystemSuccess_Handler(LoginView login);
        public static event LoginSystemSuccess_Handler LoginSystemSuccess;

        private string m_sAdmin = "admin";
        private string m_sPassword = "1234";
        private string m_sSuperAdmin = "superadmin";
        private string m_sSuperPassword = "1111";

        private eLoginStatus m_loginStatus;

        private readonly Dispatcher _dispatcher;
        private LoginView _loginView;
        public LoginView LoginView { get => _loginView; private set { } }
        public LoginViewModel(Dispatcher dispatcher, LoginView loginView) 
        {
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _loginView = loginView;

            this.LoginCmd = new LoginCmd();
        }
        public eLoginStatus LoginStatus
        {
            get => m_loginStatus;
            set => m_loginStatus = value;
        }
        public string Admin
        {
            get => m_sAdmin;
            set => m_sAdmin = value;
        }
        public string Password
        {
            get => m_sPassword;
            set => m_sPassword = value;
        }
        public string SuperAdmin
        {
            get => m_sSuperAdmin;
            set => m_sSuperAdmin = value;
        }
        public string SuperPassword
        {
            get => m_sSuperPassword;
            set => m_sSuperPassword = value;
        }
        public ICommand LoginCmd { get; }
    }
}
