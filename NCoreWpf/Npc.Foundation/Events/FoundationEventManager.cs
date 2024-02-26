using Npc.Foundation.Helper;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Events
{
    public static class FoundationEventManager
    {
        #region [ =============== LoginUserChangedEvent =============== ]
        /// <summary>
        /// Publish LoginUserChangedEvent
        /// </summary>
        public static void PublishLoginUserChangedEvent()
        {
            var eaGlobal = PrismHelper.GetEventAggregator();
            eaGlobal.GetEvent<LoginUserChangedEvent>().Publish(new LoginUserChangedEventArgs());
        }

        /// <summary>
        /// Subscribe LoginUserChangedEvent
        /// </summary>
        /// <param name="ea"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static SubscriptionToken ArrivalLoginUserChangedEvent(Action<LoginUserChangedEventArgs> callback)
        {
            var eaGlobal = PrismHelper.GetEventAggregator();
            return eaGlobal.GetEvent<LoginUserChangedEvent>().Subscribe(callback, ThreadOption.UIThread, false);
        }

        /// <summary>
        /// Unsubscribe LoginUserChangedEvent
        /// </summary>
        /// <param name="subToken"></param>
        public static void UnsubscribeLoginUserChangedEvent(SubscriptionToken subToken)
        {
            if (subToken != null)
            {
                var eaGlobal = PrismHelper.GetEventAggregator();
                eaGlobal.GetEvent<LoginUserChangedEvent>().Unsubscribe(subToken);
                subToken.Dispose();
                subToken = null;
            }
        }
        #endregion // [ =============== LoginUserChangedEvent =============== ]

        #region [ =============== LogViewEvent =============== ]
        public static void RequestLogViewEvent(LogViewEventArgs args)
        {
            var eaGlobal = PrismHelper.GetEventAggregator();
            eaGlobal.GetEvent<LogViewEvent>().Publish(args);
        }

        public static SubscriptionToken ResponseLogViewEvent(Action<LogViewEventArgs> callback)
        {
            var eaGlobal = PrismHelper.GetEventAggregator();
            return eaGlobal.GetEvent<LogViewEvent>().Subscribe(callback, ThreadOption.UIThread, false);
        }

        public static void UnsubscribeLogViewEvent(SubscriptionToken subToken)
        {
            if (subToken != null)
            {
                var eaGlobal = PrismHelper.GetEventAggregator();
                eaGlobal.GetEvent<LogViewEvent>().Unsubscribe(subToken);
                subToken.Dispose();
                subToken = null;
            }
        }
        #endregion // [ =============== LogViewEvent =============== ]
    }
}
