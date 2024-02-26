using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NpcCore.Wpf.Ex
{
    public class BubbleContextMenu : ContextMenu
    {
        public BubbleContextMenu()
        {
            this.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(OnMenuItemClick));
        }

        public static readonly RoutedEvent MenuItemClickEvent = EventManager.RegisterRoutedEvent(
            "MenuItemClickEvent", RoutingStrategy.Bubble, typeof(RoutedEventArgs), typeof(BubbleContextMenu));

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            var popup = this.Parent as System.Windows.Controls.Primitives.Popup;
            if (popup != null)
            {
                var target = popup.PlacementTarget as FrameworkElement;
                if (target != null)
                {
                    target.RaiseEvent(new RoutedEventArgs(BubbleContextMenu.MenuItemClickEvent, e.Source));
                }
            }
            
        }

    }
}
