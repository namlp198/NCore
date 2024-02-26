using Npc.Foundation.Base;
using System.Windows;

namespace NpcCore.Wpf.Popups
{
    /// <summary>
    /// MessageInfoBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageInfoBox : PopupWindowBase
    {
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageInfoBox));

        public string DetailMessage
        {
            get { return (string)GetValue(DetailMessageProperty); }
            set { SetValue(DetailMessageProperty, value); }
        }
        public static readonly DependencyProperty DetailMessageProperty = DependencyProperty.Register("DetailMessage", typeof(string), typeof(MessageInfoBox));


        public MessageInfoBox()
        {
            InitializeComponent();

            this.Title = "Exception Message";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup(PopupWindowResults.OK);
        }
    }
}
