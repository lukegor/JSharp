using Emgu.CV.CvEnum;
using JSharp.Shared.Resources;

namespace JSharp.Utility.Utility
{
    public static class BorderTypeHelper
    {
        private static readonly Dictionary<string, BorderType> KernelBorderToBorderTypeMapping = new Dictionary<string, BorderType>
        {
            { Kernels.BorderTypeIsolated, BorderType.Isolated },
            { Kernels.BorderTypeReflect, BorderType.Reflect },
            { Kernels.BorderTypeReplicate, BorderType.Replicate },
            { Kernels.BorderTypeReflect101, BorderType.Reflect101 },
            { Kernels.BorderTypeWrap, BorderType.Wrap }
        };

        public static string LocalizeBorderType(BorderType borderType)
        {
            return EnumHelper.MapEnumToLocalString(borderType, KernelBorderToBorderTypeMapping);
        }

        public static BorderType BorderizeLocalizedBorderType(string localizedBorderType)
        {
            return EnumHelper.MapLocalStringToEnum(localizedBorderType, KernelBorderToBorderTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedEdgePixelsHandlingOptions(IEnumerable<BorderType> borderTypes)
        {
            return EnumHelper.GetLocalizedOptions(borderTypes, KernelBorderToBorderTypeMapping);
        }
    }
}
