using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JSharp.Validation.Validation_rules
{
    public class RangeValidationRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Value is required.");
            }

            if (!int.TryParse(value.ToString(), NumberStyles.Integer, cultureInfo, out int intValue))
            {
                return new ValidationResult(false, "Invalid integer value.");
            }

            if (intValue < Min || intValue > Max)
            {
                return new ValidationResult(false, $"Value should be between {Min} and {Max}.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
