using System.ComponentModel;
using System.Windows;
using Npc.Foundation.Base;
using Npc.Foundation.Helper;
using Prism.Events;

namespace NpcCore.Wpf.Popups
{
    /// <summary>
    /// Interaction logic for ProgressPopup.xaml
    /// </summary>
    public partial class ProgressPopup : PopupWindowBase
    {
        #region [ =============== Field =============== ] 
        #endregion // [ =============== Field =============== ]


        #region [ =============== Property =============== ]
        public string TaskTitle
        {
            get { return (string)GetValue(TaskTitleProperty); }
            set { SetValue(TaskTitleProperty, value); }
        }
        public static readonly DependencyProperty TaskTitleProperty = DependencyProperty.Register("TaskTitle", typeof(string), typeof(ProgressPopup));

        public int ProgressValue
        {
            get { return (int)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }
        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(int), typeof(ProgressPopup));

        public int ElapsedTime
        {
            get { return (int)GetValue(ElapsedTimeProperty); }
            set { SetValue(ElapsedTimeProperty, value); }
        }
        public static readonly DependencyProperty ElapsedTimeProperty = DependencyProperty.Register("ElapsedTime", typeof(int), typeof(ProgressPopup));
        #endregion // [ =============== Property =============== ]


        #region [ =============== Constructor / Initialization =============== ]
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="recipe"></param>
        public ProgressPopup(IEventAggregator eventAggregator, string taskTitle, int progressValue, int elapsedTime, bool showCancel)
        {
            this._eventAggregator = eventAggregator;
            this._eventAggregatorGlobal = PrismHelper.GetEventAggregator();

            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            this.TaskTitle = taskTitle;
            this.ProgressValue = progressValue;
            this.ElapsedTime = elapsedTime;

            if (showCancel)
                rdBottom.Height = new GridLength(65);
            else
                rdBottom.Height = new GridLength(0);
        }
        #endregion // [ =============== Constructor / Initialization =============== ]


        #region [ =============== Event Action =============== ]  
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup(PopupWindowResults.Cancel);
        } 

        #endregion // [ =============== Event Action =============== ]


        #region [ =============== Etc =============== ]

        #endregion // [ =============== Etc =============== ]


        #region [ =============== Dispose =============== ]
        //private bool disposed = false;

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose Managed Resource
                }

                // Dispose UnManaged Resource
                //if (this.DataContext != null) { (this.DataContext as ExtRecipeSaveJobModel).Dispose(); } 
                this.disposed = true;
            }

            base.Dispose(disposing);
        }
        #endregion // [ =============== Dispose =============== ]
    }
}