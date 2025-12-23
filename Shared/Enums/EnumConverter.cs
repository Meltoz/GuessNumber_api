using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public static class EnumConverter
    {
        public static bool TryConvert<T>(string value, out T result)
       where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = default;
                return false;
            }

            return Enum.TryParse(value, ignoreCase: true, out result)
                   && Enum.IsDefined(typeof(T), result);
        }
    }
}
