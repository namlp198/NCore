using Npc.Foundation.Helper;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Npc.Foundation.Base
{
    public enum PopupWindowResults
    {
        OK,
        Cancel,
        Yes,
        No,
        DualExit,
    }

    public class PopupWindowBase : Window, IDisposable
    {
        protected IUnityContainer _unityContainer;
        protected IRegionManager _regionManager;
        protected IEventAggregator _eventAggregator;
        protected IEventAggregator _eventAggregatorGlobal;

        public class PopupWindowBaseCompleteArgs
        {
            public bool IsCancel { get; set; }
        }

        public PopupWindowResults PopupResult { get; private set; }
        public object UserToken { get; set; }

        public PopupWindowBase()
        {
            this._regionManager = PrismHelper.GetRegionManager();
            this._unityContainer = PrismHelper.GetUnityContainer();
            this._eventAggregatorGlobal = PrismHelper.GetEventAggregator();

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public PopupWindowBase(IEventAggregator eventAggregator)
        {
            this._regionManager = PrismHelper.GetRegionManager();
            this._unityContainer = PrismHelper.GetUnityContainer();
            this._eventAggregator = eventAggregator;
            this._eventAggregatorGlobal = PrismHelper.GetEventAggregator();

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public void ClosePopup(PopupWindowResults result, object userToken = null)
        {
            PopupWindowBaseCompleteArgs e = new PopupWindowBaseCompleteArgs();
            OnCloseingPopup(e);

            if (e.IsCancel == true)
            {
                return;
            }

            this.PopupResult = result;
            //팝업 종료 시 객체를 넘기는 경우
            if (userToken != null)
                this.UserToken = userToken;

            this.Close();
        }

        protected virtual void OnCloseingPopup(PopupWindowBaseCompleteArgs e)
        {

        }

        protected virtual void OnClosedPopup()
        {
        }



        #region [ Dispose ]
        protected bool disposed = false;

        ~PopupWindowBase()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose Managed Resource
                }

                // Dispose UnManaged Resource

                this.disposed = true;
            }
        }
        #endregion // [ Dispose ]
    }
}
