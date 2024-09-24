using System.Globalization;
using System.Windows.Data;
using Emgu.CV.CvEnum;
using JSharp.Utility;

namespace JSharp.Converters
{
    public class BorderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            BorderType borderType = (BorderType)value;
            return BorderTypeHelper.LocalizeBorderType(borderType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string localizedBorderType = value.ToString();
            return BorderTypeHelper.BorderizeLocalizedBorderType(localizedBorderType);
        }
    }
}
