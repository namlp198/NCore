using DinoWpf.Views;
using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DinoWpf.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        #region SingleTon
        private static MainViewModel _instance;
        public static MainViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion
        private readonly Dispatcher _dispatcher;
        private MainView _mainView;
        public MainView MainView { get { return _mainView; } private set { } }

        #region Constructor
        public MainViewModel(Dispatcher dispatcher, MainView mainView)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _mainView = mainView;
        }
        #endregion
    }
}
