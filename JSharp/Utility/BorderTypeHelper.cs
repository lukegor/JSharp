using Emgu.CV.CvEnum;
using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public static class BorderTypeHelper
    {
        private static readonly Dictionary<string, BorderType> kernelBorderToBorderTypeMapping = new Dictionary<string, BorderType>
        {
            { Kernels.BorderTypeIsolated, BorderType.Isolated },
            { Kernels.BorderTypeReflect, BorderType.Reflect },
            { Kernels.BorderTypeReplicate, BorderType.Replicate },
            { Kernels.BorderTypeReflect101, BorderType.Reflect101 },
            { Kernels.BorderTypeWrap, BorderType.Wrap }
        };

        public static string LocalizeBorderType(BorderType borderType)
        {
            return EnumHelper.MapEnumToLocalString(borderType, kernelBorderToBorderTypeMapping);
        }

        public static BorderType BorderizeLocalizedBorderType(string localizedBorderType)
        {
            return EnumHelper.MapLocalStringToEnum(localizedBorderType, kernelBorderToBorderTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedEdgePixelsHandlingOptions(IEnumerable<BorderType> borderTypes)
        {
            return EnumHelper.GetLocalizedOptions(borderTypes, kernelBorderToBorderTypeMapping);
        }
    }
}
