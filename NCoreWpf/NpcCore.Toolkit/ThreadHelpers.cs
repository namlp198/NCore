using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NpcCore.Toolkit
{
    public class ThreadHelpers
    {
        public delegate void MainThreadFunctionToCallHandler();

        private SynchronizationContext m_synchronizationContext;
        public SynchronizationContext SynchronizationContextProperty
        {
            get { return m_synchronizationContext; }
            set { m_synchronizationContext = value; }
        }

        private Thread m_creationThread;
        public Thread CreationThreadProperty
        {
            get { return m_creationThread; }
            set { m_creationThread = value; }
        }

        public ThreadHelpers()
        {
            SynchronizationContextProperty = SynchronizationContext.Current;
            if (SynchronizationContextProperty == null)
            {
                SynchronizationContextProperty = new SynchronizationContext();
            }
            CreationThreadProperty = Thread.CurrentThread;
        }

        public bool IsMethodCallInMainThread()
        {
            if (m_creationThread == Thread.CurrentThread ||
                m_synchronizationContext.GetType() == typeof(SynchronizationContext))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CallInMainThread(MainThreadFunctionToCallHandler functionToCall)
        {
            if (IsMethodCallInMainThread())
            {
                functionToCall();
            }
            else
            {
                m_synchronizationContext.Send(new SendOrPostCallback(delegate
                {
                    CallInMainThread(functionToCall);
                }), null);
            }
        }
    }
}
