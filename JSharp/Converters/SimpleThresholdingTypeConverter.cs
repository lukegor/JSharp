using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JSharp.Converters
{
    public class SimpleThresholdingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            SimpleThresholdingMethod thresholdingType = (SimpleThresholdingMethod)value;
            return SimpleThresholdingTypeHelper.MapThresholdingTypeToLocalString(thresholdingType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string localizedThresholdingType = value.ToString();
            return SimpleThresholdingTypeHelper.MapLocalStringToThresholdingType(localizedThresholdingType);
        }
    }
}
