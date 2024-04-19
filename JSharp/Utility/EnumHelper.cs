using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public static class EnumHelper
    {
        public static TEnum MapLocalStringToEnum<TEnum>(string input, Dictionary<string, TEnum> mapping) where TEnum : struct, Enum
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an enum type.");
            }

            if (mapping.TryGetValue(input, out TEnum value))
            {
                return value;
            }
            else
            {
                throw new InvalidOperationException("Invalid value for the given enum type.");
            }
        }

        public static string MapEnumToLocalString<TEnum>(TEnum enumValue, Dictionary<string, TEnum> mapping) where TEnum : struct, Enum
        {
            foreach (var kvp in mapping)
            {
                if (EqualityComparer<TEnum>.Default.Equals(kvp.Value, enumValue))
                {
                    return kvp.Key;
                }
            }

            throw new NotImplementedException("Mapping not found for the provided enum value.");
        }

        public static IEnumerable<string> GetLocalizedOptions<TEnum>(IEnumerable<TEnum> enumValues, Dictionary<string, TEnum> mapping) where TEnum : struct, Enum
        {
            return enumValues.Select(enumValue => EnumHelper.MapEnumToLocalString(enumValue, mapping));
        }
    }
}
