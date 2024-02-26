using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace NpcCore.Wpf.Controls.Behaviors
{
    public class MouseLeftButtonContextMenuSupportBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty AllowMouseLeftMouseDownProperty =
             DependencyProperty.RegisterAttached("AllowMouseLeftMouseDown", typeof(bool), typeof(MouseLeftButtonContextMenuSupportBehavior), new FrameworkPropertyMetadata(false));

        public static bool GetAllowMouseLeftMouseDown(UIElement obj)
        {
            return (bool)obj.GetValue(AllowMouseLeftMouseDownProperty);
        }

        public static void SetAllowMouseLeftMouseDown(UIElement obj, bool value)
        {
            obj.SetValue(AllowMouseLeftMouseDownProperty, value);
        }



        protected override void OnAttached()
        {
            base.OnAttached();

            if (this.AssociatedObject != null)
            {
                AssociatedObject.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnPreviewMouseLeftButtonDownEvent));
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (this.AssociatedObject != null)
            {
                AssociatedObject.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnPreviewMouseLeftButtonDownEvent));
            }
        }

        private void OnPreviewMouseLeftButtonDownEvent(object sender, RoutedEventArgs e)
        {
            var fe = e.Source as FrameworkElement;
            if (fe != null)
            {
                if (true.Equals(fe.GetValue(MouseLeftButtonContextMenuSupportBehavior.AllowMouseLeftMouseDownProperty)))
                {
                    var cm = ContextMenuService.GetContextMenu(fe);
                    if (cm != null)
                    {
                        var mouseDownEvent = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Right)
                        {
                            RoutedEvent = Mouse.MouseUpEvent,
                            Source = fe,
                        };
                        InputManager.Current.ProcessInput(mouseDownEvent);

                        e.Handled = true;
                    }
                }
            }
        }




    }
}
