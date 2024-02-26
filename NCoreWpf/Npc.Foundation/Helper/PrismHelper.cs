using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace Npc.Foundation.Helper
{
    public enum EventAggregatorType
    {
        Global,
        Recipe,
        Scan
    }
    public static class PrismHelper
    {
        #region [ ===== RegionManager ===== ]
        /// <summary>
        /// RegionManager
        /// </summary>
        public static IRegionManager _regionManager { get; private set; }

        /// <summary>
        /// Set RegionManager
        /// </summary>
        /// <param name="regionManager"></param>
        public static void SetRegionManager(IRegionManager regionManager)
        {
            if (_regionManager == null)
                _regionManager = regionManager;
        }

        /// <summary>
        /// Get RegionManager
        /// </summary>
        /// <returns></returns>
        public static IRegionManager GetRegionManager()
        {
            return _regionManager;
        }

        /// <summary>
        /// Register View in Region
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="region"></param>
        /// <param name="viewType"></param>
        public static void RegisterViewInRegion(string regionName, DependencyObject region, Type viewType)
        {
            if (!_regionManager.Regions.ContainsRegionWithName(regionName))
            {
                RegionManager.SetRegionManager(region, _regionManager);
                RegionManager.UpdateRegions();
                _regionManager.Regions[regionName]?.RemoveAll();
                _regionManager.RegisterViewWithRegion(regionName, viewType);
            }
            else
            {
                _regionManager.RequestNavigate(regionName, viewType.Name);
            }
        }
        #endregion // [ ===== RegionManager ===== ]


        #region [ ===== EventAggregator ===== ]
        /// <summary>
        /// Global EventAggregator
        /// </summary>
        private static IEventAggregator _eventAggregatorGlobal
        {
            get { return GetEventAggregator(EventAggregatorType.Global); }
        }

        /// <summary>
        /// EventAggregator List
        /// </summary>
        private static Dictionary<EventAggregatorType, IEventAggregator> _eventAggregatorList { get; set; }

        /// <summary>
        /// Set EventAggregator
        /// </summary>
        /// <param name="key"></param>
        /// <param name="eventAggregator"></param>
        public static void SetEventAggregator(EventAggregatorType key, IEventAggregator eventAggregator)
        {
            if (_eventAggregatorList == null)
            {
                _eventAggregatorList = new Dictionary<EventAggregatorType, IEventAggregator>();
            }
            if (!_eventAggregatorList.ContainsKey(key))
            {
                _eventAggregatorList.Add(key, eventAggregator);
            }
        }

        /// <summary>
        /// Get EventAggregator
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEventAggregator GetEventAggregator(EventAggregatorType key)
        {
            if (_eventAggregatorList == null)
            {
                _eventAggregatorList = new Dictionary<EventAggregatorType, IEventAggregator>();
            }
            if (_eventAggregatorList.ContainsKey(key))
            {
                return _eventAggregatorList[key];
            }
            else
            {
                var eventAggregator = new EventAggregator();
                _eventAggregatorList.Add(key, eventAggregator);
                return eventAggregator;
            }
        }

        /// <summary>
        /// Get Global EventAggregator
        /// </summary>
        /// <returns></returns>
        public static IEventAggregator GetEventAggregator()
        {
            return GetEventAggregator(EventAggregatorType.Global);
        }
        #endregion // [ ===== EventAggregator ===== ]


        #region [ ===== UnityContainer ===== ]
        /// <summary>
        /// UnityContainer List
        /// </summary>
        public static Dictionary<string, IUnityContainer> UnityContainers { get; private set; } = new Dictionary<string, IUnityContainer>();

        /// <summary>
        /// Set UnityContainer
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="container"></param>
        public static void SetUnityContainer(string domain, IUnityContainer container)
        {
            if (UnityContainers.ContainsKey(domain) == false)
            {
                UnityContainers.Add(domain, container);
            }
        }

        /// <summary>
        /// Get UnityContainer
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IUnityContainer GetUnityContainer(string domain)
        {
            if (UnityContainers.ContainsKey(domain) == true)
            {
                return UnityContainers[domain];
            }
            return null;
        }

        /// <summary>
        /// Get UnityContainer (CCI domain)
        /// </summary>
        /// <returns></returns>
        public static IUnityContainer GetUnityContainer()
        {
            return GetUnityContainer("Npc");
        }

        /// <summary>
        /// Register Instance in UnityContainer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <param name="instance"></param>
        public static void RegisterInstance<T>(string domain, string key, T instance)
        {
            var container = GetUnityContainer(domain);
            if (container != null)
            {
                container.RegisterInstance<T>(key, instance);
            }
        }

        /// <summary>
        /// Get Instance in UnityContainer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetInstance<T>(string domain, string key)
        {
            try
            {
                var container = GetUnityContainer(domain);
                if (container != null)
                {
                    var has = container.Registrations.Any(f => f.RegisteredType == typeof(T) && f.Name == key);
                    if (has == true)
                    {
                        return container.Resolve<T>(key);
                    }
                }
            }
            catch { }

            return default(T);
        }

        /// <summary>
        /// Get Instance in UnityContainer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static T GetInstance<T>(string domain)
        {
            try
            {
                var container = GetUnityContainer(domain);
                if (container != null)
                {
                    var has = container.Registrations.Any(f => f.RegisteredType == typeof(T));
                    if (has == true)
                    {
                        return container.Resolve<T>();
                    }

                }
            }
            catch { }

            return default(T);
        }
        #endregion // [ ===== UnityContainer ===== ]
    }
}
