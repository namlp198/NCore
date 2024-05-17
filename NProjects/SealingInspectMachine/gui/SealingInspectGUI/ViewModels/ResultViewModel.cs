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
    public class ResultViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcResultView _resultView;
        #endregion

        #region Constructor
        public ResultViewModel(Dispatcher dispatcher, UcResultView resultView)
        {
            _dispatcher = dispatcher;
            _resultView = resultView;
        }
        #endregion

        #region Properties
        public UcResultView ResultView { get { return _resultView; } }
        #endregion
    }
}
