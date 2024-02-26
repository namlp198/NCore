using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;

namespace NpcCore.Wpf.Controls
{
    public class ComboBoxControl : ComboBox
    {
        #region Fields
        private ListView _listItemsRectangle;
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
                typeof(ComboBoxControl), null);
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public static readonly DependencyProperty ContentItemSelectedProperty = DependencyProperty.Register("ContentItemSelected", typeof(string),
            typeof(ComboBoxControl), new PropertyMetadata(""));
        public string ContentItemSelected
        {
            get
            {
                return (string)GetValue(ContentItemSelectedProperty);
            }
            set
            {
                SetValue(ContentItemSelectedProperty, value);
            }
        }

        public static readonly DependencyProperty IconItemSelectedProperty = DependencyProperty.Register("IconItemSelected", typeof(Uri),
            typeof(ComboBoxControl), new UIPropertyMetadata(null));
        public Uri IconItemSelected
        {
            get
            {
                return (Uri)GetValue(IconItemSelectedProperty);
            }
            set
            {
                SetValue(IconItemSelectedProperty, value);
            }
        }

        public static readonly DependencyProperty HeightIconItemSelectedProperty = DependencyProperty.Register("HeightIconItemSelected", typeof(double),
            typeof(ComboBoxControl), new PropertyMetadata(20.0));
        public double HeightIconItemSelected
        {
            get
            {
                return (double)GetValue(HeightIconItemSelectedProperty);
            }
            set
            {
                SetValue(HeightIconItemSelectedProperty, value);
            }
        }

        public static readonly DependencyProperty WidthIconItemSelectedProperty = DependencyProperty.Register("WidthIconItemSelected", typeof(double),
            typeof(ComboBoxControl), new PropertyMetadata(20.0));
        public double WidthIconItemSelected
        {
            get
            {
                return (double)GetValue(WidthIconItemSelectedProperty);
            }
            set
            {
                SetValue(WidthIconItemSelectedProperty, value);
            }
        }

        public static readonly DependencyProperty HeightItemProperty = DependencyProperty.Register("HeightItem", typeof(double),
            typeof(ComboBoxControl), new PropertyMetadata(20.0));
        public double HeightItem
        {
            get
            {
                return (double)GetValue(HeightItemProperty);
            }
            set
            {
                SetValue(HeightItemProperty, value);
            }
        }

        public static readonly DependencyProperty WidthItemProperty = DependencyProperty.Register("WidthItem", typeof(double),
            typeof(ComboBoxControl), new PropertyMetadata(20.0));
        public double WidthItem
        {
            get
            {
                return (double)GetValue(WidthItemProperty);
            }
            set
            {
                SetValue(WidthItemProperty, value);
            }
        }

        public static readonly DependencyProperty HeightPopupProperty = DependencyProperty.Register("HeightPopup", typeof(double),
            typeof(ComboBoxControl), new PropertyMetadata(200.0));
        public double HeightPopup
        {
            get
            {
                return (double)GetValue(HeightPopupProperty);
            }
            set
            {
                SetValue(HeightPopupProperty, value);
            }
        }

        public static readonly DependencyProperty WidthPopupProperty = DependencyProperty.Register("WidthPopup", typeof(double),
            typeof(ComboBoxControl), new PropertyMetadata(200.0));
        public double WidthPopup
        {
            get
            {
                return (double)GetValue(WidthPopupProperty);
            }
            set
            {
                SetValue(WidthPopupProperty, value);
            }
        }
        #endregion
        #region Constructor
        static ComboBoxControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxControl), new FrameworkPropertyMetadata(typeof(ComboBoxControl)));
        }
        #endregion
        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _listItemsRectangle = this.GetTemplateChild("ListItemsRectangle") as ListView;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            Object selectedItemObj = SelectedItem;
            if (selectedItemObj != null)
            {
                if (selectedItemObj.GetType() == typeof(ItemCustomControl))
                {
                    ItemCustomControl itemSelected = selectedItemObj as ItemCustomControl;
                    if (itemSelected != null)
                    {
                        if (itemSelected.Content != null)
                            ContentItemSelected = itemSelected.Content.ToString();
                        if (itemSelected.IconSource != null)
                            IconItemSelected = itemSelected.IconSource;
                    }
                }
                else
                {
                    if (_listItemsRectangle != null)
                    {
                        Type typeObj = selectedItemObj.GetType();
                        if (typeObj != null)
                        {
                            PropertyInfo nameProperty = typeObj.GetProperty("Name");
                            if (nameProperty != null)
                            {
                                ContentItemSelected = nameProperty.GetValue(selectedItemObj).ToString();
                            }
                            PropertyInfo iconProperty = typeObj.GetProperty("Icon");
                            if (iconProperty != null)
                            {
                                Uri objUri = iconProperty.GetValue(selectedItemObj) as Uri;
                                if (objUri != null)
                                    IconItemSelected = objUri;
                            }
                        }
                        _listItemsRectangle.SelectedItem = selectedItemObj;
                    }
                }
            }
            base.OnSelectionChanged(e);
        }


        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            if (this.ItemsSource != null)
            {
                if (_listItemsRectangle != null)
                {
                    _listItemsRectangle.ItemsSource = this.ItemsSource;
                    _listItemsRectangle.SelectionChanged += new SelectionChangedEventHandler(UpdateSelectionItem);
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
        }

        public void UpdateSelectionItem(object sender, SelectionChangedEventArgs e)
        {
            if (_listItemsRectangle != null)
            {
                Object itemSelected = _listItemsRectangle.SelectedItem;
                if (itemSelected != null)
                {
                    Type typeObj = itemSelected.GetType();
                    if (typeObj != null)
                    {
                        PropertyInfo nameProperty = typeObj.GetProperty("Name");
                        if (nameProperty != null)
                        {
                            ContentItemSelected = nameProperty.Name;
                        }
                        PropertyInfo iconProperty = typeObj.GetProperty("Icon");
                        if (iconProperty != null)
                        {
                            Uri objUri = iconProperty.GetValue(itemSelected) as Uri;
                            if (objUri != null)
                                IconItemSelected = objUri;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
