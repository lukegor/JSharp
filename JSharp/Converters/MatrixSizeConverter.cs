using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace JSharp.Converters
{
    public class MatrixSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert integer to string
            int size = (int)value;
            return $"{size}x{size}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert ComboBoxItem to string
            if (value is ComboBoxItem comboBoxItem)
            {
                string sizeString = comboBoxItem.Content as string;
                if (sizeString != null)
                {
                    if (int.TryParse(sizeString.Substring(0, 1), out int size))
                    {
                        return size;
                    }
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
