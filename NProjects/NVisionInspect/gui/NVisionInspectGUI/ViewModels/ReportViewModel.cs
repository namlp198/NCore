using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.Views;

namespace NVisionInspectGUI.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        #region Constructor
        public ReportViewModel(Dispatcher dispatcher, ReportView reportView) 
        { 
            m_dispatcher = dispatcher;
            m_vReportView = reportView;
        }
        #endregion
        #region Variables
        private ReportView m_vReportView;
        private readonly Dispatcher m_dispatcher;
        #endregion
        #region Properties
        public ReportView ReportView { get { return m_vReportView; } private set { } }
        #endregion
    }
}
