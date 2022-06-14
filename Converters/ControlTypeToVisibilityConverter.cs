using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ShopOffice.Enums;
using ShopOffice.Controls;

namespace ShopOffice.Converters
{
    public class ControlTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && parameter != null && parameter is ControlTypes)
            {
                switch (((ControlTypes)parameter))
                {
                    case ControlTypes.Sale:
                        return value.GetType() == typeof(SaleControl) ? Visibility.Visible : Visibility.Collapsed;
                    case ControlTypes.Products:
                        return value.GetType() == typeof(ProductsControl) ? Visibility.Visible : Visibility.Collapsed;
                    default:
                        break;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
