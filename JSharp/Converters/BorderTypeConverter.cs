using JSharp.Shared.Imaging;
using JSharp.Utility.Utility;
using System.Globalization;
using System.Windows.Data;

namespace JSharp.Converters
{
    /// <summary>
    /// XAML converter between <see cref="BorderMode"/> enum and its localized <see cref="string"/> representation.
    /// </summary>
    public class BorderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            BorderMode borderType = (BorderMode)value;
            return BorderTypeHelper.LocalizeBorderType(borderType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            string localizedBorderType = value.ToString() ?? string.Empty;
            return BorderTypeHelper.BorderizeLocalizedBorderType(localizedBorderType);
        }
    }
}
