using System.Globalization;
using System.Windows.Data;
using JSharp.Utility;

namespace JSharp.Converters
{
    public class ThresholdingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            ThresholdingType thresholdingType = (ThresholdingType)value;
            return ThresholdingTypeHelper.MapThresholdingTypeToLocalString(thresholdingType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string localizedThresholdingType = value.ToString();
            return ThresholdingTypeHelper.MapLocalStringToThresholdingType(localizedThresholdingType);
        }
    }
}
