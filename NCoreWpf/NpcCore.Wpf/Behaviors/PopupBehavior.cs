using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NpcCore.Wpf.Controls.Behaviors
{
    public class PopupClosingBehavior
    {
        /// <summary>
        /// Get WindowPopupProperty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static System.Windows.Controls.Primitives.Popup GetWindowPopup(DependencyObject obj)
        {
            return (System.Windows.Controls.Primitives.Popup)obj.GetValue(WindowPopupProperty);
        }

        /// <summary>
        /// Set WindowPopupProperty
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        private static void SetWindowPopup(DependencyObject obj, System.Windows.Controls.Primitives.Popup value)
        {
            var popup = GetWindowPopup(obj);
            if (popup != null && !popup.Equals(value))
            {
                popup.IsOpen = false;
                ((FrameworkElement)obj).PreviewMouseUp -= ContainerOnPreviewMouseDown;
            }

            obj.SetValue(WindowPopupProperty, value);
        }

        /// <summary>
        /// WindowPopupProperty
        /// </summary>
        private static readonly DependencyProperty WindowPopupProperty = DependencyProperty.RegisterAttached("WindowPopup", typeof(System.Windows.Controls.Primitives.Popup), typeof(PopupClosingBehavior));

        /// <summary>
        /// Get PopupContainerProperty
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ContentControl GetPopupContainer(DependencyObject obj)
        {
            return (ContentControl)obj.GetValue(PopupContainerProperty);
        }

        /// <summary>
        /// Set PopupContainerProperty
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetPopupContainer(DependencyObject obj, ContentControl value)
        {
            obj.SetValue(PopupContainerProperty, value);
        }

        /// <summary>
        /// PopupContainerProperty
        /// </summary>
        public static readonly DependencyProperty PopupContainerProperty = DependencyProperty.RegisterAttached("PopupContainer", typeof(ContentControl), typeof(PopupClosingBehavior), new PropertyMetadata(OnPopupContainerChanged));

        /// <summary>
        /// Poup Container Changed 이벤트 처리
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPopupContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var popup = (System.Windows.Controls.Primitives.Popup)d;
            var contentControl = e.NewValue as ContentControl;

            //popup.LostFocus += (sender, args) =>
            //{
            //    var popup1 = (System.Windows.Controls.Primitives.Popup)sender;
            //    popup.IsOpen = false;
            //    if (contentControl != null)
            //        contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            //};
            popup.Opened += (sender, args) =>
            {
                var popup1 = (System.Windows.Controls.Primitives.Popup)sender;
                popup.Focus();
                SetWindowPopup(contentControl, popup1);
                contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
                contentControl.PreviewMouseDown += ContainerOnPreviewMouseDown;
            };
            //popup.PreviewMouseUp += (sender, args) =>
            //{
            //    popup.IsOpen = false;
            //    if (contentControl != null)
            //        contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            //};
            //popup.MouseLeave += (sender, args) =>
            //{
            //    //popup.IsOpen = false;
            //    if (contentControl != null)
            //        contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            //};
            popup.Unloaded += (sender, args) =>
            {
                popup.IsOpen = false;
                if (contentControl != null)
                    contentControl.PreviewMouseDown -= ContainerOnPreviewMouseDown;
            };
        }

       
        /// <summary>
        /// Popup PreviewMouseDown 이벤트 처리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseButtonEventArgs"></param>
        private static void ContainerOnPreviewMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var popup = GetWindowPopup((DependencyObject)sender);
            var parent = FindAncestorByNameVisualTree((DependencyObject)mouseButtonEventArgs.OriginalSource, popup.Name);
            if (parent == null)
            {
                popup.IsOpen = false;
                ((FrameworkElement)sender).PreviewMouseUp -= ContainerOnPreviewMouseDown;
            }
        }

        /// <summary>
        /// Name 으로 Parent Element 찾기
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DependencyObject FindAncestorByNameVisualTree(DependencyObject element, string name)
        {
            // Production Tab 에서 Recently Ispected PCB Popup 오픈 후에 상단의 Test Button 클릭시 에러 발생
            //if (element == null)
            if (element == null || element is System.Windows.Documents.Run)
                return null;

            if (element != null && (element is FrameworkElement))
            {
                FrameworkElement fe = element as FrameworkElement;
                if (fe.Name == name)
                {
                    return element;
                }
                else if (fe.Parent != null && fe.Parent is FrameworkElement && (fe.Parent as FrameworkElement).Name == name)
                {
                    return (fe.Parent as FrameworkElement);
                }
            }

            return FindAncestorByNameVisualTree(VisualTreeHelper.GetParent(element), name);
        }
    }
}
