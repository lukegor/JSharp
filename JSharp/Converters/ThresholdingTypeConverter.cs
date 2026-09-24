using JSharp.Utility.Utility;
using System.Globalization;
using System.Windows.Data;

namespace JSharp.Converters
{
    /// <summary>
    /// XAML converter between <see cref="ThresholdingType"/> enum and localized <see cref="string"/>
    /// </summary>
    public class ThresholdingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            ThresholdingType thresholdingType = (ThresholdingType)value;
            return ThresholdingTypeHelper.MapThresholdingTypeToLocalString(thresholdingType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            string localizedThresholdingType = value.ToString() ?? string.Empty;
            return ThresholdingTypeHelper.MapLocalStringToThresholdingType(localizedThresholdingType);
        }
    }
}
