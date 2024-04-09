using Emgu.CV.CvEnum;
using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JSharp.Converters
{
    // for future use
    public class BorderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BorderType borderType = (BorderType)value;
            return borderType switch
            {
                BorderType.Isolated => Kernels.BorderTypeIsolated,
                BorderType.Reflect => Kernels.BorderTypeReflect,
                BorderType.Replicate => Kernels.BorderTypeReplicate,
                _ => throw new InvalidOperationException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string localizedBorderType = value.ToString();
            if (localizedBorderType.Equals(Kernels.BorderTypeIsolated)) { return BorderType.Isolated; }
            if (localizedBorderType.Equals(Kernels.BorderTypeReflect)) { return BorderType.Reflect; }
            if (localizedBorderType.Equals(Kernels.BorderTypeReplicate)) { return BorderType.Replicate; }
            else throw new InvalidOperationException();
        }
    }
}
