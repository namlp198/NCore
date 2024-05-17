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
    public class StatisticsViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcStatisticsView _statisticsView;
        #endregion

        #region Constructor
        public StatisticsViewModel(Dispatcher dispatcher, UcStatisticsView statisticsView)
        {
            _dispatcher = dispatcher;
            _statisticsView = statisticsView;
        }
        #endregion

        #region Properties
        public UcStatisticsView StatisticsView { get { return _statisticsView; } }
        #endregion
    }
}
