using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using ValidationToolkit;
using System.Windows;
using System.Windows.Media;

namespace NpcCore.Wpf.Converters
{
    public class PrameterValue
    {
        public PrameterValue()
        {
            StringValue = "";
            DoubleValue = 0.0;
        }
        public string StringValue { get; set; }
        public double DoubleValue { get; set; }
    }

    public class PrameterAction
    {
        public PrameterAction()
        {
            InputValue = 0;
            OutputValue = 0;
        }
        public int InputValue { get; set; }
        public int OutputValue { get; set; }
    }


    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) *
                   System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AdditionValueConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) + System.Convert.ToDouble(parameter);
        }
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDouble(value) - System.Convert.ToDouble(parameter);
        }
    }

    [ValueConversion(typeof(Color), typeof(Brush))]
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class ValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                return value.ToString();
            }
            catch
            {

            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt32(value.ToString());
            }
            catch
            {

            }
            return 0;

        }
    }

    public class MarginThumbControlConverter : IValueConverter
    {
        public enum Orientation
        {
            Horizontal = 0,
            Vertical = 1
        }
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            Thickness marginOrigin = new Thickness();
            Thickness marginNew = new Thickness();
            if (marginOrigin != null && marginNew != null)
            {
                marginOrigin.Left = System.Convert.ToDouble(value);
                marginOrigin.Top = System.Convert.ToDouble(value);
                marginOrigin.Right = System.Convert.ToDouble(value);
                marginOrigin.Bottom = System.Convert.ToDouble(value);
                var orientation = (Orientation)Enum.Parse(typeof(Orientation), (string)parameter);
                if (orientation == Orientation.Horizontal)
                {
                    marginNew.Left = (-1) * marginOrigin.Left * 0.5;
                    marginNew.Right = (-1) * marginOrigin.Right * 0.5;
                }
                else
                {
                    marginNew.Top = (-1) * marginOrigin.Top * 0.5;
                    marginNew.Bottom = (-1) * marginOrigin.Bottom * 0.5;
                }
            }
            return marginNew;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
