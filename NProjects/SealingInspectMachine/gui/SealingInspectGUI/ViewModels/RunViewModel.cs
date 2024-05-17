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
        public RunViewModel(Dispatcher dispatcher, UcRunView runView, SumCameraViewModel sumCamVM,
                            ResultViewModel resultVM, StatisticsViewModel statisticsVM)
        {
            _dispatcher = dispatcher;
            _runView = runView;
            _resultVM = resultVM;
            _statisticsVM = statisticsVM;
            _sumCameraVM = sumCamVM;
        }
        #endregion

        #region Properties
        public UcRunView RunView { get {  return _runView; } }

        private SumCameraViewModel _sumCameraVM;
        public SumCameraViewModel SumCamVM { get => _sumCameraVM; private set { } }

        private ResultViewModel _resultVM;
        public ResultViewModel ResultVM { get => _resultVM; private set { } }

        private StatisticsViewModel _statisticsVM;
        public StatisticsViewModel StatisticsVM { get => _statisticsVM; private set { } }
        #endregion
    }
}
