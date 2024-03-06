using DinoWpf.Commons;
using DinoWpf.Models;
using DinoWpf.Views;
using Npc.Foundation.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DinoWpf.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public log4net.ILog Logger { get; } = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region SingleTon
        private static MainViewModel _instance;
        public static MainViewModel Instance
        {
            get { return _instance; }
            private set { }
        }
        #endregion

        #region Constructor
        public MainViewModel(Dispatcher dispatcher, MainView mainView)
        {
            // construct a instance of MainViewModel
            if (_instance == null) _instance = this;
            else return;

            _dispatcher = dispatcher;
            _mainView = mainView;

            GetAllJobNames();
        }
        #endregion

        #region variables
        private readonly Dispatcher _dispatcher;
        private MainView _mainView;
        private List<string> _jobs = new List<string>();
        private string _jobSelected = string.Empty;
        #endregion

        public MainView MainView { get { return _mainView; } private set { } }

        #region Properties
        public List<string> JobList
        {
            get => _jobs;
            set
            {
                if (SetProperty(ref _jobs, value))
                {

                }
            }
        }
        public string JobSelected
        {
            get => _jobSelected;
            set
            {
                if(SetProperty(ref _jobSelected, value))
                {

                }
            }
        }
        #endregion

        #region Methods
        public void GetAllJobNames()
        {
            _jobs.Clear();

            List<string> jobs = new List<string>();
            DirectoryInfo di = new DirectoryInfo(CommonDefines.JobXmlPath);
            FileInfo[] fileInfos = di.GetFiles("*.xml");
            foreach (FileInfo fileInfo in fileInfos)
            {
                string jobName = fileInfo.Name;
                jobs.Add(jobName);
            }
            JobList = jobs;
        }
        #endregion
    }
}
