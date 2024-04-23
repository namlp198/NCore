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
    public class SettingsViewModel : ViewModelBase
    {

        #region SingleTon

        private static SettingsViewModel _instance;
        public static SettingsViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region Constructor
        public SettingsViewModel()
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;


        }
        #endregion
    }
}
