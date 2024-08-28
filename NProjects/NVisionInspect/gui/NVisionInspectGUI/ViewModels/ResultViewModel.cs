using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.Models;
using NVisionInspectGUI.Views.UcViews;

namespace NVisionInspectGUI.ViewModels
{
    public class ResultViewModel : ViewModelBase
    {
        #region variables
        private static readonly object _lockObj = new object();
        private readonly Dispatcher _dispatcher;
        private UcResultView m_ucResultView;
        private bool m_bResultOKNG;
        private int m_nCountOK;
        private int m_nCountNG;
        private int m_nCountTotal;
        private double m_dProcessTime;
        #endregion

        #region Constructor
        public ResultViewModel(Dispatcher dispatcher, UcResultView resultView)
        {
            _dispatcher = dispatcher;
            m_ucResultView = resultView;
        }
        #endregion

        #region Properties
        public UcResultView ResultView { get { return m_ucResultView; } }
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
