using Npc.Foundation.Helper;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Unity;

namespace Npc.Foundation.Base
{
    public class ViewBase : UserControl, IDisposable
    {
        /// <summary>
        /// UI Guid (View Instance Unique Key)
        /// </summary>
        protected Guid UIGuid { get; set; }

        protected IUnityContainer _unityContainer;
        protected IRegionManager _regionManager;
        protected IEventAggregator _eventAggregator;
        protected IEventAggregator _eventAggregatorGlobal;

        public ViewBase()
        {
            // Set UIGuid
            if (this.UIGuid == Guid.Empty) { this.UIGuid = Guid.NewGuid(); }
        }

        public ViewBase(IEventAggregator eventAggregator)
        {
            // Set UIGuid
            if (this.UIGuid == Guid.Empty) { this.UIGuid = Guid.NewGuid(); }

            this._regionManager = PrismHelper.GetRegionManager();
            this._unityContainer = PrismHelper.GetUnityContainer();
            this._eventAggregator = eventAggregator;
            this._eventAggregatorGlobal = PrismHelper.GetEventAggregator();
        }

        #region [ Dispose ]
        protected bool disposed = false;

        ~ViewBase()
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
