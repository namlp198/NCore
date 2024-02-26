using Npc.Foundation.Helper;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Npc.Foundation.Base
{
    public class ViewModelBase : BindableBase, IDisposable
    {
        protected IUnityContainer _unityContainer;
        protected IRegionManager _regionManager;
        protected IEventAggregator _eventAggregator;
        protected IEventAggregator _eventAggregatorGlobal;

        public ViewModelBase()
        {
            
        }

        public ViewModelBase(IEventAggregator eventAggregator)
        {
            this._regionManager = PrismHelper.GetRegionManager();
            this._unityContainer = PrismHelper.GetUnityContainer();
            this._eventAggregator = eventAggregator;
            this._eventAggregatorGlobal = PrismHelper.GetEventAggregator();
        }



        #region [ Dispose ]
        protected bool disposed = false;

        ~ViewModelBase()
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
