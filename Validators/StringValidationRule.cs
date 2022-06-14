using System;
using System.Globalization;
using System.Windows.Controls;

namespace ShopOffice.Validations
{
    /// <summary>
    /// Rule for validating serial number
    /// </summary>
    public sealed class StringValidationRule : ValidationRule
    {
        #region - Properties -
        /// <summary>
        /// Max length for serial number
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// Min length for serial number
        /// </summary>
        public int MinLength { get; set; }
        #endregion

        /// <summary>
        /// This method validates serial number
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="cultureInfo">Current culture</param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // check value
            if (value == null || value.GetType() != typeof(String))
            {
                return new ValidationResult(false, "Value is not valid");
            }

            // get string value
            String? currentValue = value as String;

            // check length
            if (currentValue == null || this.MinLength > currentValue.Length || this.MaxLength < currentValue.Length)
            {
                return new ValidationResult(false, "Value is not valid");
            }

            // serial number is valid
            return ValidationResult.ValidResult;
        }
    }
}
