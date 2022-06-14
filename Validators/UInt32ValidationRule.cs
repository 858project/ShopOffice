using System;
using System.Globalization;
using System.Windows.Controls;

namespace ShopOffice.Validations
{

    /// <summary>
    /// Rule for validating serial number
    /// </summary>
    public sealed class UInt32ValidationRule : ValidationRule
    {
        #region - Properties -
        /// <summary>
        /// Max value for amount
        /// </summary>
        public UInt32 MaxValue { get; set; }
        /// <summary>
        /// Min value for amount
        /// </summary>
        public UInt32 MinValue { get; set; }
        #endregion

        /// <summary>
        /// This method validates amount
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
            UInt32 currentValue = 0x00;
            if (!UInt32.TryParse(value as String, out currentValue))
            {
                return new ValidationResult(false, "Value is not valid");
            }

            // check value
            if (this.MinValue > currentValue || this.MaxValue < currentValue)
            {
                return new ValidationResult(false, "Value is not valid");
            }

            // serial number is valid
            return ValidationResult.ValidResult;
        }
    }
}
