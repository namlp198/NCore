using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Npc.Foundation.Base;
using ReadCodeGUI.Views.UcViews;

namespace ReadCodeGUI.ViewModels
{
    public class RunViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcRunView _runView;
        #endregion

        #region Constructor
        public RunViewModel(Dispatcher dispatcher, UcRunView runView, SumCameraViewModel sumCamVM, ResultViewModel resultVM)
        {
            _dispatcher = dispatcher;
            _runView = runView;
            _sumCameraVM = sumCamVM;
            _resultVM = resultVM;
        }
        #endregion

        public UcRunView RunView { get { return _runView; } }

        private SumCameraViewModel _sumCameraVM;
        public SumCameraViewModel SumCamVM { get => _sumCameraVM; private set { } }

        private ResultViewModel _resultVM;
        public ResultViewModel ResultVM { get => _resultVM; private set { } }
    }
}
