using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Npc.Foundation.Converter
{
    public class DoubleToMultiplicationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double n = 0.0d;
            double.TryParse((value ?? "").ToString(), out n);

            double multiple = 1.0d;
            double.TryParse((parameter ?? "").ToString(), out multiple);

            return n * multiple;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double n = 0.0d;
            double.TryParse((value ?? "").ToString(), out n);

            double multiple = 1.0d;
            double.TryParse((parameter ?? "").ToString(), out multiple);

            return n / multiple;
        }
    }

    public class RectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = 0;
            double height = 0;

            if (values != null && values.Length == 2)
            {
                double.TryParse(values[0].ToString(), out width);
                double.TryParse(values[1].ToString(), out height);
            }

            return new Rect(new Size(width, height));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToRoundedCornerradiusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CornerRadius radius = new CornerRadius(0);
            if (value != null && radius != null)
            {
                radius.TopLeft = (double)value * 0.5;
                radius.TopRight = (double)value * 0.5;
                radius.BottomRight = (double)value * 0.5;
                radius.BottomLeft = (double)value * 0.5;
            }
            return radius;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToSqrtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double d = 0.0d;
            double.TryParse((value ?? "").ToString(), out d);

            return System.Math.Sqrt(d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToSquaredConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double n = 0.0d;
            double.TryParse((value ?? "").ToString(), out n);

            return System.Math.Pow(n, 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double n = 0.0d;
            double.TryParse((value ?? "").ToString(), out n);

            return System.Math.Sqrt(n);
        }
    }

    public class DoubleToMinusConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            double n = 0.0d;
            double.TryParse((value[0] ?? "").ToString(), out n);

            double multiple = 1.0d;
            double.TryParse((value[1] ?? "").ToString(), out multiple);

            return n - multiple;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class DoubleToPlusConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            double n = 0.0d;
            double.TryParse((value[0] ?? "").ToString(), out n);

            double multiple = 1.0d;
            double.TryParse((value[1] ?? "").ToString(), out multiple);

            return n + multiple;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class MultiValueEqualityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //return values?.All(o => o?.Equals(values[0]) == true) == true || values?.All(o => o == null) == true;

            double n = 0.0d;
            double.TryParse((values[0] ?? "").ToString(), out n);

            double m = 1.0d;
            double.TryParse((values[1] ?? "").ToString(), out m);

            return n == m;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
