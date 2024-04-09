using Emgu.CV.CvEnum;
using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public static class BorderTypeLocalizationHelper
    {
        public static string LocalizeBorderType(BorderType borderType)
        {
            return borderType switch
            {
                BorderType.Isolated => Kernels.BorderTypeIsolated,// Assuming you have localized resource strings
                BorderType.Reflect => Kernels.BorderTypeReflect,
                BorderType.Replicate => Kernels.BorderTypeReplicate,
                _ => throw new NotImplementedException(),
            };
        }

        public static BorderType BorderizeLocalizedBorderType(string localizedBorderType)
        {
            if (localizedBorderType.Equals(Kernels.BorderTypeIsolated)) { return BorderType.Isolated; }
            if (localizedBorderType.Equals(Kernels.BorderTypeReflect)) { return BorderType.Reflect; }
            if (localizedBorderType.Equals(Kernels.BorderTypeReplicate)) { return BorderType.Replicate; }
            else throw new InvalidOperationException();
        }
    }
}
