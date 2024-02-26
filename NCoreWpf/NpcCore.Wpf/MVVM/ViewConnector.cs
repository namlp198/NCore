using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcCore.Wpf.MVVM
{
    public class ViewConnector<TView>
    {
        public IView View { get; set; }

        public void CreateView()
        {
            View = (IView)Activator.CreateInstance(typeof(TView));
        }

        public TView GetViewInstance()
        {
            return (TView)View;
        }
    }
}
