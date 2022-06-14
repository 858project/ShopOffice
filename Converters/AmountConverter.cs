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
    public class AmountConverter : IValueConverter
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
            UInt64 amountValue = 0x00;
            if (!UInt64.TryParse(value.ToString(), out amountValue))
            {
                amountValue = 0x00;
            }
            return String.Format("{0:0.00} €", (amountValue / 100.0f));
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
            throw new NotSupportedException();
        }
        #endregion
    }
}
