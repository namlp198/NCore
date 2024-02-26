using Npc.Foundation.Base;
using Prism.Events;
using System.Windows;
using System.Windows.Controls;
          
namespace NpcCore.Wpf.Popups
{
    /// <summary>
    /// ExitPopup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ExitPopup : PopupWindowBase
    {
        public ExitPopup(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            InitializeComponent();

            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var fe = e.OriginalSource as FrameworkElement;
            if (fe != null)
            {
                var tag = (fe.Tag ?? "").ToString();

                if (tag == "Close")
                {
                    this.ClosePopup(PopupWindowResults.OK);
                }
                else if (tag == "DualClose")
                {
                    this.ClosePopup(PopupWindowResults.DualExit);
                }
                else
                {
                    this.ClosePopup(PopupWindowResults.Cancel);
                }
            }
        }
    }
}
