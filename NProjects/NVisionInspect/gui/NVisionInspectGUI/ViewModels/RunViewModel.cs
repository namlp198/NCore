using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.Manager.Class;
using NVisionInspectGUI.ViewModels;
using NVisionInspectGUI.Views.UcViews;

namespace NVisionInspectGUI.ViewModels
{
    public interface ISumCamVM
    {
        //void InspectionComplete(int nCamIdx, int bSetting);
    }
    public class RunViewModel<T> : ViewModelBase where T : ISumCamVM
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcRunView m_ucRunView;
        #endregion

        #region Constructor
        public RunViewModel(Dispatcher dispatcher, UcRunView runView)
        {
            _dispatcher = dispatcher;
            m_ucRunView = runView;
        }
        #endregion

        public UcRunView RunView { get { return m_ucRunView; } }

        private T m_sumCameraVM;
        public T SumCamVM { get => m_sumCameraVM; set => m_sumCameraVM = value; }

        private ResultViewModel m_resultVM;
        public ResultViewModel ResultVM { get => m_resultVM; set => m_resultVM = value; }
    }
}
