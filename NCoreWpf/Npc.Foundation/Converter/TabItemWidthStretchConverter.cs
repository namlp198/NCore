﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Npc.Foundation.Converter
{
    public class TabItemWidthStretchConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // [NCS-2695] CID 171140 Dereference null return (stat)
            //if (values != null && values.Length > 0 && values[0] is TabControl tabControl)
            //{
            //    //TabControl tabControl = values[0] as TabControl;
            //    double width = (tabControl.ActualWidth - tabControl.Items.Count) / tabControl.Items.Count;
            //    //Subtract 1, otherwise we could overflow to two rows.
            //    return (width <= 1) ? 0 : (width - 1);
            //}
            //else
            //{
            //    return 0;
            //}
            if (values != null && values.Length > 0 && values[0] is TabControl tabControl && tabControl.Items != null)
            {
                double width = (tabControl.ActualWidth - tabControl.Items.Count) / tabControl.Items.Count;
                return (width <= 1) ? 0 : (width - 1);
            }
            else
            {
                return 0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
