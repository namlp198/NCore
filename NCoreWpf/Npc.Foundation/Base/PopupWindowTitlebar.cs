using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Xml.Linq;
using Npc.Foundation.Helper;

namespace Npc.Foundation.Base
{
    [TemplatePart(Name = PART_TitlePanel, Type = typeof(Panel))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    public class PopupWindowTitlebar : ContentControl
    {
        private const string PART_TitlePanel = "PART_TitlePanel";
        private const string PART_CloseButton = "PART_CloseButton";

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(PopupWindowTitlebar));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _hostWindow = VisualTreeHelperEx.FindAncestorByType<PopupWindowBase>(this);

            var titlePanel = GetTemplateChild(PART_TitlePanel) as Panel;
            if (titlePanel != null)
            {
                titlePanel.MouseMove += TitlePanel_MouseMove;
            }

            var closeButton = GetTemplateChild(PART_CloseButton) as Button;
            if (closeButton != null)
            {
                closeButton.Click += CloseButton_Click;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _hostWindow.ClosePopup(PopupWindowResults.Cancel);
        }

        PopupWindowBase _hostWindow;

        private void TitlePanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && _hostWindow != null)
            {
                _hostWindow.DragMove();
            }
        }
    }
}
