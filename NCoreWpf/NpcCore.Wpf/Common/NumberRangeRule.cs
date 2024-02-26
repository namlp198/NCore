using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ValidationToolkit
{
    public class IntegerRangeRule : ValidationRule
    {
        public string Name
        {
            get;
            set;
        }

        int min = int.MinValue;
        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        int max = int.MaxValue;
        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                if (Name != null && Name.Length == 0)
                    Name = "Field";
                try
                {
                    if ((value.ToString()).Length > 0)
                    {
                        int val = Int32.Parse(value.ToString());
                        if (val > max)
                            return new ValidationResult(false, Name + " must be <= " + Max + ".");
                        if (val < min)
                            return new ValidationResult(false, Name + " must be >= " + Min + ".");
                    }
                }
                catch (Exception)
                {
                    return new ValidationResult(false, Name + " is not in a correct numeric format.");
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}
