using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npc.Foundation.Events
{
    /// <summary>
    /// Login User Changed Event
    /// </summary>
    public class LoginUserChangedEvent : PubSubEvent<LoginUserChangedEventArgs>
    {
    }

    public class LoginUserChangedEventArgs
    {
        public LoginUserChangedEventArgs()
        {
        }
    }
}
