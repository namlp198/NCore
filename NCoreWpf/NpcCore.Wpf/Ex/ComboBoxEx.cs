using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace NpcCore.Wpf.Ex
{
    public class ComboBoxEx : ComboBox
    {
        public DataTemplate SelectedItemDataTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemDataTemplateProperty); }
            set { SetValue(SelectedItemDataTemplateProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemDataTemplateProperty =
            DependencyProperty.Register("SelectedItemDataTemplate", typeof(DataTemplate), typeof(ComboBoxEx));

    }
}
