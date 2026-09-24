using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace JSharp.Converters
{
    /// <summary>
    /// XAML converter between <see cref="int"/> and <see cref="string"/> for matrix size (e.g. 3 -> "3x3" and "3x3" -> 3)
    /// </summary>
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
                if (comboBoxItem.Content is string sizeString)
                {
                    if (int.TryParse(sizeString.AsSpan(0, 1), out int size))
                    {
                        return size;
                    }
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
