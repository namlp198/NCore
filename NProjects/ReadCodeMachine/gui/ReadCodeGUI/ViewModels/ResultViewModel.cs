using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Npc.Foundation.Base;
using ReadCodeGUI.Models;
using ReadCodeGUI.Views.UcViews;

namespace ReadCodeGUI.ViewModels
{
    public class ResultViewModel : ViewModelBase
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcResultView _resultView;

        private List<ResultStringMapToDataGridModel> m_listResultStringMapToDataGrid = new List<ResultStringMapToDataGridModel>();
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

        public List<ResultStringMapToDataGridModel> ListResultStringMapToDataGrid 
        { 
            get { return m_listResultStringMapToDataGrid; }
            set
            {
                if(SetProperty(ref m_listResultStringMapToDataGrid, value))
                {

                }
            }
        }
        #endregion
    }
}
