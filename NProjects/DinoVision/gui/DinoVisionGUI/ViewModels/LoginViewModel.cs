using DinoVisionGUI.Views;
using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DinoVisionGUI.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        #region SingleTon

        private static LoginViewModel _instance;
        public static LoginViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

        }
        #endregion
    }
}
