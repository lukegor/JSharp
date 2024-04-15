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
                BorderType.Reflect101 => Kernels.BorderTypeReflect101,
                BorderType.Wrap => Kernels.BorderTypeWrap,
                _ => throw new NotImplementedException(),
            };
        }

        private static Dictionary<string, BorderType> kernelBorderToBorderTypeMapping = new Dictionary<string, BorderType>
        {
            { Kernels.BorderTypeIsolated, BorderType.Isolated },
            { Kernels.BorderTypeReflect, BorderType.Reflect },
            { Kernels.BorderTypeReplicate, BorderType.Replicate }
        };

        public static BorderType BorderizeLocalizedBorderType(string localizedBorderType)
        {
            if (kernelBorderToBorderTypeMapping.TryGetValue(localizedBorderType, out BorderType value))
            {
                return value;
            }
            else
            {
                throw new InvalidOperationException("Invalid kernel border type.");
            }
        }

        public static IEnumerable<string> GetLocalizedEdgePixelsHandlingOptions(IEnumerable<BorderType> borderTypes)
        {
            return borderTypes.Select(bt => BorderTypeLocalizationHelper.LocalizeBorderType(bt));
        }
    }
}
