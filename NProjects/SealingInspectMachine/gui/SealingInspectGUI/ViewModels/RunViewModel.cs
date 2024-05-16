using Npc.Foundation.Base;
using SealingInspectGUI.Views.UcViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SealingInspectGUI.ViewModels
{
    public class RunViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcRunView _runView;
        #endregion

        #region Constructor
        public RunViewModel(Dispatcher dispatcher, UcRunView runView)
        {
            _dispatcher = dispatcher;
            _runView = runView;
        }
        #endregion

        #region Properties
        public UcRunView RunView { get {  return _runView; } }
        #endregion
    }
}
