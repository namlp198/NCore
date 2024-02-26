using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Text;

namespace ValidationToolkit
{
    public class TraceValidationRule : ValidationRule
    {
        public string PropertyName
        {
            get;
            set;
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            StringBuilder buidler = new StringBuilder();
            Debug.WriteLine(buidler.Append("TraceValidationRule for '")
                                .Append(PropertyName)
                                .Append("' called. ValidationStep='")
                                .Append(ValidationStep.ToString())
                                .Append("'").ToString());

            return ValidationResult.ValidResult;
        }
    }
}
