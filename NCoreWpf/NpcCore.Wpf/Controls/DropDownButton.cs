using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace NpcCore.Wpf.Controls
{
    public class DropDownButton : ToggleButton
    {
        #region Fields
        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register("Menu", typeof(OverFlowControl), typeof(DropDownButton), new UIPropertyMetadata(null, OnMenuChanged));
        public static readonly DependencyProperty IsFocusedItemProperty = DependencyProperty.Register("IsFocusedItem", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty ItemMouseOverProperty = DependencyProperty.Register("ItemMouseOver", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata((bool)false, OnItemMoveChanged));
        public static readonly DependencyProperty IsMenuItemOpenProperty = DependencyProperty.Register("IsMenuItemOpen", typeof(bool), typeof(DropDownButton), new FrameworkPropertyMetadata((bool)false));

        #endregion
        #region Constructor
        public DropDownButton()
        {

        }
        #endregion
        #region Properties
        public OverFlowControl Menu
        {
            get
            {
                return (OverFlowControl)GetValue(MenuProperty);
            }
            set
            {
                SetValue(MenuProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Click += OnclickEvent;

        }

        public bool IsFocusedItem
        {
            get
            {
                return (bool)GetValue(IsFocusedItemProperty);
            }
            set
            {
                SetValue(IsFocusedItemProperty, value);

            }

        }

        public bool IsMenuItemOpen
        {
            get
            {
                return (bool)GetValue(IsMenuItemOpenProperty);
            }
            set
            {
                SetValue(IsMenuItemOpenProperty, value);

            }
        }

        public bool ItemMouseOver
        {
            get
            {
                return (bool)GetValue(ItemMouseOverProperty);
            }
            set
            {
                SetValue(ItemMouseOverProperty, value);

            }
        }
        #endregion
        #region Methods
        private static void OnMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnItemMoveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton itemMove = d as DropDownButton;

            if (itemMove != null)
            {
                itemMove.IsMenuItemOpen = false;
                if (itemMove.Menu != null)
                {
                    itemMove.IsMenuItemOpen = itemMove.Menu.IsOpen;
                    itemMove.Menu.Closed += IsclosedEvent;
                }
            }
        }

        public static void IsclosedEvent(object sender, RoutedEventArgs e)
        {
            OverFlowControl btnpopup = sender as OverFlowControl;
            if (btnpopup != null)
            {
                DropDownButton closedPopup = btnpopup.PlacementTarget as DropDownButton;
                if (closedPopup != null)
                {
                    closedPopup.IsMenuItemOpen = false;
                    if (closedPopup.Menu != null)
                    {
                        closedPopup.Menu.Closed -= IsclosedEvent;
                    }
                }
            }

        }



        private void OnclickEvent(object sender, RoutedEventArgs e)
        {
            DropDownButton dropdownbutton = sender as DropDownButton;
            if (dropdownbutton != null)
            {
                if (dropdownbutton.Menu != null)
                {
                    if (dropdownbutton.Menu.Items != null && dropdownbutton.Menu.Items.Count > 0)
                    {
                        dropdownbutton.Menu.DataContext = this.DataContext;
                        dropdownbutton.Menu.PlacementTarget = this;
                        dropdownbutton.Menu.Placement = PlacementMode.Bottom;
                        dropdownbutton.Menu.IsOpen = true;

                    }
                    else
                    {
                        dropdownbutton.Menu.IsOpen = false;
                    }
                }
            }
            ;
        }

        //protected override void OnClick()
        //{

        //    if (Menu != null)
        //    {
        //        if (Menu.Items != null && Menu.Items.Count > 0)
        //        {
        //            Menu.DataContext = this.DataContext;
        //            Menu.PlacementTarget = this;
        //            Menu.Placement = PlacementMode.Bottom;
        //            Menu.IsOpen = true;

        //        }
        //        else
        //        {
        //            Menu.IsOpen = false;
        //        }
        //    }
        //}
        #endregion
    }
}
