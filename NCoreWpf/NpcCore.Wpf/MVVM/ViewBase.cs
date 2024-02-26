using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NpcCore.Wpf.MVVM
{
    public class ViewBase : Window, IView
    {
        private string m_language;
        public string CurrentLanguage
        {
            get { return m_language; }
            set
            {
                m_language = value;
            }
        }
        public ViewBase()
        {
            CurrentLanguage = "Default";
        }
        public void SetLanguage(string sLanguage)
        {
            CurrentLanguage = sLanguage;
        }
        public void ShowView()
        {
            Show();
        }

        public void CloseView()
        {
            Close();
        }

        /// <summary>
        /// Function allowing to define to the view its DataContext which will ensure the DataBinding
        /// </summary>
        /// <param name="viewModel"></param>
        public virtual void SetDataContext(object viewModel)
        {
            DataContext = viewModel;
        }
    }
}
