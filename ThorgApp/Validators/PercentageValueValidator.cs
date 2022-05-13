using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GolemUI.Validators
{
    public class PercentageValueValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var text = value as string;
            float valueFloat = 0.0f;

            var result = float.TryParse(text, out valueFloat);

            if (result == false)
            {
                return new ValidationResult(false, "Failed to parse as float");
            }

            if (valueFloat < 0.0f || valueFloat > 1.0f)
            {
                return new ValidationResult(false, "Value is not between 0.0 and 1.0");
            }


            return ValidationResult.ValidResult;
        }
    }
}
