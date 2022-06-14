using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShopOffice.Converters
{
    /// <summary>
    /// Skonvertuje pocet na pozadovanu hodnotu
    /// </summary>
    public class IntToAmountDoubleConverter : IValueConverter
    {
        #region - Public Method -
        /// <summary>
        /// Prekonvertuje hodnotu na pozadovanu farbu
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Farba</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(int))
            {
                return System.Convert.ToDouble(((int)value) / 100.0d);
            }
            throw new Exception("Invalid value");
        }
        /// <summary>
        /// Prekovertuje farbu na hodnotu
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>Hodnota</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.GetType() == typeof(double))
            {
                return System.Convert.ToInt32(((double)value) * 100);
            }
            throw new Exception("Invalid value");
        }
        #endregion
    }
}
