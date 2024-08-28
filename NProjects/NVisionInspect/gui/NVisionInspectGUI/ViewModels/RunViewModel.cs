using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Npc.Foundation.Base;
using NVisionInspectGUI.ViewModels;
using NVisionInspectGUI.Views.UcViews;

namespace NVisionInspectGUI.ViewModels
{
    public class RunViewModel : ViewModelBase
    {
        #region variables
        private readonly Dispatcher _dispatcher;
        private UcRunView m_ucRunView;
        #endregion

        #region Constructor
        public RunViewModel(Dispatcher dispatcher, UcRunView runView, Sum1CameraViewModel sumCamVM, ResultViewModel resultVM)
        {
            _dispatcher = dispatcher;
            m_ucRunView = runView;
            m_sum1CameraVM = sumCamVM;
            m_resultVM = resultVM;
        }
        #endregion

        public UcRunView RunView { get { return m_ucRunView; } }

        private Sum1CameraViewModel m_sum1CameraVM;
        public Sum1CameraViewModel SumCamVM { get => m_sum1CameraVM; private set { } }

        private ResultViewModel m_resultVM;
        public ResultViewModel ResultVM { get => m_resultVM; private set { } }
    }
}
