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

namespace DinoVisionGUI
{
    public enum EStatusLogin
    {
        StatusLogin_Fail,
        StatusLogin_Success
    }

    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public delegate void LoginSystemSuccess_Handler(LoginView login);
        public static event LoginSystemSuccess_Handler LoginSystemSuccess;

        private string m_sAdmin = "admin";
        private string m_sPassword = "123132213231312321";
        private EStatusLogin m_statusLogin;
        public EStatusLogin StatusLogin
        {
            get => m_statusLogin;
            set => m_statusLogin = value;
        }
        public LoginView()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(txtUser.Text.Trim().ToLower().CompareTo(m_sAdmin) == 0 &&
                pwBox.Password.CompareTo(m_sPassword) == 0)
            {
                StatusLogin = EStatusLogin.StatusLogin_Success;
                LoginSystemSuccess?.Invoke(this);
                this.Close();
            }
            else
            {
                MessageBox.Show("incorrect user or password, try again!");
            }
        }
    }
}
