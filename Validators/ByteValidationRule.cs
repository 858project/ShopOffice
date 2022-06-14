using System;
using System.Globalization;
using System.Windows.Controls;

namespace ShopOffice.Validations
{
    /// <summary>
    /// Rule for validating serial number
    /// </summary>
    public sealed class ByteValidationRule : ValidationRule
    {
        #region - Properties -
        /// <summary>
        /// Max value for amount
        /// </summary>
        public Byte MaxValue { get; set; }
        /// <summary>
        /// Min value for amount
        /// </summary>
        public Byte MinValue { get; set; }
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
            Byte currentValue = 0x00;
            if (!Byte.TryParse(value as String, out currentValue))
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
