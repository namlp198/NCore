using NCore.Wpf.BufferViewerSimple;
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

        private emInspectResult m_inspResultFinal_Cavity1 = emInspectResult.InspectResult_UNKNOWN;
        private emInspectResult m_inspResultFinal_Cavity2 = emInspectResult.InspectResult_UNKNOWN;

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

        public emInspectResult InspectionResultFinal_Cavity1
        {
            get => m_inspResultFinal_Cavity1;
            set
            {
                if(SetProperty(ref m_inspResultFinal_Cavity1, value))
                {

                }
            }
        }
        public emInspectResult InspectionResultFinal_Cavity2
        {
            get => m_inspResultFinal_Cavity2;
            set
            {
                if (SetProperty(ref m_inspResultFinal_Cavity2, value))
                {

                }
            }
        }
        #endregion
    }
}
