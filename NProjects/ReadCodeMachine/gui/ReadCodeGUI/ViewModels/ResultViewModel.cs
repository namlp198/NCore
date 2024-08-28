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
        private bool m_bResultOKNG;
        private int m_nCountOK;
        private int m_nCountNG;
        private int m_nCountTotal;
        private double m_dProcessTime;

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

        public int CountOK
        {
            get => m_nCountOK;
            set
            {
                if (SetProperty(ref m_nCountOK, value)) { }
            }
        }
        public int CountNG
        {
            get => m_nCountNG;
            set
            {
                if (SetProperty(ref m_nCountNG, value)) { }
            }
        }
        public int CountTotal
        {
            get => m_nCountTotal;
            set
            {
                if (SetProperty(ref m_nCountTotal, value)) { }
            }
        }
        public double ProcessTime
        {
            get => m_dProcessTime;
            set
            {
                if (SetProperty(ref m_dProcessTime, value)) { }
            }
        }
        #endregion
    }
}
